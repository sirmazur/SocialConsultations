using SocialConsultations.Entities;

namespace SocialConsultations.Models
{
    public class CommunityForClientCreationDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public FileDataForCreationDto? Avatar { get; set; }

        public FileDataForCreationDto? Background { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsPublic { get; set; }
    }
}
