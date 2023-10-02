using Microsoft.EntityFrameworkCore;
using NotifyMe.Core.Entities;

namespace NotifyMe.Infrastructure.Services;

public static class Helpers
{
    public static int GetNewIdEntity<TEntity>(ICollection<TEntity> origList)
    {
        var entityType = typeof(TEntity);
        var sequence =  origList.Cast<TEntity>().ToList();
        return entityType.Name switch
        {
            "Event" => sequence.Any() ? sequence
                .AsEnumerable()
                .Max(e => Convert.ToInt32(((Event)(object)e!).Id)) + 1 : 1,            
            "Message" => sequence.Any() ? sequence
                .AsEnumerable()
                .Max(e => Convert.ToInt32(((Message)(object)e!).Id)) + 1 : 1,
            "Notification" => sequence.Any() ? sequence
                .AsEnumerable()
                .Max(e => Convert.ToInt32(((Notification)(object)e!).Id)) + 1 : 1,
            "Configuration" => sequence.Any() ? sequence
                .AsEnumerable()
                .Max(e => Convert.ToInt32(((Configuration)(object)e!).Id)) + 1 : 1,
            "NotificationUser" => sequence.Any() ? sequence
                .AsEnumerable()
                .Max(e => Convert.ToInt32(((NotificationUser)(object)e!).Id)) + 1 : 1,
            _ => 1
        };
    }
}