using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NotifyMe.Core.Entities;

namespace NotifyMe.Infrastructure.Context
{
    public class DatabaseContext : IdentityDbContext<User>
    {
        public override DbSet<User> Users { get; set; } = null!;
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<Change> Changes { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Configuration> Configurations { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<NotificationUser> NotificationUsers { get; set; } = null!;
 
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            // Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.ConfigureWarnings(warnings => 
            //     warnings.Ignore(CoreEventId.DetachedLazyLoadingWarning));
        }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        // }
    }
}