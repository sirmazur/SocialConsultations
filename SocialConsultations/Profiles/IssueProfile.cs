using AutoMapper;

namespace SocialConsultations.Profiles
{
    public class IssueProfile : Profile
    {
        public IssueProfile()
        {
            CreateMap<Entities.Issue, Models.IssueDto>();
            CreateMap<Models.IssueDto, Entities.Issue>();
            CreateMap<Models.IssueForCreationDto, Entities.Issue>();
            CreateMap<Models.IssueForUpdateDto, Entities.Issue>();
            CreateMap<Entities.Issue, Models.IssueFullDto>();
        }
    }
}
