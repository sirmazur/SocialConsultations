using AutoMapper;
using SocialConsultations.Entities;
using SocialConsultations.Models;
using SocialConsultations.Services.Basic;
using SocialConsultations.Services.IssueServices;

namespace SocialConsultations.Services.SolutionServices
{
    public class SolutionService : BasicService<SolutionDto, Solution, SolutionFullDto, SolutionForCreationDto, SolutionForUpdateDto>, ISolutionService
    {
        private readonly IConfiguration _configuration;
        private readonly ISolutionRepository _solutionRepository;
        public SolutionService(IMapper mapper, IConfiguration configuration, IBasicRepository<Solution> basicRepository, ISolutionRepository solutionRepository) : base(mapper, basicRepository)
        {
            _configuration = configuration;
            _solutionRepository = solutionRepository;
        }
    }
}
