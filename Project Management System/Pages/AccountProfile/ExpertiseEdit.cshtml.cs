using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project_Management_System.Data;

// Created by Harry & Scott

namespace Project_Management_System.Pages.AccountProfile
{
    [Authorize(Roles = "Staff")]
    public class ExpertiseEditModel : PageModel
    {
        private readonly UserManager<SPMS_Staff> _UserManager;
        private readonly SPMS_Context _context;
        private readonly SPMS_Context _db;

        public ExpertiseEditModel(SPMS_Context context, SPMS_Context db, UserManager<SPMS_Staff> userManager)
        {
            _UserManager = userManager;
            _context = context;
            _db = db;
        }

        [BindProperty] //Declare properties and variables
        public StaffExpertise StaffExpertise { get; set; } = default!;

        [BindProperty]
        public int ExpertiseID { get; set; }

        public IList<StaffExpertise> StaffExpertises { get; set; } = new List<StaffExpertise>();

        public async Task<IActionResult> OnGetAsync()
        { //Load in current logged in user details
            var user = await _UserManager.GetUserAsync(User);
            if (user == null)
            {
                return Page();
            }
            // Reads in list of all experise
            var expertise = await _context.StaffExpertise.FirstOrDefaultAsync();
            if (expertise == null)
            {
                return Page();
            }
            StaffExpertise = expertise;
            // Loads in expertise of only logged in user
            StaffExpertises = await _context.StaffExpertise.FromSqlRaw("SELECT * FROM StaffExpertise WHERE StaffID = {0}", user.Id).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string command)
        {
            if (command == "Delete") //If delete then remove the Expertise from context and save changes.
            {
                var expertiseToDelete = await _context.StaffExpertise.FindAsync(ExpertiseID);

                if (expertiseToDelete == null)
                {
                    return Page();
                }

                _context.StaffExpertise.Remove(expertiseToDelete);
                await _context.SaveChangesAsync();

                return RedirectToPage("/AccountProfile/StaffProfile");
            }
            else if (command == "Save") // If save then save changes
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("/AccountProfile/StaffProfile");

            }
            var expertiseToUpdate = await _context.StaffExpertise.FindAsync(ExpertiseID);

            if (expertiseToUpdate == null)
            {
                return Page();
            }

            expertiseToUpdate.Expertise = StaffExpertise.Expertise;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpertiseExists(ExpertiseID))
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

        private bool ExpertiseExists(int id)
        {
            return _context.StaffExpertise.Any(e => e.ExpertiseID == id);
        }
    }
}