using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project_Management_System.Data;
using Project_Management_System.Migrations;

namespace Project_Management_System.Pages.AccountProfile
{
    // Made by Harry

    [Authorize(Roles = "Staff")] // Only allow staff to view page
    public class InterestEditModel : PageModel
    {
        private readonly UserManager<SPMS_Staff> _UserManager;
        private readonly SPMS_Context _context;
        private readonly SPMS_Context _db;

        public InterestEditModel(SPMS_Context context, SPMS_Context db, UserManager<SPMS_Staff> userManager)
        {
            _UserManager = userManager;
            _context = context;
            _db = db;
        }

        // Declare variables and properties

        [BindProperty]
        public StaffInterest StaffInterest { get; set; } = default!;

        [BindProperty]
        public int InterestID { get; set; }

        public IList<StaffInterest> StaffInterests { get; set; } = new List<StaffInterest>();

        public async Task<IActionResult> OnGetAsync()
        {
            //Load details for logged in user
            var user = await _UserManager.GetUserAsync(User);

            if (user == null)
            {
                return Page();
            }

            var interest = await _context.StaffInterest.FirstOrDefaultAsync(); //reads all interest from context
            if (interest == null)
            {
                return Page();
            }
            StaffInterest = interest;

            StaffInterests = await _context.StaffInterest.FromSqlRaw("SELECT * FROM StaffInterest WHERE StaffID = {0}", user.Id).ToListAsync();

            return Page(); // Only loads in interests belonging to logged in user
        }

        public async Task<IActionResult> OnPostAsync(string command)
        {
            if (command == "Delete") // If delete button is clicked find the interest in table and then remove
            {
                var interestToDelete = await _context.StaffInterest.FindAsync(InterestID);

                if (interestToDelete == null)
                {
                    return Page();
                }

                _context.StaffInterest.Remove(interestToDelete);
                await _context.SaveChangesAsync();

                return RedirectToPage("/AccountProfile/StaffProfile");
            }
            else if (command == "Save") // Save any change
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("/AccountProfile/StaffProfile");

            }
            var interestToUpdate = await _context.StaffInterest.FindAsync(InterestID);

            if (interestToUpdate == null)
            {
                return NotFound();
            }

            interestToUpdate.Interest = StaffInterest.Interest;
        

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InterestExists(InterestID))
                {
                    return Page();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/AccountProfile/StaffProfile");
        }




        private bool InterestExists(int id) // Flag to check interest exists
        {
            return _context.StaffInterest.Any(e => e.InterestID == id);
        }
    }
}
