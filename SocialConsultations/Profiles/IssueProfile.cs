using AutoMapper;

namespace SocialConsultations.Profiles
{
    public class IssueProfile : Profile
    {
        public IssueProfile()
        {
            CreateMap<Entities.Issue, Models.IssueDto>();
            CreateMap<Models.IssueDto, Entities.Issue>();
        }
    }
}
