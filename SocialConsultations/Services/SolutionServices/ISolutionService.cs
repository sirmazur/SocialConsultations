using SocialConsultations.Entities;
using SocialConsultations.Models;
using SocialConsultations.Services.Basic;

namespace SocialConsultations.Services.SolutionServices
{
    public interface ISolutionService : IBasicService<SolutionDto, Solution, SolutionFullDto, SolutionForCreationDto, SolutionForUpdateDto>
    {
    }
}
