using NotifyMe.Core.Enums;

namespace NotifyMe.Core.Entities;

public class Change: BaseEntity
{
    public ChangeType ChangeType { get; set; }
    public string? ChangeDescription { get; set; }
    // public virtual Event Event { get; set; }
    public string? EventId { get; set; }
}