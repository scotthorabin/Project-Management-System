using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project_Management_System.Data;

namespace Project_Management_System.Pages
{
    [Authorize(Roles = "Staff, Co-ordinator")]

    public class CreateModel : PageModel
    {
        private readonly SPMS_Context _db;
        private readonly Project_Management_System.Data.SPMS_Context _context;
        private readonly UserManager<SPMS_Staff> _UserManager;

        public CreateModel(Project_Management_System.Data.SPMS_Context context, SPMS_Context db, UserManager<SPMS_Staff> userManager)
        {
            _context = context;
            _db = db;
            _UserManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (_context.Course != null)
            {
                Course = await _context.Course.ToListAsync();
            }
            return Page();
        }

        [BindProperty]
        public Topic Topic { get; set; } = default!;

        [BindProperty]
        public CourseTopic CourseTopic { get; set; } = default!;

        public IList<Course> Course { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Clear();
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var currentTopic = _context.Topic.FromSqlRaw("SELECT * FROM Topic")
                .OrderByDescending(b => b.TopicID)
                .FirstOrDefault();
            if(currentTopic != null)
            {
                Topic.TopicID = currentTopic.TopicID + 1; //increment last id by 1
            }
            else
            {
                Topic.TopicID = 1;
            }

            var user = await _UserManager.GetUserAsync(User);

            if (User.IsInRole("Staff") && user != null)
            {
                Topic.SupervisorID = user.Id;
            }

            _context.Topic.Add(Topic);
            await _context.SaveChangesAsync();
            CourseTopic.TopicID = Topic.TopicID;
            _db.CourseTopic.Add(CourseTopic);
            _db.SaveChanges();
            // adds course to topic

            return RedirectToPage("/Index");
        }
    }
}
