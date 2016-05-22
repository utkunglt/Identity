using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace WebApplication.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class BusUser : IdentityUser<long, BusLogin, BusUserRole, BusClaim>
    {
    }
    public class BusUserRole : IdentityUserRole<long> { }

    public class BusRole : IdentityRole<long, BusUserRole> { }

    public class BusClaim : IdentityUserClaim<long> { }

    public class BusLogin : IdentityUserLogin<long> { }

    public class ApplicationDbContext : IdentityDbContext<BusUser, BusRole, long, BusLogin, BusUserRole, BusClaim>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Map Entities to their tables.
            modelBuilder.Entity<BusUser>().ToTable("Users");
            modelBuilder.Entity<BusRole>().ToTable("Roles");
            modelBuilder.Entity<BusClaim>().ToTable("UserClaims");
            modelBuilder.Entity<BusLogin>().ToTable("UserLogins");
            modelBuilder.Entity<BusUserRole>().ToTable("UserRoles");
            // Set AutoIncrement-Properties
            modelBuilder.Entity<BusUser>().Property(r => r.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<BusClaim>().Property(r => r.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<BusRole>().Property(r => r.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}