using AutoMapper;
using SocialConsultations.Entities;
using SocialConsultations.Models;
using SocialConsultations.Services.Basic;
using SocialConsultations.Services.CommunityServices;
using SocialConsultations.Services.UserServices;

namespace SocialConsultations.Services.IssueServices
{
    public class IssueService : BasicService<IssueDto, Issue, IssueFullDto, IssueForCreationDto, IssueForUpdateDto>, IIssueService
    {
        private readonly IConfiguration _configuration;
        private readonly IIssueRepository _issueRepository;
        public IssueService(IMapper mapper, IConfiguration configuration, IBasicRepository<Issue> basicRepository, IIssueRepository issueRepository) : base(mapper, basicRepository)
        {
            _configuration = configuration;
            _issueRepository = issueRepository;
        }
    }
}
