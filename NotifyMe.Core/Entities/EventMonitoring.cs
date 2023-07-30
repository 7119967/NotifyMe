namespace NotifyMe.Core.Entities
{
    public class EventMonitoring: BaseEntity
    {
        public string EventName {get;set;}
        public string EventDescription {get;set;}
        public virtual List<AlertTrigger>? AlertTrigger { get; set; }
    }
}
