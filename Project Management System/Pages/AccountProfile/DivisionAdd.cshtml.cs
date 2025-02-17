using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;
using Project_Management_System.Data;
using Project_Management_System.Migrations;

namespace Project_Management_System.Pages.AccountProfile
{
    //Created by Harry

    [Authorize(Roles = "Staff, Co-ordinator")]

    public class DivisionAddModel : PageModel
    {
        private readonly UserManager<SPMS_Staff> _UserManager;
        private readonly Project_Management_System.Data.SPMS_Context _context;

        public DivisionAddModel(Project_Management_System.Data.SPMS_Context context, UserManager<SPMS_Staff> userManager)
        {
            _UserManager = userManager;
            _context = context;
        }

        public async Task OnGetAsync()
        {
            if (_context.Division != null)
            {
                Division = await _context.Division.ToListAsync();
            }
         
        }
        // Declare variables and properties
        public IList<Division> Division { get; set; } = default!;

        public IList<StaffDivision> StaffDivisions { get; set; } = default!;

        [BindProperty]
        public StaffDivision StaffDivision { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        { //Clears any previos model errors
            ModelState.Clear();
            if (!ModelState.IsValid)
            { // Error then return to page
                await OnGetAsync();
                return Page();
            }

            var user = await _UserManager.GetUserAsync(User); // Get fields of logged in user
            // Only read in Division that staff is a member of
            if(user == null)
            {
                await OnGetAsync();
                return Page();
            }

            StaffDivisions = await _context.StaffDivision.FromSqlRaw("SELECT * FROM StaffDivision WHERE StaffID = {0}", user.Id).ToListAsync();

            if(StaffDivisions != null) //If they already belong to that division then do nothing
            {
                foreach(var Division in StaffDivisions)
                {
                    if(Division.DivisionID == StaffDivision.DivisionID)
                    {
                        await OnGetAsync();
                        return Page();
                        
                    }
                }

            }
            // Allocate staffID
            StaffDivision.StaffID = user.Id;

            _context.StaffDivision.Add(StaffDivision); //Allocate new divisionID
            await _context.SaveChangesAsync(); //Save
            if (User.IsInRole("Staff")) // Redirect to correct profile
            {
                return RedirectToPage("/AccountProfile/StaffProfile");
            }
            if (User.IsInRole("Co-ordinator"))
            {
                return RedirectToPage("/AccountProfile/CoordinatorProfile");
            }
            return RedirectToPage("/Index");
        }
    }
}
