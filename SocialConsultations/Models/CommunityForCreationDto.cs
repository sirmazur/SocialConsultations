using SocialConsultations.Entities;

namespace SocialConsultations.Models
{
    public class CommunityForCreationDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public FileDataForCreationDto? Avatar { get; set; }

        public FileDataForCreationDto? Background { get; set; }

        public List<User> Administrators { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}
