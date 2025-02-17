using System.ComponentModel.DataAnnotations;

namespace Project_Management_System.Data

{
    public class StaffDivision
    {
        [Required]
        public string StaffID { get; set; }

        [Required]
        public int DivisionID { get; set; }

        
    }
}
