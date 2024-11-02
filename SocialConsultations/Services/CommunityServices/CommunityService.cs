using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SocialConsultations.Entities;
using SocialConsultations.Helpers;
using SocialConsultations.Models;
using SocialConsultations.Services.Basic;
using SocialConsultations.Services.UserServices;
using System.Runtime.CompilerServices;

namespace SocialConsultations.Services.CommunityServices
{
    public class CommunityService : BasicService<CommunityDto, Community, CommunityFullDto, CommunityForCreationDto, CommunityForUpdateDto>, ICommunityService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        public CommunityService(IMapper mapper, IConfiguration configuration, IBasicRepository<Community> basicRepository, IUserRepository userRepository) : base(mapper, basicRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
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
                IsPublic = community.IsPublic
            };
            return communityToReturn;
        }
    }
}
