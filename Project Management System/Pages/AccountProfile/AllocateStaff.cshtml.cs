using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_Management_System.Data;

// Created by Harry

namespace Project_Management_System.Pages.AccountProfile
{
    public class AllocateStaffModel : PageModel
    {
        private readonly Project_Management_System.Data.SPMS_Context _context;

        public AllocateStaffModel(Project_Management_System.Data.SPMS_Context context)
        {
            _context = context;
        }

        //Creates property

        [BindProperty]
        public Topic Topic { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id) //Reads in topic where TopicID = id passed to from previous page
        {
            if (id == null)
            {
                return NotFound();
            } 

            var topic =  await _context.Topic.FirstOrDefaultAsync(m => m.TopicID == id);
            if (topic == null)
            {
                return NotFound();
            }
            Topic = topic; // Allocate Topic
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var topicToUpdate = await _context.Topic.FindAsync(Topic.TopicID); //Find the topic that matches the ID

            if (topicToUpdate == null)
            {
                return Page();
            } //Return if no topic found

            if (Topic.SupervisorID != null) //If user has given Supervisor a value then update variable
            {
                topicToUpdate.SupervisorID = Topic.SupervisorID;
            }
            if (Topic.MarkerID != null) //If user has given Marker a value then update variable
            {
                topicToUpdate.MarkerID = Topic.MarkerID;
            }

            try //Save changes to context
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TopicExists(Topic.TopicID))
                {
                    return Page();
                }
                else
                {
                    throw;
                }
            }
            // Once complete send back to profile
            return RedirectToPage("/AccountProfile/CoordinatorProfile");
        }

        // Flag that checks if topic exists
        private bool TopicExists(int id)
        {
            return _context.Topic.Any(e => e.TopicID == id);
        }
    }
}
