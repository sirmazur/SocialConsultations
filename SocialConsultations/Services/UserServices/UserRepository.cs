using SocialConsultations.DbContexts;
using SocialConsultations.Entities;
using Microsoft.EntityFrameworkCore;

namespace SocialConsultations.Services.UserServices
{
    public class UserRepository : IUserRepository
    {
        private readonly ConsultationsContext _context;
        public UserRepository(ConsultationsContext context)
        {
            _context = context;
        }

        public async Task<bool> IsEmailAvailable(string email)
        {
            return !(await _context.Users.AnyAsync(u => u.Email == email));
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }


    }
}
