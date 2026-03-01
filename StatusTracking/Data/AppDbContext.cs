using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StatusTracking.Models;


namespace StatusTracking.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options) { }

        public DbSet<Issue> Issues { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Issue>()
                .HasOne(i => i.AssignedToUser)
                .WithMany(u => u.AssignedIssues)
                .HasForeignKey(i => i.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
