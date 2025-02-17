using Microsoft.AspNetCore.Identity;
using Project_Management_System.Data;

namespace Project_Management_System.Data
{
    public class SPMS_SeedUsers
    {
        public static async Task Initialize(SPMS_Context context,
            UserManager<SPMS_User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            string staffRole = "Staff";
            string studentRole = "Student";
            string coordinatorRole = "Co-ordinator";
            string password4all = "P@55word";

            if (await roleManager.FindByNameAsync(staffRole) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(staffRole));
            }

            if (await roleManager.FindByNameAsync(studentRole) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(studentRole));
            }

            if (await roleManager.FindByNameAsync(coordinatorRole) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(coordinatorRole));
            }

            if (await userManager.FindByNameAsync("student@chester.ac.uk") == null)
            {
                var user = new SPMS_Student
                {
                    UserName = "student@chester.ac.uk",
                    Email = "student@chester.ac.uk",
                    FirstName = "Student",
                    Surname = "User",
                    StudentNo = "1234567"
                };
                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password4all);
                    await userManager.AddToRoleAsync(user, studentRole);
                }
            }
            if (await userManager.FindByNameAsync("staff@chester.ac.uk") == null)
            {
                var user = new SPMS_Staff
                {
                    UserName = "staff@chester.ac.uk",
                    Email = "staff@chester.ac.uk",
                    FirstName = "Staff",
                    Surname = "User",
                };
                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password4all);
                    await userManager.AddToRoleAsync(user, staffRole);
                }
            }
            if (await userManager.FindByNameAsync("projectcoordinator@chester.ac.uk") == null)
            {
                var user = new SPMS_Staff
                {
                    UserName = "projectcoordinator@chester.ac.uk",
                    Email = "projectcoordinator@chester.ac.uk",
                    FirstName = "Project",
                    Surname = "Co-ordinator",
                };
                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password4all);
                    await userManager.AddToRoleAsync(user, coordinatorRole);
                }
            }

        }
    }
}
