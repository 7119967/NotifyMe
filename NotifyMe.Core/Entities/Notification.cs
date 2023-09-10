using System.Text.Json.Serialization;

namespace NotifyMe.Core.Entities
{
    public class Notification : BaseEntity
    {
        public string? Message { get; set; }
        public int CurrentThreshold { get; set; }
        public string? EventId { get; set; }
        [JsonIgnore] 
        public virtual Event? Event { get; set; }
    }
}