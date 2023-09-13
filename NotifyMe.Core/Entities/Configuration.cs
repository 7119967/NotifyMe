using System.Text.Json.Serialization;

using NotifyMe.Core.Enums;

namespace NotifyMe.Core.Entities
{
    public class Configuration : BaseEntity
    {
        public ChangeType ChangeType { get; set; }
        public PriorityType PriorityType { get; set; }
        public int Threshold { get; set; }
        public string? Message { get; set; }
        public string? GroupId { get; set; } = null;
        [JsonIgnore]
        public virtual Group? Group { get; set; }
        [JsonIgnore]
        public virtual List<Event> Events { get; set; } = new();
        //public virtual List<Group> Groups { get; set; } = new();
       
    }
}
