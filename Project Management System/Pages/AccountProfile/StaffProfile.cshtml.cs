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
    //Made by Harry

    [Authorize(Roles = "Staff")] //Only allow staff to view page
    public class StaffProfileModel : PageModel
    {
        private readonly UserManager<SPMS_User> _userManager;
        private readonly SignInManager<SPMS_User> _signInManager;
        private readonly SPMS_Context _db;
        private readonly SPMS_Context _context;

        public StaffProfileModel(SPMS_Context context,
            UserManager<SPMS_User> userManager,
            SignInManager<SPMS_User> signInManager,
            SPMS_Context db)
        {
            _context = context;
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        
        // Declare and assign properties and variables

        public IList<Division> Divisions { get; set; } = default!;
        public IList<Topic> Topic { get; set; } = default!;

        public IList<StaffDivision> StaffDivisions = default!;
        public IList<StaffInterest> StaffInterest { get; private set; } = default!;
        public IList<StaffExpertise> StaffExpertise { get; private set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        { // Read in details of usdr
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (_context.StaffDivision != null)
            {
                StaffDivisions = await _context.StaffDivision.ToListAsync();
            }

            if (_context.Division != null)
            {
                Divisions = await _context.Division.ToListAsync();
            }

            // Read in list of division and staffDivision

            Topic = _db.Topic //select data from database
                .FromSqlRaw("SELECT * FROM Topic WHERE SupervisorID = {0}", user.Id)
                .ToList();

            Topic = Topic.Where(i=> i.MarkerID != null).ToList(); // Only load in projects that have been fully assigned

            StaffInterest = _db.StaffInterest //select data from database
                .FromSqlRaw("SELECT * FROM StaffInterest WHERE StaffID = {0}", user.Id)
                .ToList();

            StaffExpertise = _db.StaffExpertise //select data from database
                .FromSqlRaw("SELECT * FROM StaffExpertise WHERE StaffID = {0}", user.Id)
                .ToList();

            // Only load in topics, interests and expertise where staffID = logged in user

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int DivisionID) //takes id passed from button
        { // Finds division given the ID of the staff and removes from database
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var division = await _db.StaffDivision.FindAsync(user.Id, DivisionID);
                if (division != null)
                {
                    _db.StaffDivision.Remove(division);
                    await _db.SaveChangesAsync();
                }
            }
            //save changes
            return RedirectToPage(); //return to page
        }

        public async Task<IActionResult> OnPostAsync(IFormFile uploadedImage)
        {
            if (uploadedImage != null && uploadedImage.Length > 0)
            { // Gets image from form and converts to bytes
                using (var memoryStream = new MemoryStream())
                {
                    await uploadedImage.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();

                    // Save to database as a byte

                    var user = await _userManager.GetUserAsync(User);
                    if (user != null)
                    {
                        user.ProfilePicture = imageBytes;
                        await _userManager.UpdateAsync(user);
                        await _signInManager.RefreshSignInAsync(user);
                    }
                }
            }
            return RedirectToPage(); // Reload user profile and page
        }
    }
}
