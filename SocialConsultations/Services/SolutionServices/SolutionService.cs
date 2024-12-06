using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SocialConsultations.Entities;
using SocialConsultations.Models;
using SocialConsultations.Services.Basic;
using SocialConsultations.Services.IssueServices;
using SocialConsultations.Services.UserServices;

namespace SocialConsultations.Services.SolutionServices
{
    public class SolutionService : BasicService<SolutionDto, Solution, SolutionFullDto, SolutionForCreationDto, SolutionForUpdateDto>, ISolutionService
    {
        private readonly IConfiguration _configuration;
        private readonly ISolutionRepository _solutionRepository;
        private readonly IUserService _userService;
        private readonly IIssueService _issueService;
        public SolutionService(IMapper mapper, IConfiguration configuration, IBasicRepository<Solution> basicRepository, ISolutionRepository solutionRepository, IUserService userService, IIssueService issueService) : base(mapper, basicRepository)
        {
            _configuration = configuration;
            _solutionRepository = solutionRepository;
            _userService = userService;
            _issueService=issueService;
        }

        public async Task<SolutionFullDto> ToggleUpvoteSolution(int solutionId, int userId)
        {
            var solution = await _basicRepository.GetByIdAsync(solutionId);
            if (solution == null)
            {
                throw new Exception("Solution not found");
            }

            var issue = await _issueService.GetByIdAsync(solution.IssueId);
            if(issue.IssueStatus != IssueStatus.Voting)
            {
                throw new Exception("Voting is only allowed during voting issue status.");
            }
            var solutions = _basicRepository.GetQueryableAll().Include(c => c.UserVotes).Where(d => d.UserVotes.Any(e => e.Id == userId) && d.Id != solutionId && d.IssueId == issue.Id);
            if(solutions is not null && solutions.Count()>0)
            {
                throw new Exception("User already voted for another solution in this issue.");
            }
            var user = await _userService.GetEntityByIdAsync(userId);
            if (user is null)
            {
                throw new Exception("User not found");
            }
            if (solution.UserVotes.Contains(user))
            {
                solution.UserVotes.Remove(user);
            }
            else
            {
                solution.UserVotes.Add(user);
            }
            await _basicRepository.SaveChangesAsync();
            return _mapper.Map<SolutionFullDto>(solution);
        }
    }
}
