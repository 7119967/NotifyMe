using NotifyMe.Core.Entities;

namespace NotifyMe.Infrastructure.Services;

public static class Helpers
{
    // public int? GetNewIdEntity<TEntity>(List<TEntity> origList)
    // {
    //     var entityType = typeof(TEntity);
    //
    //     var sequence = origList.Cast<TEntity>();
    //
    //     switch (entityType.Name)
    //     {
    //         case "Event":
    //             return sequence.Any() ? (TKey)(object)sequence.Max(e => Convert.ToInt32(((Event)(object)e.Id)) + 1) : 1;
    //         
    //         case "Notification":
    //             return sequence.Any() ? (TKey)(object)(sequence.Max(e => Convert.ToInt32(((Notification)(object)e).Id)) + 1);
    //         
    //         case "NotificationUser":
    //             return sequence.Any() ? (TKey)(object)(sequence.Max(e => Convert.ToInt32(((NotificationUser)(object)e).Id)) + 1) : default(TKey);
    //         
    //         case "Configuration":
    //             return sequence.Any() ? (TKey)(object)(sequence.Max(e => Convert.ToInt32(((Configuration)(object)e).Id)) + 1) : default(TKey);
    //     }
    //
    //     return default(TKey);
    // }

    // public static int? GetNewIdEntity<TEntity>(List<TEntity> origList)
    // {
    //     var entityType = typeof(TEntity);
    //     
    //     switch (entityType.Name)
    //     {
    //         case "Event":
    //             var sequence1 = origList.ConvertAll(x => (Event)(object)x);
    //             return sequence1.Any() ? sequence1.Max(e => Convert.ToInt32(e.Id)) + 1 : 1;
    //         
    //         case "Notification":
    //             var sequence2 = origList.ConvertAll(x => (Notification)(object)x);
    //             return sequence2.Any() ? sequence2.Max(e => Convert.ToInt32(e.Id)) + 1 : 1;
    //         
    //         case "NotificationUser":
    //             var sequence3 = origList.ConvertAll(x => (NotificationUser)(object)x);
    //             return sequence3.Any() ? sequence3.Max(e => Convert.ToInt32(e.Id)) + 1 : 1;
    //         
    //         case "Configuration":
    //             var sequence4 = origList.ConvertAll(x => (Configuration)(object)x);
    //             return sequence4.Any() ? sequence4.Max(e => Convert.ToInt32(e.Id)) + 1 : 1;
    //
    //     }
    //     
    //     return null;
    // }
    
    public static int GetNewIdEntity<TEntity>(List<TEntity> origList)
    {
        var entityType = typeof(TEntity);
    
        var sequence = origList.Cast<TEntity>();
    
        switch (entityType.Name)
        {
            case "Event":
                return sequence.Any() ? sequence.Max(e => Convert.ToInt32(((Event)(object)e).Id)) + 1 : 1;
            
            case "Notification":
                return sequence.Any() ? sequence.Max(e => Convert.ToInt32(((Notification)(object)e).Id)) + 1 : 1;
            
            case "NotificationUser":
                return sequence.Any() ? sequence.Max(e => Convert.ToInt32(((NotificationUser)(object)e).Id)) + 1 : 1;
            
            case "Configuration":
                return sequence.Any() ? sequence.Max(e => Convert.ToInt32(((Configuration)(object)e).Id)) + 1 : 1;
        }
    
        return 1;
    }

}