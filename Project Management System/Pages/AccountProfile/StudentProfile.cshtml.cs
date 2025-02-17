using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project_Management_System.Data;
using System.ComponentModel.DataAnnotations;

namespace Project_Management_System.Pages.AccountProfile
{
    // Created by Harry

    [Authorize(Roles = "Student")] //Only allow students to access page
    public class StudentProfileModel : PageModel
    {
        private readonly UserManager<SPMS_Student> _userManager;
        private readonly SignInManager<SPMS_User> _signInManager;
        private readonly SPMS_Context _db;
        private readonly Project_Management_System.Data.SPMS_Context _context;

        public StudentProfileModel(Project_Management_System.Data.SPMS_Context context,
            UserManager<SPMS_Student> userManager,
            SignInManager<SPMS_User> signInManager,
            SPMS_Context db)
        {
            _context = context;
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IList<Course> Course { get; set; } = default!; // Create property (list of courses)

        public async Task<IActionResult> OnGetAsync()
        { // Finds details of logged in user and loads in a list of courses
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (_context.Course != null)
            {
                Course = await _context.Course.ToListAsync();
            }

            return Page();
        }

        [BindProperty]
        public IFormFile UploadedImage { get; set; } = default!; // Property to store uploaded file

        public async Task<IActionResult> OnPostAsync(IFormFile uploadedImage)
        { // Checks if form has an image
            if (uploadedImage != null && uploadedImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                { // Converts image to byte
                    await uploadedImage.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();
                    
                    // Byte is then stored in database table
                    
                    var user = await _userManager.GetUserAsync(User);
                    if (user != null)
                    {
                        user.ProfilePicture = imageBytes;
                        await _userManager.UpdateAsync(user);
                        await _signInManager.RefreshSignInAsync(user);
                    }
                    
                }
            }
            //Reload page and user details
            return RedirectToPage();
        }
    }
}
