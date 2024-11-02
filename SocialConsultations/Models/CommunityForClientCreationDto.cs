using SocialConsultations.Entities;

namespace SocialConsultations.Models
{
    public class CommunityForClientCreationDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public FileData? Avatar { get; set; }

        public FileData? Background { get; set; }

        public bool IsPublic { get; set; }
    }
}
