using ActivitySystem.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ActivitySystem.Infrastructure.Database
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Activity>()
                .HasOne(a => a.Organizer)
                .WithMany(o => o.Activities)
                .HasForeignKey(a => a.OrganizerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Activity)
                .WithMany(a => a.Subscriptions)
                .HasForeignKey(s => s.ActivityId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Guest)
                .WithMany(g => g.Subscriptions)
                .HasForeignKey(s => s.GuestId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
