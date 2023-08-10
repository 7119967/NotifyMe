using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using NotifyMe.Core.Entities;

namespace NotifyMe.Infrastructure.Context
{
    public class DatabaseContext : IdentityDbContext<User>
    {
        public DbSet<EventMonitoring> ChangeEvents { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<IdentityUserLogin<string>>(entity =>
            //{
            //    entity.HasKey(l => new { l.LoginProvider, l.ProviderKey });
            //});

            //builder.Entity("NotifyMe.Core.Entities.UserGroupUser", b =>
            //{
            //    b.HasOne("NotifyMe.Core.Entities.UserGroup", "UserGroup")
            //        .WithMany("Users")
            //        .HasForeignKey("UserGroupId");
            //    b.HasOne("NotifyMe.Core.Entities.User", "User")
            //        .WithMany("Groups")
            //        .HasForeignKey("UserId");
            //    b.Navigation("User");
            //    b.Navigation("UserGroup");
            //});


            //builder.Entity<UserGroup>()
            //   .HasMany(group => group.Users) // Specify the navigation property
            //   .WithMany(user => user.Groups) // Specify the navigation property in User entity
            //   .UsingEntity<UserGroupUser>(
            //       j => j.HasOne(userGroupUser => userGroupUser.User)
            //           .WithMany()
            //           .HasForeignKey(userGroupUser => userGroupUser.UserId),
            //       j => j.HasOne(userGroupUser => userGroupUser.UserGroup)
            //           .WithMany()
            //           .HasForeignKey(userGroupUser => userGroupUser.UserGroupId)
            //   );
        }
    }
}
