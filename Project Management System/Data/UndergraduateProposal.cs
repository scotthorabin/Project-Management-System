using System.ComponentModel.DataAnnotations;

namespace Project_Management_System.Data
{
    public class UndergraduateProposal
    {
        [Required]
        public string StudentID { get; set; }

        [Required]
        public int TopicID1 { get; set; }

        [Required]
        public int TopicID2 { get; set; }

        [Required]
        public int TopicID3 { get; set; }
    }
}
