using System.ComponentModel.DataAnnotations;

namespace Project_Management_System.Data
{
    public class CourseTopic
    {
        [Required]
        public int CourseID { get; set; }
        [Required]
        public int TopicID { get; set; }

    }
}
