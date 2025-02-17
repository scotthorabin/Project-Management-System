using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Project_Management_System.Data
{
    public class StaffExpertise
    {
        [Required]
        public int ExpertiseID { get; set; }
        [Required]
        public required string StaffID { get; set; }
        [Required]
        public required string Expertise { get; set; }

    }
}
