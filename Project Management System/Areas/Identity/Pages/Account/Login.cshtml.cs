// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_Management_System.Data;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using Project_Management_System.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Project_Management_System.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<SPMS_User> _signInManager;
        private readonly UserManager<SPMS_User> _userManager;
        private readonly IUserStore<SPMS_User> _userStore;
        private readonly IUserEmailStore<SPMS_User> _emailStore;
        private readonly ILogger<LoginModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly SPMS_Context _db;

        // Declare and set properties and variables

        public StaffDivision StaffDivisions = new();

        public LoginModel(
            UserManager<SPMS_User> userManager,
            IUserStore<SPMS_User> userStore,
            SignInManager<SPMS_User> signInManager,
            ILogger<LoginModel> logger,
            IEmailSender emailSender,
            Project_Management_System.Data.SPMS_Context context,
            SPMS_Context db)
        {
            _db = db;
            _context = context;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        private readonly Project_Management_System.Data.SPMS_Context _context;

        [BindProperty]
        public LoginViewModel LoginInput { get; set; }

        [BindProperty]
        public StudentRegisterViewModel RegisterInput { get; set; }

        [BindProperty]
        public StaffRegisterViewModel StaffRegisterInput { get; set; } // Import models

        public IList<Division> Division { get; set; } = default!;

        public IList<Course> Course { get; set; } = default!;

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public void NewDivision(string StaffID, int DivisionID)
        { //Allocates staff their division
            StaffDivisions.StaffID = StaffID;
            StaffDivisions.DivisionID = DivisionID; // links user account to staff account
            _db.StaffDivision.Add(StaffDivisions);
            _db.SaveChanges();
            // function that assigns divsion to staff user and saves to database
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (_context.Division != null)
            {
                Division = await _context.Division.ToListAsync();
            }

            if (_context.Course != null)
            {
                Course = await _context.Course.ToListAsync();
            }

            // Reads in course and division

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostLoginAsync(string returnUrl = null)
        {
            ModelState.Clear();
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(LoginInput.Email, LoginInput.Password, LoginInput.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = LoginInput.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    await OnGetAsync();

                    // Set up the alert message
                    string alertMessage = "Invalid login attempt. Please check your email and password.";

                    // Render the alert message on the page
                    TempData["ErrorMessage"] = alertMessage;

                    // Redirect the user to the login page
                    return RedirectToPage("/Account/Login");
                }

            }

            await OnGetAsync();
            // If we got this far, something failed, redisplay form
            return Page();
        }


        public async Task<IActionResult> OnPostStudentRegisterAsync(string returnUrl = null)
        {
            ModelState.Clear();
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateStudent(); // Run function

                await _userStore.SetUserNameAsync(user, RegisterInput.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, RegisterInput.Email, CancellationToken.None);
                user.FirstName = RegisterInput.FirstName;
                user.Surname = RegisterInput.Surname;
                user.StudentNo = RegisterInput.StudentNo;
                user.CourseID = RegisterInput.CourseID;
                var result = await _userManager.CreateAsync(user, RegisterInput.Password); // Store input from form


                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(user, "Student"); // Assign role

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(RegisterInput.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = RegisterInput.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            await OnGetAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostStaffRegisterAsync(string returnUrl = null)
        {
            ModelState.Clear(); // Clear previous errors
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = CreateStaff();
                // Create structure and store details from form
                await _userStore.SetUserNameAsync(user, StaffRegisterInput.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, StaffRegisterInput.Email, CancellationToken.None);
                user.FirstName = StaffRegisterInput.FirstName;
                user.Surname = StaffRegisterInput.Surname;
                var result = await _userManager.CreateAsync(user, StaffRegisterInput.Password);


                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (StaffRegisterInput.ProjectCoordinator)
                    {
                        await _userManager.AddToRoleAsync(user, "Co-ordinator");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "Staff");
                    } // Assign roles
                    
                    // Create user and account

                    var userId = await _userManager.GetUserIdAsync(user);
                    NewDivision(userId, StaffRegisterInput.DivisionID); // Assign division
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(StaffRegisterInput.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = StaffRegisterInput.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        return LocalRedirect(returnUrl);
                    }
                }
            }
            await OnGetAsync();
            // If we got this far, something failed, redisplay form
            return Page();
        }

        private SPMS_Student CreateStudent()
        {
            try
            {
                return Activator.CreateInstance<SPMS_Student>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(SPMS_Student)}'. " +
                    $"Ensure that '{nameof(SPMS_Student)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
            //Create student account structure
        }

        private SPMS_Staff CreateStaff()
        {
            try
            {
                return Activator.CreateInstance<SPMS_Staff>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(SPMS_Staff)}'. " +
                    $"Ensure that '{nameof(SPMS_Staff)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
            // Create the staff account structure
        }

        private IUserEmailStore<SPMS_User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<SPMS_User>)_userStore;
        }
        //Store email
    }
}
