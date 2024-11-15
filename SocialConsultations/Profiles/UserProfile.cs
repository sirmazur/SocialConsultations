using AutoMapper;
using SocialConsultations.Entities;
using SocialConsultations.Models;

namespace SocialConsultations.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserForCreationDto, User>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<User, UserForUpdateDto>();
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<User, UserFullDto>();
        }
    }
}
