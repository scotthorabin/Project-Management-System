using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project_Management_System.Data;
using Project_Management_System.Migrations;

// Created by Harry

namespace Project_Management_System.Pages
{
    [Authorize (Roles = "Student")]
    public class Submit_ProposalModel : PageModel
    {
        public UndergraduateProposal proposal = new(); //Create new instance of Proposal
        private readonly SPMS_Context _db;
        private readonly UserManager<SPMS_Student> _UserManager;
        private readonly Project_Management_System.Data.SPMS_Context _context;

        // Create variables and properties

        [BindProperty]
        public Topic Topic { get; set; } = default!;

        [BindProperty]
        public CourseTopic CourseTopic { get; set; } = default!;
        public IList<TopicBasket> TopicBasket { get; set; } = default!;
        public IList<Course> Course { get; set; } = default!;
        public IList<Topic> Topics { get; private set; } = default!;
        public int Priority { get; set; }
        public IList<UndergraduateProposal> UndergraduateProposals { get; set; } = default!;
        public IList<PostgraduateProposal> PostgraduateProposals { get; set; } = default!;
        public bool Postgraduate { get; set; }

        public bool Submitted { get; set; }
        public int Topic1 { get; set; }
        public int Topic2 { get; set; }
        public int Topic3 { get; set; }

        public Submit_ProposalModel(Project_Management_System.Data.SPMS_Context context, SPMS_Context db, UserManager<SPMS_Student> userManager)
        {
            _context = context;
            _db = db;
            _UserManager = userManager;
        }
        public async Task OnGetAsync()
        {
            var user = await _UserManager.GetUserAsync(User); // Load in details from user

            if (user != null)
            {
                TopicBasket = _db.TopicBasket //select data from database
                .FromSqlRaw("SELECT * FROM TopicBasket WHERE StudentID = {0}", user.Id)
                .ToList();

                Course = _db.Course //select data from database
                .FromSqlRaw("SELECT * FROM Course WHERE CourseID = {0}", user.CourseID)
                .ToList();
                
                //Only select data that matches data contained in user

                foreach(var course in Course)
                {
                    if (course.Postgraduate)
                    {
                        Postgraduate = true;
                    }
                }
            }
            // Checks if students course is postgraduate

            if (_context.Topic != null)
            {
                Topics = await _context.Topic.ToListAsync();
            } // Loads topics

            if (Postgraduate && user != null)
            {
                PostgraduateProposals = _db.PostgraduateProposal //select data from database
              .FromSqlRaw("SELECT * FROM PostgraduateProposal WHERE StudentID = {0}", user.Id)
              .ToList(); // Loads in proposal

                if (PostgraduateProposals.Count() > 0)
                {
                    Submitted = true;
                } //If submissions submitted is true
            }
            else if(user != null)
            {
                UndergraduateProposals = _db.UndergraduateProposal //select data from database
              .FromSqlRaw("SELECT * FROM UndergraduateProposal WHERE StudentID = {0}", user.Id)
              .ToList(); // Loads in proposals for specific student

                if (UndergraduateProposals.Count() > 0)
                {
                    Submitted = true;
                } // If there are submissions then mark as submitted
            }
        }
        public async Task<IActionResult> OnPostPostgradSubmitAsync()
        { // Submit postgraduate module choices

            var user = await _UserManager.GetUserAsync(User);
            if (user == null)
            {
                await OnGetAsync();
                return Page();
            }
            TopicBasket = _db.TopicBasket //select data from database for students submissions
                .FromSqlRaw("SELECT * FROM TopicBasket WHERE StudentID = {0}", user.Id)
                .ToList();

            if(TopicBasket.Count() == 1)
            { // Checks only one topic and then adds to database
                PostgraduateProposal newPostTopic = new PostgraduateProposal
                {
                    StudentID = user.Id,
                    TopicID = TopicBasket[0].TopicID,
                };
                _db.PostgraduateProposal.Add(newPostTopic);
                await _db.SaveChangesAsync(); //save all changes to sql database
                return RedirectToPage("/Index"); //return to home page

            }
            return RedirectToPage(); // refreshs page if error
        }

        public async Task<IActionResult> OnPostAsync()
        { // Submits create own topic
            if(Topic.TopicDescription == null || Topic.TopicName == null)
            {
                await OnGetAsync();
                return Page(); // Checks that a topic has been entered
            }
            var currentTopic = _context.Topic.FromSqlRaw("SELECT * FROM Topic")
                .OrderByDescending(b => b.TopicID)
                .FirstOrDefault();
            if (currentTopic != null) 
            {
                Topic.TopicID = currentTopic.TopicID + 1; //increment last id by 1
            }
            else
            {
                Topic.TopicID = 1;
            }
            var user = await _UserManager.GetUserAsync(User);

            if(user == null)
            {
                return Page();
            }
            Topic.StudentID = user.Id; // Assigns student to topic

            _context.Topic.Add(Topic);
            await _context.SaveChangesAsync();
            CourseTopic.CourseID = user.CourseID;
            CourseTopic.TopicID = Topic.TopicID;
            _db.CourseTopic.Add(CourseTopic);
            _db.SaveChanges();
            // adds course to topic
            TopicBasket newTopic = new TopicBasket
            {
                StudentID = user.Id,
                TopicID = Topic.TopicID
            };
            _db.TopicBasket.Add(newTopic);
            await _db.SaveChangesAsync();
            // Save topic to database
            return RedirectToPage("/Submit_Proposal");
        }

        public async Task<IActionResult> OnPostUndergradSubmitAsync()
        {
            // Submits undergraduate proposals
            var user = await _UserManager.GetUserAsync(User);
            if(user == null)
            {
                await OnGetAsync();
                return Page();
            }
            TopicBasket = _db.TopicBasket //select data from database
                .FromSqlRaw("SELECT * FROM TopicBasket WHERE StudentID = {0}", user.Id)
                .ToList();

            if (TopicBasket.Count() == 3)
            {
                UndergraduateProposal newTopic = new UndergraduateProposal
                { // Stores new proposal
                    StudentID = user.Id,
                    TopicID1 = TopicBasket[0].TopicID,
                    TopicID2 = TopicBasket[1].TopicID,
                    TopicID3 = TopicBasket[2].TopicID,
                };
                _db.UndergraduateProposal.Add(newTopic);
                await _db.SaveChangesAsync(); //save all changes to sql database
                return RedirectToPage("/Index"); //return to home page
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int topicID) //takes id passed from button
        {
            var user = await _UserManager.GetUserAsync(User);
            if(user != null)
            {
                var topic = await _db.TopicBasket.FindAsync(user.Id, topicID);
                if (topic != null)
                {
                    _db.TopicBasket.Remove(topic);
                    await _db.SaveChangesAsync();
                }
            }
            //save changes
            return RedirectToPage(); //return to page
        }
    }
}
