using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConsultations.Entities
{
    public class JoinRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public Community Community { get; set; }

        public InviteStatus Status { get; set; } = InviteStatus.Pending;
    }
    public enum InviteStatus
    {
        Pending,
        Accepted,
        Rejected
    }
}
