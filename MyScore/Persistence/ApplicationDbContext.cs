using Microsoft.AspNet.Identity.EntityFramework;
using MyScore.Core.Domain;
using System.Data.Entity;

namespace MyScore.Persistence
{
    //public class ApplicationDbContext : DbContext 
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
            : base("name=defaultConnection")
        {

        }
        public DbSet<Course> Courses { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<User>().Property(u => u.UserName).HasMaxLength(249);
            //modelBuilder.Entity<User>().HasKey(u => u.UserName);

            modelBuilder.Entity<Course>()
                .HasRequired(c => c.User)
                .WithMany(u => u.Courses)
                .HasForeignKey(c => c.UserId);

        }
    }
}