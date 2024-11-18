using SocialConsultations.Entities;
using SocialConsultations.Models;
using SocialConsultations.Services.Basic;

namespace SocialConsultations.Services.IssueServices
{
    public interface IIssueService : IBasicService<IssueDto, Issue, IssueFullDto, IssueForCreationDto, IssueForUpdateDto>
    {
    }
}
