using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SocialConsultations.Entities;
using SocialConsultations.Helpers;
using SocialConsultations.Models;
using SocialConsultations.Services.Basic;
using SocialConsultations.Services.UserServices;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace SocialConsultations.Services.CommunityServices
{
    public class CommunityService : BasicService<CommunityDto, Community, CommunityFullDto, CommunityForCreationDto, CommunityForUpdateDto>, ICommunityService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IBasicRepository<JoinRequest> _joinrequestRepository;
        public CommunityService(IMapper mapper, IConfiguration configuration, IBasicRepository<Community> basicRepository, IUserRepository userRepository, IBasicRepository<JoinRequest> joinRequestRepository) : base(mapper, basicRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _joinrequestRepository = joinRequestRepository;
        }

        public async Task CreateJoinRequest(int userId, int communityId)
        {
            var user = await _userRepository.GetUserById(userId);
            var community = await _basicRepository.GetQueryableAll().Include(c => c.Members).Include(c => c.Administrators).Include(c => c.JoinRequests).FirstOrDefaultAsync(c => c.Id == communityId);
            if (user is null)
            {
                throw new Exception("User not found");
            }
            else
            if (community is null)
            {
                throw new Exception("Community not found");
            }
            else
            if (community.Administrators.Select(c=>c.Id).Contains(userId) || community.Members.Select(c => c.Id).Contains(userId))
            {
                throw new Exception("User is already a member or administrator of this community");
            }
            else
            if(community.JoinRequests.Where(c => c.UserId == userId &&  c.Status==InviteStatus.Pending).Any())
            {
                throw new Exception("User has already requested to join this community");
            }

            community.JoinRequests.Add(
                new JoinRequest() {
                Status = InviteStatus.Pending,
                User = user});
            await _basicRepository.SaveChangesAsync();
        }

        public async Task AcceptJoinRequest(int requestId, int communityId)
        {
            var request = await _joinrequestRepository.GetQueryableAll().Include(c=>c.User).Where(d=>d.Id == requestId).FirstOrDefaultAsync() ?? throw new Exception("Request not found");
            if (request.Status != InviteStatus.Pending)
            {
                throw new Exception("Request is already accepted or rejected");
            }
            var community = await _basicRepository.GetByIdAsync(communityId);
            if (community is null)
            {
                throw new Exception("Community not found");
            }
            request.Status = InviteStatus.Accepted;
            community.Members.Add(request.User);
            await _basicRepository.SaveChangesAsync();
            await _joinrequestRepository.SaveChangesAsync();
        }

        public async Task<CommunityFullDto> GetExtendedByIdWithEagerLoadingAsyncCustom(int id)
        {
            var item = await _basicRepository.GetQueryableAll().Include(c => c.Administrators).Include(c => c.Members).Include(c => c.JoinRequests).ThenInclude(c=>c.User)
                .Include(c => c.Avatar).Include(d => d.Background).Include(g => g.Issues).FirstOrDefaultAsync(c => c.Id == id);
            var itemToReturn = _mapper.Map<CommunityFullDto>(item);
            return itemToReturn;
        }

        public async Task<bool> ValidateAdmin(int userId, int communityId)
        {
            Expression<Func<Community, object>>[] includeProperties = { c => c.Administrators };
            var community = await GetExtendedByIdWithEagerLoadingNoTrackingAsync(communityId, includeProperties);
            var adminIds = community.Administrators.Select(a => a.Id).ToList();
            await _basicRepository.SaveChangesAsync();
            if (!adminIds.Contains(userId))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> ValidateMember(int userId, int communityId)
        {
            Expression<Func<Community, object>>[] includeProperties = { c => c.Administrators, d=>d.Members };
            var community = await GetExtendedByIdWithEagerLoadingNoTrackingAsync(communityId, includeProperties);
            var adminIds = community.Administrators.Select(a => a.Id).ToList();
            var userIds = community.Members.Select(a => a.Id).ToList();
            await _basicRepository.SaveChangesAsync();
            if (!adminIds.Contains(userId) && !userIds.Contains(userId))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task RejectJoinRequest(int requestId, int communityId)
        {
            var request = await _joinrequestRepository.GetByIdAsync(requestId);
            if (request.Status != InviteStatus.Pending)
            {
                throw new Exception("Request is already accepted or rejected");
            }
            var community = await _basicRepository.GetByIdAsync(communityId);
            if (community is null)
            {
                throw new Exception("Community not found");
            }
            request.Status = InviteStatus.Rejected;
            await _basicRepository.SaveChangesAsync();
            await _joinrequestRepository.SaveChangesAsync();
        }



        public async Task<CommunityForCreationDto> GetCommunityForCreationDto(CommunityForClientCreationDto community, int userid)
        {
            var initialAdministrator = await _userRepository.GetUserById(userid);
            List<User> users = [initialAdministrator];
            var communityToReturn = new CommunityForCreationDto
            {
                Name = community.Name,
                Description = community.Description,
                Avatar = community.Avatar,
                Background = community.Background,
                Administrators = users,
                Latitude = community.Latitude,
                Longitude = community.Longitude,
                IsPublic = community.IsPublic
            };
            return communityToReturn;
        }

        public async Task<List<CommunityFullDto>> GetClosestCommunities(Location location, int amount)
        {
            var communities = await _basicRepository.GetQueryableAll().Include(c=>c.Avatar).OrderBy(c => Math.Sqrt(Math.Pow(c.Latitude - location.Latitude, 2) + Math.Pow(c.Longitude - location.Longitude, 2))).Take(amount).ToListAsync();
            return _mapper.Map<List<CommunityFullDto>>(communities);
        }
    }
}
