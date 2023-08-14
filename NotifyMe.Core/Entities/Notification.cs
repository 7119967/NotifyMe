namespace NotifyMe.Core.Entities
{
    public class Notification : BaseEntity
    {
        public string? Recipient { get; set; }
        public string? Message { get; set; }
        public string? ChangedElements { get; set; }
    }
}
