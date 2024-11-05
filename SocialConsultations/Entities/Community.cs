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

        public FileData? Avatar { get; set; }

        public FileData? Background { get; set; }

        [InverseProperty("AdminCommunities")]
        public List<User> Administrators { get; set; } = new List<User>();

        [InverseProperty("MemberCommunities")]
        public List<User> Members { get; set; } = new List<User>();

        public List<Issue> Issues { get; set; } = new List<Issue>();

        public List<JoinRequest> JoinRequests { get; set; } = new List<JoinRequest>();

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool IsPublic { get; set; }
    }
}
