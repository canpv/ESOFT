using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Esso.Models;
using System.Collections;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations;
using System;

namespace Esso.Web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string UPDATE_USER { get; set; }
        [Required]
        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string CREATE_USER { get; set; }
        public bool? PUSH_NOT { get; set; }
        //[StringLength(15, ErrorMessage = "Maximum length is {1}")]
        //public string PASSWORD { get; set; }
        [Required]
        public bool IS_DEMO { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public Int16? REPORT_SEND_MAIL { get; set; }
        public Int16? ALARM_SEND_MAIL { get; set; }
        public Int16? SHOW_MONEY { get; set; }
        [Required]
        public bool IS_DELETED { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public ApplicationUser()
        {
            IS_DELETED = false;
            //SHOW_MONEY = null;

        }
    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            //Database.SetInitializer<IdentityDbContext>(null);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("LOG724DB");
        }
    }
}