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

namespace Project_Management_System.Pages.AccountProfile
{

    // Created by Harry

    [Authorize(Roles = "Staff")]

    public class ExpertiseAddModel : PageModel
    {
        private readonly UserManager<SPMS_Staff> _UserManager;
        private readonly Project_Management_System.Data.SPMS_Context _context;

        public ExpertiseAddModel(Project_Management_System.Data.SPMS_Context context, UserManager<SPMS_Staff> userManager)
        {
            _UserManager = userManager;
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public StaffExpertise StaffExpertise { get; set; } = default!; // Declare property

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Clear(); // Clear previous form errors 
            if (!ModelState.IsValid) // Return to page if errors
            {
                return Page();
            }

            // Checks what currentID is at and increments by 1 or set as 1 if nothing there
            var currentExpertise = _context.StaffExpertise.FromSqlRaw("SELECT * FROM StaffExpertise")
                .OrderByDescending(b => b.ExpertiseID)
                .FirstOrDefault();
            if(currentExpertise != null)
            {
                StaffExpertise.ExpertiseID = currentExpertise.ExpertiseID + 1; //increment last id by 1
            }
            else
            {
                StaffExpertise.ExpertiseID = 1;
            }
            // Assign new expertise and save changes
            var user = await _UserManager.GetUserAsync(User);
            
            if(user == null)
            {
                return Page();
            }

            StaffExpertise.StaffID = user.Id;

            _context.StaffExpertise.Add(StaffExpertise);
            await _context.SaveChangesAsync();

            return RedirectToPage("/AccountProfile/StaffProfile");
            
        }
    }
}
