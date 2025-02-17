using System.ComponentModel.DataAnnotations;

namespace Project_Management_System.Data
{
    public class Topic
    {
        [Required]
        public int TopicID { get; set; }

        [Required]
        public string TopicName { get; set; }
        [Required]

        public string TopicDescription { get; set; }

        public string? SupervisorID { get; set; }
        public string? MarkerID { get; set; }
        public string? StudentID { get; set; }
    }
}
