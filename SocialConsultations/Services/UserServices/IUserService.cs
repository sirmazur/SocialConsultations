using SocialConsultations.Controllers;
using SocialConsultations.Entities;
using SocialConsultations.Models;
using SocialConsultations.Services.Basic;

namespace SocialConsultations.Services.UserServices
{
    public interface IUserService : IBasicService<UserDto, User, UserFullDto, UserForCreationDto, UserForUpdateDto>
    {
        public Task<UserFullDto> AuthenticateUser(UserParams userParams);  
        //public Task<Role> AuthorizeUser(int userId);
        public Task<UserDto> CreateUser(UserForClientCreation user);
        public string GenerateToken(UserFullDto user);
    }
}
