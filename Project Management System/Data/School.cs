using System.ComponentModel.DataAnnotations;

namespace Project_Management_System.Data
{
    public class School
    {
        [Required]
        public int SchoolID { get; set; }

        [Required]
        public string SchoolName { get; set; }
    }
}
