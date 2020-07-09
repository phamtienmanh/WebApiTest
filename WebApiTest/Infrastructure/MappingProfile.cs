using AutoMapper;
using Microsoft.AspNetCore.Identity;
using WebApiTest.DataAccess.Entities;
using WebApiTest.Infrastructure.Models;

namespace WebApiTest.Infrastructure
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<PostModel, PostTranslation>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    if (dest.Post != null) dest.Post.Published = src.Published;
                })
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Post.Id));
            CreateMap<PostModel, Post>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.PostTranslations.Add(new PostTranslation()
                    {
                        LanguageCode = src.LanguageCode,
                        Title = src.Title,
                        Description = src.Description
                    });
                });
            CreateMap<RegistrationModel, IdentityUser>();
        }
    }
}
