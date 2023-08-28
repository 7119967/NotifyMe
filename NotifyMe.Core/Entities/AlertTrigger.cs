namespace NotifyMe.Core.Entities;

public class AlertTrigger : BaseEntity
{
    public string? AlertName { get; set; }
    public string? EventMonitoringId { get; set; }
    public virtual EventMonitoring? EventMonitoring { get; set; }
    public string? ThresholdCondition { get; set; }
}