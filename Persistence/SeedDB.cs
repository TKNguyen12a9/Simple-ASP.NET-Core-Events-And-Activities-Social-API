using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace Persistence
{
   public static class SeedDB
   {
      public static async Task Seed(AppDbContext context)
      {
         if (context.Activities.Any()) return;

         var activities = new List<Activity>
         {
            new Activity
            {
               Title = "Past Activity 1",
               Date = DateTime.Now.AddMonths(-2),
               Description = "Activity 2 months ago",
               Category = "drinks",
               City = "London",
               Venue = "Pub",
            },
            new Activity
            {
               Title = "Past Activity 2",
               Date = DateTime.Now.AddMonths(-1),
               Description = "Activity 1 month ago",
               Category = "culture",
               City = "Paris",
               Venue = "The Louvre",
            },
            new Activity
            {
               Title = "Future Activity 1",
               Date = DateTime.Now.AddMonths(1),
               Description = "Activity 1 month in future",
               Category = "music",
               City = "London",
               Venue = "Wembly Stadium",
         },
            new Activity
            {
               Title = "Future Activity 2",
               Date = DateTime.Now.AddMonths(2),
               Description = "Activity 2 months in future",
               Category = "food",
               City = "London",
               Venue = "Jamies Italian",
            }
         };

         await context.AddRangeAsync(activities);
         await context.SaveChangesAsync();
      }
   }
}