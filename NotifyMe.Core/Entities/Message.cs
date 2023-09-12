using System.Text.Json.Serialization;

namespace NotifyMe.Core.Entities;

public class Message: BaseEntity
{
    public string Sender { get; set; }
    public virtual List<User> Receivers { get; set; }
    public string Subject { get; set; }
    public string ContentBody { get; set; }
    public string? EventId { get; set; }
    [JsonIgnore]
    public virtual Event? Event { get; set; }
}