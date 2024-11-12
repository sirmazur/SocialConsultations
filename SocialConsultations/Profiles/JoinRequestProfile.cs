using AutoMapper;

namespace SocialConsultations.Profiles
{
    public class JoinRequestProfile : Profile
    {
        public JoinRequestProfile()
        {
            CreateMap<Entities.JoinRequest, Models.JoinRequestDto>();
            CreateMap<Models.JoinRequestDto, Entities.JoinRequest>();
        }
    }
}
