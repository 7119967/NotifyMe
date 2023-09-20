using System.Text.Json.Serialization;

namespace NotifyMe.Core.Entities
{
    public class Notification : BaseEntity
    {
        public string? Message { get; set; }
        public string? EventId { get; set; }
        [JsonIgnore] 
        public virtual Event? Event { get; set; }
        public virtual ICollection<NotificationUser>? NotificationUsers { get; set; } =
            new List<NotificationUser>();
    }
}