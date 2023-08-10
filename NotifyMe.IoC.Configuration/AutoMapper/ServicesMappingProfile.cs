using AutoMapper;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Models;

namespace NotifyMe.IoC.Configuration.AutoMapper;

public class ServicesMappingProfile: Profile
{
    public ServicesMappingProfile()
    {
        CreateMap<User, ProfileViewModel>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            .ForMember(d => d.UserName, s => s.MapFrom(o => o.UserName))
            .ForMember(d => d.FirstName, s => s.MapFrom(o => o.FirstName))
            .ForMember(d => d.LastName, s => s.MapFrom(o => o.LastName))
            .ForMember(d => d.Email, s => s.MapFrom(o => o.Email))
            .ForMember(d => d.PhoneNumber, s => s.MapFrom(o => o.PhoneNumber))
            .ForMember(d => d.Avatar, s => s.MapFrom(o => o.Avatar))
            .ForMember(d => d.Info, s => s.MapFrom(o => o.Info));
        
        CreateMap<User, EditProfileViewModel>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            .ForMember(d => d.UserName, s => s.MapFrom(o => o.UserName))
            .ForMember(d => d.FirstName, s => s.MapFrom(o => o.FirstName))
            .ForMember(d => d.LastName, s => s.MapFrom(o => o.LastName))
            .ForMember(d => d.Email, s => s.MapFrom(o => o.Email))
            .ForMember(d => d.PhoneNumber, s => s.MapFrom(o => o.PhoneNumber))
            .ForMember(d => d.Avatar, s => s.MapFrom(o => o.Avatar))
            .ForMember(d => d.Info, s => s.MapFrom(o => o.Info));

        CreateMap<User, IndexUserViewModel>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            .ForMember(d => d.UserName, s => s.MapFrom(o => o.UserName))
            .ForMember(d => d.FirstName, s => s.MapFrom(o => o.FirstName))
            .ForMember(d => d.LastName, s => s.MapFrom(o => o.LastName))
            .ForMember(d => d.Email, s => s.MapFrom(o => o.Email))
            .ForMember(d => d.PhoneNumber, s => s.MapFrom(o => o.PhoneNumber));
       
        CreateMap<EditProfileViewModel, User>()
            .ForMember(d => d.Id, s => s.MapFrom(o => o.Id))
            .ForMember(d => d.UserName, s => s.MapFrom(o => o.UserName))
            .ForMember(d => d.FirstName, s => s.MapFrom(o => o.FirstName))
            .ForMember(d => d.LastName, s => s.MapFrom(o => o.LastName))
            .ForMember(d => d.Email, s => s.MapFrom(o => o.Email))
            .ForMember(d => d.PhoneNumber, s => s.MapFrom(o => o.PhoneNumber))
            .ForMember(d => d.Avatar, s => s.MapFrom(o => o.Avatar))
            .ForMember(d => d.Info, s => s.MapFrom(o => o.Info));
    }
}