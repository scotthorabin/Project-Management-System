using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Project_Management_System.Data
{
    public class SPMS_Context : IdentityDbContext<SPMS_User>
    {
        public SPMS_Context(DbContextOptions<SPMS_Context> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SPMS_User>().Ignore(e => e.Name);
            modelBuilder.Entity<SPMS_Staff>().ToTable("Staff");
            modelBuilder.Entity<SPMS_Student>().ToTable("Student");
            modelBuilder.Entity<StaffDivision>().HasKey(t => new {t.StaffID,t.DivisionID});
            modelBuilder.Entity<CourseTopic>().HasKey(t => new { t.CourseID, t.TopicID });
            modelBuilder.Entity<TopicBasket>().HasKey(t => new { t.StudentID, t.TopicID });
            modelBuilder.Entity<PostgraduateProposal>().HasKey(t => new { t.StudentID});
            modelBuilder.Entity<UndergraduateProposal>().HasKey(t => new { t.StudentID });
            modelBuilder.Entity<StaffInterest>().HasKey(t => new { t.InterestID });
            modelBuilder.Entity<StaffExpertise>().HasKey(t => new { t.ExpertiseID });
        } // Set primary keys for each table and create a table for staff and student users

        public DbSet<SPMS_User> SPMS_Account { get; set; }
        public DbSet<SPMS_Student> Student { get; set; }
        public DbSet<SPMS_Staff> Staff { get; set; }
        public DbSet<School> School { get; set; }
        public DbSet<Division> Division { get; set; }
        public DbSet<StaffDivision> StaffDivision { get; set; } = default!;
        public DbSet<Topic> Topic { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<StaffInterest> StaffInterest { get; set; }
        public DbSet<StaffExpertise> StaffExpertise { get; set; }
        public DbSet<CourseTopic> CourseTopic { get; set; } = default!;
        public DbSet<UndergraduateProposal> UndergraduateProposal { get; set; }
        public DbSet<PostgraduateProposal> PostgraduateProposal { get; set; }
        public DbSet<TopicBasket> TopicBasket { get; set; } = default!;
    } // Connect database tables with data classes
}