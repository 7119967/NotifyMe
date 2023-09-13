using System.Text.Json.Serialization;
using NotifyMe.Core.Enums;

namespace NotifyMe.Core.Entities
{
    public class Event : BaseEntity
    {
        public string? EventName { get; set; }
        public string? EventDescription { get; set; }
        public int CurrentThreshold { get; set; }
        public string? ConfigurationId { get; set; }
        [JsonIgnore]
        public virtual Configuration? Configuration { get; set; }
        
        [JsonIgnore]
        public virtual List<Change>? Changes { get; set; }= new();
        [JsonIgnore]
        public virtual List<Message>? Messages { get; set; }= new();
        [JsonIgnore]
        public virtual List<Notification>? Notifications { get; set; }= new();
    }
}
