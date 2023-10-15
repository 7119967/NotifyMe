﻿using AutoMapper;

using Microsoft.Extensions.Hosting;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Models.User;

namespace NotifyMe.Infrastructure.Services;

public static class Helpers
{
    public static int GetNewIdEntity<TEntity>(ICollection<TEntity> origList)
    {
        var entityType = typeof(TEntity);
        var sequence =  origList.Cast<TEntity>().ToList();
        return entityType.Name switch
        {
            "Event" => sequence.Any() ? 
                sequence.AsEnumerable().Max(e => Convert.ToInt32(((Event)(object)e!).Id)) + 1 : 1,        
            "Group" => sequence.Any() ? 
                sequence.AsEnumerable().Max(e => Convert.ToInt32(((Group)(object)e!).Id)) + 1 : 1,   
            "Change" => sequence.Any() ? 
                sequence.AsEnumerable().Max(e => Convert.ToInt32(((Change)(object)e!).Id)) + 1 : 1,  
            "Message" => sequence.Any() ? 
                sequence.AsEnumerable().Max(e => Convert.ToInt32(((Message)(object)e!).Id)) + 1 : 1,
            "Notification" => sequence.Any() ? 
                sequence.AsEnumerable().Max(e => Convert.ToInt32(((Notification)(object)e!).Id)) + 1 : 1,
            "Configuration" => sequence.Any() ? 
                sequence.AsEnumerable().Max(e => Convert.ToInt32(((Configuration)(object)e!).Id)) + 1 : 1,
            "NotificationUser" => sequence.Any() ? 
                sequence.AsEnumerable().Max(e => Convert.ToInt32(((NotificationUser)(object)e!).Id)) + 1 : 1,
            _ => 1
        };
    }
    
    public static string GetPathImage(IHostEnvironment env, IMapper mapper, UploadFileService uploader, object argument)
    {
        UserAvator model;
        var path = Path.Combine(env.ContentRootPath, "wwwroot/img/avators/");
        
        if (argument is UserCreateViewModel)
            model = mapper.Map<UserCreateViewModel, UserAvator>((UserCreateViewModel)argument);
        else if (argument is UserEditViewModel)
            model = mapper.Map<UserEditViewModel, UserAvator>((UserEditViewModel)argument);     
        else
            model = mapper.Map<User, UserAvator>((User)argument);
        
        Task.Run(async () => await uploader.Upload(path, $"{model!.Email}.jpg", model.File!));
        return $"/img/avators/{model.Email}.jpg";
    }
}