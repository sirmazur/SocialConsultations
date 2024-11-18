using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace SocialConsultations.Entities
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(30)]
        public string Surname { get; set; }

        [Required]
        [MinLength(7)]
        [MaxLength(20)]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }

        public Guid ConfirmationCode { get; set; } = Guid.NewGuid();

        [InverseProperty("Author")]
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        [InverseProperty("Upvotes")]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public ICollection<Comment> UpvotedComments { get; set; } = new List<Comment>();

        [InverseProperty("Administrators")]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public List<Community> AdminCommunities { get; set; } = new List<Community>();

        [InverseProperty("Members")]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public List<Community> MemberCommunities { get; set; } = new List<Community>();

        public FileData? Avatar { get; set; }
        public bool Confirmed { get; set; } = false;

        public DateTime LastPasswordReminder { get; set; } = DateTime.UtcNow.AddDays(-10);

    }
}
