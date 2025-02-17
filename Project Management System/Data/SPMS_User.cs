using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

// Sets up additional fields for users and seperates into Student and Staff using inheritance

namespace Project_Management_System.Data
{
    public class SPMS_User : IdentityUser
    {
        [PersonalData, Required, StringLength(30)]
        public string FirstName { get; set; }
        [PersonalData, Required, StringLength(60)]
        public string Surname { get; set; }
        public string Name { get { return $"{FirstName} {Surname}"; } }
      
        public byte[]? ProfilePicture{ get; set; }
    }

    public class SPMS_Student : SPMS_User
    {
        [Required,PersonalData, StringLength(7)]
        public string StudentNo { get; set; }

        [Required]
        public int CourseID { get; set; }
    }

    public class SPMS_Staff : SPMS_User
    {
    }
}
