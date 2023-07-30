namespace NotifyMe.Core.Entities
{
    public interface IHasIdentity
    {
        long Id { get; set; }
    }
    public interface IDatesCreateUpdate
    {
        public DateTime Timestamp { get; set; }
    }
    public abstract class BaseEntity : IHasIdentity, IDatesCreateUpdate
    {
        public long Id { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"{GetType()}: {Id}";
        }
    }
}
