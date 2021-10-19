namespace Domain
{
    public class UserFollowing
    {
        public string ObserverId { get; set; }
        public ApplicationUser Observer { get; set; }
        public string TargetId { get; set; }
        public ApplicationUser Target { get; set; }
    }
}