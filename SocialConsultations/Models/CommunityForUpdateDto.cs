using SocialConsultations.Entities;

namespace SocialConsultations.Models
{
    public class CommunityForUpdateDto
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public FileDataForCreationDto? Avatar { get; set; }

        public FileDataForCreationDto? Background { get; set; }

        public List<UserDto>? Administrators { get; set; }

        public List<UserDto>? Members { get; set; }

        public List<IssueDto>? Issues { get; set; }

        public List<JoinRequestDto>? JoinRequests { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}
