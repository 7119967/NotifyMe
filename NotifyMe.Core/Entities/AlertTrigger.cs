namespace NotifyMe.Core.Entities;

public class AlertTrigger: BaseEntity
{
    public string? AlertName {get;set;}
    public int EventMonitoringId {get;set;}
    public virtual EventMonitoring? EventMonitoring {get;set;}
    public string? ThresholdCondition {get;set;}
}