using SocialConsultations.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocialConsultations.Models
{
    public class JoinRequestDto
    {
        public int Id { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }

        public InviteStatus Status { get; set; }
    }
}
