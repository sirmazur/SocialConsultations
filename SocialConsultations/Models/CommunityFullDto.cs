using SocialConsultations.Entities;

namespace SocialConsultations.Models
{
    public class CommunityFullDto
    {
        public int Id { get; set; }


        public string Name { get; set; }


        public string Description { get; set; }

        public FileDataDto? Avatar { get; set; }

        public FileDataDto? Background { get; set; }

        public List<User> Administrators { get; set; }

        public List<User> Members { get; set; }

        public List<Issue> Issues { get; set; }

        public List<JoinRequest> JoinRequests { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool IsPublic { get; set; }
    }
}
