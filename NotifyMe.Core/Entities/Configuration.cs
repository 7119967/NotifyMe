using NotifyMe.Core.Enums;

namespace NotifyMe.Core.Entities
{
    public class Configuration : BaseEntity
    {
        public EventType EventType { get; set; }
        public double Threshold { get; set; }
        public virtual List<User>? Recipients { get; set; }
    }
}
