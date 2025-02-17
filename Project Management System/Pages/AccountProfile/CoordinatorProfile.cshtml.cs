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
    [Authorize(Roles = "Co-ordinator")]
    public class CoordinatorProfileModel : PageModel
    {
        private readonly UserManager<SPMS_User> _userManager;
        private readonly SignInManager<SPMS_User> _signInManager;
        private readonly SPMS_Context _db;
        private readonly SPMS_Context _context;


        public CoordinatorProfileModel(SPMS_Context context,
            UserManager<SPMS_User> userManager,
            SignInManager<SPMS_User> signInManager,
            SPMS_Context db)
        {
            _context = context;
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        // Creates properties and variables to be used.
        public IList<Division> Divisions { get; set; } = default!;

        public IList<StaffDivision> StaffDivisions = default!;

        public IList<Topic> StudentProposals { get; set; } = default!;
        public IList<Topic> Topics { get; set; } = default!;

        public IList<UndergraduateProposal> UndergraduateProposals { get; set; } = default!;
        public IList<PostgraduateProposal> PostgraduateProposals { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User); // Get fields from logged in user
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (_context.Topic != null)
            { // Read in topics where there is a studentID assigned and no marker has been assigned (only display student proposals)
                StudentProposals = await _context.Topic.Where(i => !string.IsNullOrEmpty(i.StudentID) && string.IsNullOrEmpty(i.MarkerID)).ToListAsync();
            }
            if (_context.Topic != null)
            { // Read in list of topics from context
                Topics = await _context.Topic.ToListAsync();
            }
            if (_context.UndergraduateProposal != null)
            { // Read in undergrad proposal submissions
                UndergraduateProposals = await _context.UndergraduateProposal.ToListAsync();
            }
            if (_context.PostgraduateProposal != null)
            { // Read in postgrad proposal submissions
                PostgraduateProposals = await _context.PostgraduateProposal.ToListAsync();
            }

            if (_context.StaffDivision != null)
            { // Read in all staff and what division they are in
                StaffDivisions = await _context.StaffDivision.ToListAsync();
            }

            if (_context.Division != null)
            { // Read in division details
                Divisions = await _context.Division.ToListAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int DivisionID) //takes id passed from button
        { //Takes the divisionID of the division and locates the divion in the table then removes from database
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
            if (uploadedImage != null && uploadedImage.Length > 0) // Checks if image has been uploaded to form
            {
                using (var memoryStream = new MemoryStream())
                { 
                    await uploadedImage.CopyToAsync(memoryStream); 
                    byte[] imageBytes = memoryStream.ToArray(); // Converts image to bytes

                    // Finds user details and updates their profile picture in the database

                    var user = await _userManager.GetUserAsync(User);
                    if (user != null)
                    {
                        user.ProfilePicture = imageBytes;
                        await _userManager.UpdateAsync(user);
                        await _signInManager.RefreshSignInAsync(user); // refresh user login to reflect changes
                    }
                }
            }
            return RedirectToPage();
        }

    }


}
