using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Project_Management_System.Data
{
    public class StaffInterest
    {
        [Required]
        public int InterestID { get; set; }
        [Required]
        public required string StaffID { get; set; }
        [Required]
        public required string Interest { get; set; }

    }
}
