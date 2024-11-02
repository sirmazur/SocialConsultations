using AutoMapper;
using SocialConsultations.Entities;
using SocialConsultations.Models;

namespace SocialConsultations.Profiles
{
    public class CommunityProfile : Profile
    {
        public CommunityProfile()
        {
            CreateMap<CommunityForCreationDto, Community>();
            CreateMap<CommunityForUpdateDto, Community>();
            CreateMap<Community, CommunityForUpdateDto>();
            CreateMap<Community, CommunityDto>();
            CreateMap<Community, CommunityFullDto>();
        }
    }
}
