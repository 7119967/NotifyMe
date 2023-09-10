using NotifyMe.Core.Enums;

namespace NotifyMe.Core.Entities
{
    public class Configuration : BaseEntity
    {
        public ChangeType ChangeType { get; set; }
        public PriorityType PriorityType { get; set; }
        public int Threshold { get; set; }
        public string? Message { get; set; }
        public virtual List<Group> Groups { get; set; } = new();
        public virtual List<Event> Events { get; set; } = new();
    }
}
