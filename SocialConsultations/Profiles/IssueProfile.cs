using AutoMapper;
using SocialConsultations.Entities;

namespace SocialConsultations.Profiles
{
    public class IssueProfile : Profile
    {
        public IssueProfile()
        {
            CreateMap<Entities.Issue, Models.IssueDto>();
            CreateMap<Models.IssueDto, Entities.Issue>();
            CreateMap<Issue, Models.IssueForUpdateDto>();
            CreateMap<Models.IssueForCreationDto, Entities.Issue>();
            CreateMap<Models.IssueForUpdateDto, Entities.Issue>();
            CreateMap<Entities.Issue, Models.IssueFullDto>();
        }
    }
}
