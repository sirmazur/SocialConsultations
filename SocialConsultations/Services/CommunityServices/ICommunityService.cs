using SocialConsultations.Entities;
using SocialConsultations.Helpers;
using SocialConsultations.Models;
using SocialConsultations.Services.Basic;

namespace SocialConsultations.Services.CommunityServices
{
    public interface ICommunityService : IBasicService<CommunityDto, Community, CommunityFullDto, CommunityForCreationDto, CommunityForUpdateDto>
    {
        public Task<CommunityForCreationDto> GetCommunityForCreationDto(CommunityForClientCreationDto community, int userid);
        public Task<List<CommunityFullDto>> GetClosestCommunities(Location location, int amount);
    }
}
