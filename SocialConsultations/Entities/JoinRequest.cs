using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConsultations.Entities
{
    public class JoinRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public User User { get; set; }
        public int UserId { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public Community Community { get; set; }
        public int CommunityId { get; set; }

        public InviteStatus Status { get; set; } = InviteStatus.Pending;
    }
    public enum InviteStatus
    {
        Pending,
        Accepted,
        Rejected
    }
}
