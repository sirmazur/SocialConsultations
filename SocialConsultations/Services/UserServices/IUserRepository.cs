using SocialConsultations.Entities;

namespace SocialConsultations.Services.UserServices
{
    public interface IUserRepository
    {
        Task<bool> IsEmailAvailable(string name);
        Task<User?> GetUserByEmail(string name);
        Task<User?> GetUserById(int id);
    }
}
