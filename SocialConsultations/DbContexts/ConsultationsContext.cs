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
        public DbSet<Solution> Solutions { get; set; }

        public ConsultationsContext(DbContextOptions<ConsultationsContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)        
                .WithMany(u => u.Comments)    
                .HasForeignKey(c => c.AuthorId) 
                .OnDelete(DeleteBehavior.Cascade); 

          
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
