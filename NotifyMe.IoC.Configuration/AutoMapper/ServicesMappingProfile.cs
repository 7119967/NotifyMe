using System.Net.Mail;
using AutoMapper;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Models.User;
using NotifyMe.Core.Models.Group;
using NotifyMe.Core.Models.Notification;

namespace NotifyMe.IoC.Configuration.AutoMapper;

public class ServicesMappingProfile : Profile
{
    public ServicesMappingProfile()
    {
    
        // CreateMap<User, ProfileViewModel>()
        //     .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
        //     .ForMember(d => d.UserName, s => s.MapFrom(o => o.UserName))
        //     .ForMember(d => d.FirstName, s => s.MapFrom(o => o.FirstName))
        //     .ForMember(d => d.LastName, s => s.MapFrom(o => o.LastName))
        //     .ForMember(d => d.Email, s => s.MapFrom(o => o.Email))
        //     .ForMember(d => d.PhoneNumber, s => s.MapFrom(o => o.PhoneNumber))
        //     .ForMember(d => d.Avatar, s => s.MapFrom(o => o.Avatar))
        //     .ForMember(d => d.Info, s => s.MapFrom(o => o.Info));

        CreateMap<User, UserEditViewModel>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            .ForMember(d => d.UserName, s => s.MapFrom(o => o.UserName))
            .ForMember(d => d.FirstName, s => s.MapFrom(o => o.FirstName))
            .ForMember(d => d.LastName, s => s.MapFrom(o => o.LastName))
            .ForMember(d => d.Email, s => s.MapFrom(o => o.Email))
            .ForMember(d => d.PhoneNumber, s => s.MapFrom(o => o.PhoneNumber))
            .ForMember(d => d.Avatar, s => s.MapFrom(o => o.Avatar))
            .ForMember(d => d.Info, s => s.MapFrom(o => o.Info))
            .ForMember(d => d.ConcurrencyStamp, s => s.MapFrom(o => o.ConcurrencyStamp))
            .ForMember(d => d.NormalizedUserName, s => s.MapFrom(o => o.NormalizedUserName))
            .ForMember(d => d.NormalizedEmail, s => s.MapFrom(o => o.NormalizedEmail))
            .ForMember(d => d.PasswordHash, s => s.MapFrom(o => o.PasswordHash))
            .ForMember(d => d.SecurityStamp, s => s.MapFrom(o => o.SecurityStamp))
            .ForMember(d => d.LockoutEnabled, s => s.MapFrom(o => o.LockoutEnabled))
            .ForMember(d => d.GroupId, s => s.MapFrom(o => o.GroupId))
            ;

        CreateMap<User, UserListViewModel>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            .ForMember(d => d.UserName, s => s.MapFrom(o => o.UserName))
            .ForMember(d => d.FirstName, s => s.MapFrom(o => o.FirstName))
            .ForMember(d => d.LastName, s => s.MapFrom(o => o.LastName))
            .ForMember(d => d.Email, s => s.MapFrom(o => o.Email))
            .ForMember(d => d.PhoneNumber, s => s.MapFrom(o => o.PhoneNumber))
            // .ForMember(d => d.Group, s => s.MapFrom(o => o.Group))
            ;

        CreateMap<UserEditViewModel, User>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            .ForMember(d => d.UserName, s => s.MapFrom(o => o.UserName))
            .ForMember(d => d.FirstName, s => s.MapFrom(o => o.FirstName))
            .ForMember(d => d.LastName, s => s.MapFrom(o => o.LastName))
            .ForMember(d => d.Email, s => s.MapFrom(o => o.Email))
            .ForMember(d => d.PhoneNumber, s => s.MapFrom(o => o.PhoneNumber))
            .ForMember(d => d.Avatar, s => s.MapFrom(o => o.Avatar))
            .ForMember(d => d.Info, s => s.MapFrom(o => o.Info))
            .ForMember(d => d.ConcurrencyStamp, s => s.MapFrom(o => o.ConcurrencyStamp))
            .ForMember(d => d.NormalizedUserName, s => s.MapFrom(o => o.NormalizedUserName))
            .ForMember(d => d.NormalizedEmail, s => s.MapFrom(o => o.NormalizedEmail))
            .ForMember(d => d.PasswordHash, s => s.MapFrom(o => o.PasswordHash))
            .ForMember(d => d.SecurityStamp, s => s.MapFrom(o => o.SecurityStamp))
            .ForMember(d => d.LockoutEnabled, s => s.MapFrom(o => o.LockoutEnabled))
            .ForMember(d => d.GroupId, s => s.MapFrom(o => o.GroupId))
            ;
        
        CreateMap<UserCreateViewModel, User>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            .ForMember(d => d.UserName, s => s.MapFrom(o => o.UserName))
            .ForMember(d => d.FirstName, s => s.MapFrom(o => o.FirstName))
            .ForMember(d => d.LastName, s => s.MapFrom(o => o.LastName))
            .ForMember(d => d.Email, s => s.MapFrom(o => o.Email))
            .ForMember(d => d.PhoneNumber, s => s.MapFrom(o => o.PhoneNumber))
            .ForMember(d => d.Avatar, s => s.MapFrom(o => o.Avatar))
            .ForMember(d => d.Info, s => s.MapFrom(o => o.Info))
            .ForMember(d => d.ConcurrencyStamp, s => s.MapFrom(o => o.ConcurrencyStamp))
            .ForMember(d => d.NormalizedUserName, s => s.MapFrom(o => o.NormalizedUserName))
            .ForMember(d => d.NormalizedEmail, s => s.MapFrom(o => o.NormalizedEmail))
            .ForMember(d => d.PasswordHash, s => s.MapFrom(o => o.PasswordHash))
            .ForMember(d => d.SecurityStamp, s => s.MapFrom(o => o.SecurityStamp))
            .ForMember(d => d.LockoutEnabled, s => s.MapFrom(o => o.LockoutEnabled))
            .ForMember(d => d.GroupId, s => s.MapFrom(o => o.GroupId))
            ;
        
        CreateMap<UserEditViewModel, User>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            .ForMember(d => d.UserName, s => s.MapFrom(o => o.UserName))
            .ForMember(d => d.FirstName, s => s.MapFrom(o => o.FirstName))
            .ForMember(d => d.LastName, s => s.MapFrom(o => o.LastName))
            .ForMember(d => d.Email, s => s.MapFrom(o => o.Email))
            .ForMember(d => d.PhoneNumber, s => s.MapFrom(o => o.PhoneNumber))
            .ForMember(d => d.Avatar, s => s.MapFrom(o => o.Avatar))
            .ForMember(d => d.Info, s => s.MapFrom(o => o.Info))
            .ForMember(d => d.ConcurrencyStamp, s => s.MapFrom(o => o.ConcurrencyStamp))
            .ForMember(d => d.NormalizedUserName, s => s.MapFrom(o => o.NormalizedUserName))
            .ForMember(d => d.NormalizedEmail, s => s.MapFrom(o => o.NormalizedEmail))
            .ForMember(d => d.PasswordHash, s => s.MapFrom(o => o.PasswordHash))
            .ForMember(d => d.SecurityStamp, s => s.MapFrom(o => o.SecurityStamp))
            .ForMember(d => d.LockoutEnabled, s => s.MapFrom(o => o.LockoutEnabled))
            .ForMember(d => d.GroupId, s => s.MapFrom(o => o.GroupId))
            ;  
        
        CreateMap<UserDetailsViewModel, User>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            .ForMember(d => d.UserName, s => s.MapFrom(o => o.UserName))
            .ForMember(d => d.FirstName, s => s.MapFrom(o => o.FirstName))
            .ForMember(d => d.LastName, s => s.MapFrom(o => o.LastName))
            .ForMember(d => d.Email, s => s.MapFrom(o => o.Email))
            .ForMember(d => d.PhoneNumber, s => s.MapFrom(o => o.PhoneNumber))
            .ForMember(d => d.Avatar, s => s.MapFrom(o => o.Avatar))
            .ForMember(d => d.Info, s => s.MapFrom(o => o.Info))
            .ForMember(d => d.ConcurrencyStamp, s => s.MapFrom(o => o.ConcurrencyStamp))
            .ForMember(d => d.NormalizedUserName, s => s.MapFrom(o => o.NormalizedUserName))
            .ForMember(d => d.NormalizedEmail, s => s.MapFrom(o => o.NormalizedEmail))
            .ForMember(d => d.PasswordHash, s => s.MapFrom(o => o.PasswordHash))
            .ForMember(d => d.SecurityStamp, s => s.MapFrom(o => o.SecurityStamp))
            .ForMember(d => d.LockoutEnabled, s => s.MapFrom(o => o.LockoutEnabled))
            .ForMember(d => d.GroupId, s => s.MapFrom(o => o.GroupId))
            ;
        
        CreateMap<User, UserDetailsViewModel>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            .ForMember(d => d.UserName, s => s.MapFrom(o => o.UserName))
            .ForMember(d => d.FirstName, s => s.MapFrom(o => o.FirstName))
            .ForMember(d => d.LastName, s => s.MapFrom(o => o.LastName))
            .ForMember(d => d.Email, s => s.MapFrom(o => o.Email))
            .ForMember(d => d.PhoneNumber, s => s.MapFrom(o => o.PhoneNumber))
            .ForMember(d => d.Avatar, s => s.MapFrom(o => o.Avatar))
            .ForMember(d => d.Info, s => s.MapFrom(o => o.Info))
            .ForMember(d => d.ConcurrencyStamp, s => s.MapFrom(o => o.ConcurrencyStamp))
            .ForMember(d => d.NormalizedUserName, s => s.MapFrom(o => o.NormalizedUserName))
            .ForMember(d => d.NormalizedEmail, s => s.MapFrom(o => o.NormalizedEmail))
            .ForMember(d => d.PasswordHash, s => s.MapFrom(o => o.PasswordHash))
            .ForMember(d => d.SecurityStamp, s => s.MapFrom(o => o.SecurityStamp))
            .ForMember(d => d.LockoutEnabled, s => s.MapFrom(o => o.LockoutEnabled))
            .ForMember(d => d.GroupId, s => s.MapFrom(o => o.GroupId))
            ;

        CreateMap<UserListViewModel, UserDetailsViewModel>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            .ForMember(d => d.UserName, s => s.MapFrom(o => o.UserName))
            .ForMember(d => d.FirstName, s => s.MapFrom(o => o.FirstName))
            .ForMember(d => d.LastName, s => s.MapFrom(o => o.LastName))
            .ForMember(d => d.Email, s => s.MapFrom(o => o.Email))
            .ForMember(d => d.PhoneNumber, s => s.MapFrom(o => o.PhoneNumber))
            .ForMember(d => d.GroupId, s => s.MapFrom(o => o.GroupId))
            ;
        
        CreateMap<User, UserDeleteViewModel>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id));

        CreateMap<GroupCreateViewModel, Group>()
            .ForMember(d => d.Name, s => s.MapFrom(o => o.Name))
            .ForMember(d => d.Description, s => s.MapFrom(o => o.Description))
            .ForMember(d => d.PriorityType, s => s.MapFrom(o => o.PriorityType))
            ;  
        
        // CreateMap<NotificationCreateViewModel, Notification>()
        //     .ForMember(d => d.Receivers, s => s.MapFrom(o => o.Recipient))
        //     .ForMember(d => d.Message, s => s.MapFrom(o => o.Message))
        //     // .ForMember(d => d., s => s.MapFrom(o => o.ChangedElements))
        //     ;
        
        CreateMap<Group, GroupDetailsViewModel>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            .ForMember(d => d.Name, s => s.MapFrom(o => o.Name))
            .ForMember(d => d.Description, s => s.MapFrom(o => o.Description));        
        
        CreateMap<Notification, NotificationDetailsViewModel>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            // .ForMember(d => d.Recipient, s => s.MapFrom(o => o.Recipient))
            .ForMember(d => d.Message, s => s.MapFrom(o => o.Message))
            // .ForMember(d => d.ChangedElements, s => s.MapFrom(o => o.ChangedElements))
            ;
        
        CreateMap<Group, GroupEditViewModel>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            .ForMember(d => d.Name, s => s.MapFrom(o => o.Name))
            .ForMember(d => d.Description, s => s.MapFrom(o => o.Description));  
        
        CreateMap<Notification, NotificationEditViewModel>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            // .ForMember(d => d.Recipient, s => s.MapFrom(o => o.Recipient))
            .ForMember(d => d.Message, s => s.MapFrom(o => o.Message))
            // .ForMember(d => d.ChangedElements, s => s.MapFrom(o => o.ChangedElements))
            ;
        
        CreateMap<GroupEditViewModel, Group>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            .ForMember(d => d.Name, s => s.MapFrom(o => o.Name))
            .ForMember(d => d.Description, s => s.MapFrom(o => o.Description));        
        
        CreateMap<NotificationEditViewModel, Notification>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            // .ForMember(d => d.Recipient, s => s.MapFrom(o => o.Recipient))
            .ForMember(d => d.Message, s => s.MapFrom(o => o.Message))
            // .ForMember(d => d.ChangedElements, s => s.MapFrom(o => o.ChangedElements))
            ;

        CreateMap<Group, GroupDeleteViewModel>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id));        
        
        CreateMap<Notification, NotificationDeleteViewModel>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id));
        
        CreateMap<Group, GroupListViewModel>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            .ForMember(d => d.Name, s => s.MapFrom(o => o.Name))
            .ForMember(d => d.Description, s => s.MapFrom(o => o.Description))
            .ForMember(d => d.PriorityType, s => s.MapFrom(o => o.PriorityType))
            ; 
        
        CreateMap<Notification, NotificationListViewModel>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            // .ForMember(d => d.Recipient, s => s.MapFrom(o => o.Recipient))
            .ForMember(d => d.Message, s => s.MapFrom(o => o.Message))
            // .ForMember(d => d.ChangedElements, s => s.MapFrom(o => o.ChangedElements))
            ;     
        
        CreateMap<MailMessage, Message>()
            .ForMember(d => d.Sender, s => s.MapFrom(o => o.From))
            .ForMember(d => d.Receivers, s => s.MapFrom(o => o.To))
            .ForMember(d => d.Subject, s => s.MapFrom(o => o.Subject))
            .ForMember(d => d.ContentBody, s => s.MapFrom(o => o.Body))
            ;
    }
}