using AutoMapper;

namespace SocialConsultations.Profiles
{
    public class SolutionProfile : Profile
    {
        public SolutionProfile()
        {
            CreateMap<Entities.Solution, Models.SolutionDto>();
            CreateMap<Models.SolutionDto, Entities.Solution>();
            CreateMap<Models.SolutionForCreationDto, Entities.Solution>();
            CreateMap<Models.SolutionForUpdateDto, Entities.Solution>();
            CreateMap<Entities.Solution, Models.SolutionFullDto>();
        }
    }
}
