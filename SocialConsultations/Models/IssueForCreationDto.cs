using SocialConsultations.Entities;

namespace SocialConsultations.Models
{
    public class IssueForCreationDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int CommunityId { get; set; }
        public List<FileDataForCreationDto> Files { get; set; }
        public DateTime CurrentStateEndDate { get; set; }
    }
}
