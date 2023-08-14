using AutoMapper;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Models;

namespace NotifyMe.IoC.Configuration.AutoMapper;

public class ServicesMappingProfile : Profile
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
            .ForMember(d => d.Info, s => s.MapFrom(o => o.Info))
            .ForMember(d => d.ConcurrencyStamp, s => s.MapFrom(o => o.ConcurrencyStamp))
            .ForMember(d => d.NormalizedUserName, s => s.MapFrom(o => o.NormalizedUserName))
            .ForMember(d => d.NormalizedEmail, s => s.MapFrom(o => o.NormalizedEmail))
            .ForMember(d => d.PasswordHash, s => s.MapFrom(o => o.PasswordHash))
            .ForMember(d => d.SecurityStamp, s => s.MapFrom(o => o.SecurityStamp))
            .ForMember(d => d.LockoutEnabled, s => s.MapFrom(o => o.LockoutEnabled));

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
            .ForMember(d => d.Info, s => s.MapFrom(o => o.Info))
            .ForMember(d => d.ConcurrencyStamp, s => s.MapFrom(o => o.ConcurrencyStamp))
            .ForMember(d => d.NormalizedUserName, s => s.MapFrom(o => o.NormalizedUserName))
            .ForMember(d => d.NormalizedEmail, s => s.MapFrom(o => o.NormalizedEmail))
            .ForMember(d => d.PasswordHash, s => s.MapFrom(o => o.PasswordHash))
            .ForMember(d => d.SecurityStamp, s => s.MapFrom(o => o.SecurityStamp))
            .ForMember(d => d.LockoutEnabled, s => s.MapFrom(o => o.LockoutEnabled));
    }
}