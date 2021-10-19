using System;

namespace Domain
{
    public class Comment
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public ApplicationUser AppUser { get; set; }
        public Activity Activity { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}