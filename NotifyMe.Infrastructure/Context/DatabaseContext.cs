using Microsoft.EntityFrameworkCore;

using NotifyMe.Core.Entities;

namespace NotifyMe.Infrastructure.Context
{
    public class DatabaseContext : DbContext
    {
        public DbSet<EventMonitoring> ChangeEvents { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserGroupUser> UserGroupUsers { get; set; }
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            Database.EnsureCreated();   
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<UserGroupUser>()
            //    .HasKey(ug => new { ug.UserGroupId, ug.UserId });

            //modelBuilder.Entity<UserGroupUser>()
            //    .HasOne<UserGroup>(ug => ug.UserGroup)
            //    .WithMany(ug => ug.Users)
            //    .HasForeignKey(ug => ug.UserGroupId);

            //modelBuilder.Entity<UserGroupUser>()
            //    .HasOne<User>(ug => ug.User)
            //    .WithMany(u => u.Groups)
            //    .HasForeignKey(ug => ug.UserId);
        }
    }
}
