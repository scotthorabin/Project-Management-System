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
    [Authorize(Roles = "Staff")]

    public class AddModel : PageModel
    {
        private readonly UserManager<SPMS_Staff> _UserManager;
        private readonly Project_Management_System.Data.SPMS_Context _context;

        public AddModel(Project_Management_System.Data.SPMS_Context context, UserManager<SPMS_Staff> userManager)
        {
            _UserManager = userManager;
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public StaffInterest StaffInterest { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Clear();
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var currentInterest = _context.StaffInterest.FromSqlRaw("SELECT * FROM StaffInterest")
                .OrderByDescending(b => b.InterestID)
                .FirstOrDefault();
            if(currentInterest != null)
            {
                StaffInterest.InterestID = currentInterest.InterestID + 1; //increment last id by 1
            }
            else
            {
                StaffInterest.InterestID = 1;
            }

            var user = await _UserManager.GetUserAsync(User);
            StaffInterest.StaffID = user.Id;

            _context.StaffInterest.Add(StaffInterest);
            await _context.SaveChangesAsync();

            return RedirectToPage("/AccountProfile/StaffProfile");
            
        }
    }
}
