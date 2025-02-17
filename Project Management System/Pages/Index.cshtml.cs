using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Project_Management_System.Data;
using Project_Management_System.Migrations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_Management_System.Pages
{
    // Created by Harry

    [Authorize] // Users must be logged in to access the page.
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly SPMS_Context _context;
        private readonly SPMS_Context _db;
        private readonly UserManager<SPMS_Student> _userManager;
        private readonly SignInManager<SPMS_User> _signInManager;

        // Constructor for the IndexModel
        public IndexModel(ILogger<IndexModel> logger, SPMS_Context context, SPMS_Context db, UserManager<SPMS_Student> userManager, SignInManager<SPMS_User> signInManager)
        {
            _logger = logger;
            _context = context;
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Properties to hold topics, courses, and search query
        public IList<Topic> Topic { get; set; } = new List<Topic>();
        public IList<Topic> Topics { get; set; } = default!;
        public IList<Course> Course { get; set; } = new List<Course>();
        public IList<CourseTopic> CourseTopic { get; set; } = default!;
        public string Search { get; set; } = default!;

        public async Task OnGetAsync(string search, int? courseID)
        {
            if (_context.CourseTopic != null)
            {
                CourseTopic = await _context.CourseTopic.ToListAsync();
            }

            if (_context.Topic != null)
            {
                Topics = await _context.Topic.Where(i => string.IsNullOrEmpty(i.StudentID)).ToListAsync();
            }
            // If course is provided, filter courses by course name
            if (courseID != null)
            {
                foreach (var course in CourseTopic.Where(i => i.CourseID == courseID))
                {
                    foreach (var topic in Topics.Where(i => i.TopicID == course.TopicID))
                    {
                        Topic.Add(topic);
                    }
                }
            }
            // If search query is provided, filter topics by topic name
            else if (!string.IsNullOrEmpty(search))
            {
                Topic = Topics.Where(t => t.TopicName.Contains(search)).ToList();
            }
            // If neither course nor search query is provided, retrieve all topics
            else
            {
                Topic = Topics;
            }

            // Retrieve all courses
            Course = await _context.Course.ToListAsync();
        }

        // Handler for HTTP GET requests


        // Handler for adding a topic to the user's basket
        public async Task<IActionResult> OnPostAddAsync(int topicID)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Check if the topic is already added to the user's basket
                var topics = await _db.TopicBasket
                    .FromSqlRaw("SELECT * FROM TopicBasket WHERE TopicID = {0} AND StudentID = {1}", topicID, user.Id)
                    .ToListAsync();

                if (topics.Count == 0)
                {
                    // If the topic is not already added, add it to the basket
                    TopicBasket newTopic = new TopicBasket
                    {
                        StudentID = user.Id,
                        TopicID = topicID
                    };
                    _db.TopicBasket.Add(newTopic);
                    await _db.SaveChangesAsync();

                    TempData["ShowPopup"] = true;
                }
                else
                {
                    // If the topic is already added, show a message
                    TempData["ShowPopup"] = "Topic is already added to your proposal.";
                }
            }
            return RedirectToPage();
        }

        // Handler for HTTP POST requests to perform a search
        public async Task<IActionResult> OnPostSearchAsync()
        {
            // Call the OnGetAsync method with search query and course parameter
            await OnGetAsync(Search, Course);
            return Page();
        }

        // Private method to handle HTTP GET requests with additional parameters
        private Task OnGetAsync(string search, IList<Course> course)
        {
            throw new NotImplementedException();
        }
    }
}
