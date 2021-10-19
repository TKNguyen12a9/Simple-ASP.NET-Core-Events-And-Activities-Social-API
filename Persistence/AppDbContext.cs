using System;
using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;


namespace Persistence
{
    // NOTE: using dotnet restore when can not import package 
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityAttendee> ActivityAttendees { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserFollowing> UserFollowings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // config for N-N relationship
            builder.Entity<ActivityAttendee>(x => x.HasKey(keys => new { keys.ActivityId, keys.AppUserId }));

            builder.Entity<ActivityAttendee>()
                .HasOne(i => i.ApplicationUser)
                .WithMany(n => n.Activities)
                .HasForeignKey(fk => fk.AppUserId);

            builder.Entity<ActivityAttendee>()
                .HasOne(i => i.Activity)
                .WithMany(n => n.Attendees)
                .HasForeignKey(fk => fk.ActivityId);

            // config for comment
            builder.Entity<Comment>()
                .HasOne(a => a.Activity)
                .WithMany(c => c.Comments)
                .OnDelete(DeleteBehavior.Cascade); // if activity is deleted, then all comments included in that activity will be delete as well

            builder.Entity<UserFollowing>(e =>
            {
                e.HasKey(k => new { k.ObserverId, k.TargetId });

                e.HasOne(x => x.Observer)
                    .WithMany(x => x.Followings)
                    .HasForeignKey(x => x.ObserverId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Target)
                    .WithMany(x => x.Followers)
                    .HasForeignKey(x => x.TargetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}