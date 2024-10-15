using Microsoft.EntityFrameworkCore;
using SocialConsultations.Entities;

namespace SocialConsultations.DbContexts
{
    public class ConsultationsContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Community> Communities { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Poll> Polls { get; set; }

        public ConsultationsContext(DbContextOptions<ConsultationsContext> options)
            : base(options)
        {
        }
    }
}
