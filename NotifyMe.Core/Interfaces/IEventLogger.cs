namespace NotifyMe.Core.Interfaces
{
    public interface IEventLogger
    {
        void LogEvent<TEvent>(TEvent eventData);
    }
}
