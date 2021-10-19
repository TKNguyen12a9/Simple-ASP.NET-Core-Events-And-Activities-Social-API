using System;
using System.Collections.Generic;

namespace Domain
{
    public class ActivityAttendee
    {
        public string AppUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Activity Activity { get; set; }
        public Guid ActivityId { get; set; }
        public bool IsHost { get; set; }
        // public ICollection<UserFollowing> Followings { get; set; }
        // public ICollection<UserFollowing> Followers { get; set; }
        // public ActivityAttendee()
        // {
        //     Followings = new List<UserFollowing>();
        //     Followers = new List<UserFollowing>();
        // }
    }
}