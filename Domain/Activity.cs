using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Activity
    {
        public Guid Id { get; set; }
        // [Required] //  field is required means not allowing null, but your database do allowed null!
        // so when do query, you will possibly got the error: https://forums.asp.net/t/2167761.aspx?An+exception+occurred+while+iterating+over+the+results+of+a+query+for+context+type+Invalid+operation+The+connection+is+closed+
        [Required]
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }
        public bool IsCancelled { get; set; }
        public ICollection<ActivityAttendee> Attendees { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public Activity()
        {
            Attendees = new List<ActivityAttendee>();
            Comments = new List<Comment>();
        }
    }
}