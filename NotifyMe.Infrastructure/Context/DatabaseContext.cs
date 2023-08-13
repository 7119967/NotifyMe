using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NotifyMe.Core.Entities;

namespace NotifyMe.Infrastructure.Context
{
    public class DatabaseContext : IdentityDbContext<User>
    {
        public DbSet<EventMonitoring> ChangeEvents { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<Configuration> Configurations { get; set; } = null!;
        public override DbSet<User> Users { get; set; } = null!;
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<GroupUser> GroupUsers { get; set; } = null!;
        
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            // Database.EnsureCreated();
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
       
        }
        
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        // }
    }
}
