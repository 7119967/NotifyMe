using AutoMapper;

using Microsoft.Extensions.Hosting;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Models.User;

namespace NotifyMe.Infrastructure.Services;

public static class Helpers
{
    public static int GetNewIdEntity<TEntity>(ICollection<TEntity> origList)
    {
        var sequence =  origList.Cast<TEntity>().ToList();
        return typeof(TEntity).Name switch
        {
            nameof(Event) => sequence.Any() ? 
                sequence.AsEnumerable().Max(e => Convert.ToInt32(((Event)(object)e!).Id)) + 1 : 1,
            nameof(Group) => sequence.Any() ? 
                sequence.AsEnumerable().Max(e => Convert.ToInt32(((Group)(object)e!).Id)) + 1 : 1,
            nameof(Change) => sequence.Any() ? 
                sequence.AsEnumerable().Max(e => Convert.ToInt32(((Change)(object)e!).Id)) + 1 : 1,
            nameof(Message) => sequence.Any() ? 
                sequence.AsEnumerable().Max(e => Convert.ToInt32(((Message)(object)e!).Id)) + 1 : 1,
            nameof(Notification) => sequence.Any() ? 
                sequence.AsEnumerable().Max(e => Convert.ToInt32(((Notification)(object)e!).Id)) + 1 : 1,
            nameof(Configuration) => sequence.Any() ? 
                sequence.AsEnumerable().Max(e => Convert.ToInt32(((Configuration)(object)e!).Id)) + 1 : 1,
            nameof(NotificationUser) => sequence.Any() ? 
                sequence.AsEnumerable().Max(e => Convert.ToInt32(((NotificationUser)(object)e!).Id)) + 1 : 1,
            _ => 1
        };
    }
    
    public static string GetPathImage(IHostEnvironment env, IMapper mapper, UploadFileService uploader, object entity)
    {
        UserAvator model;
        var path = Path.Combine(env.ContentRootPath, "wwwroot/img/avators/");
        
        if (entity is UserCreateViewModel)
            model = mapper.Map<UserCreateViewModel, UserAvator>((UserCreateViewModel)entity);
        else if (entity is UserEditViewModel)
            model = mapper.Map<UserEditViewModel, UserAvator>((UserEditViewModel)entity);     
        else
            model = mapper.Map<User, UserAvator>((User)entity);
        
        Task.Run(async () => await uploader.Upload(path, $"{model!.Email}.jpg", model.File!));
        return $"/img/avators/{model.Email}.jpg";
    }
}