using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConsultations.Entities
{
    public class Community
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MinLength(7)]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public FileData? Avatar { get; set; }
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public FileData? Background { get; set; }

        [InverseProperty("AdminCommunities")]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public List<User> Administrators { get; set; } = new List<User>();

        [InverseProperty("MemberCommunities")]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public List<User> Members { get; set; } = new List<User>();
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public List<Issue> Issues { get; set; } = new List<Issue>();
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public List<JoinRequest> JoinRequests { get; set; } = new List<JoinRequest>();

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool IsPublic { get; set; }
    }
}
