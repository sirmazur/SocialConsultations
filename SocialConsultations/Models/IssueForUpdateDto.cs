using SocialConsultations.Entities;

namespace SocialConsultations.Models
{
    public class IssueForUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public CommunityDto? Community { get; set; }
        public int? CommunityId { get; set; }
        public List<FileDataDto>? Files { get; set; }
        public List<SolutionDto>? Solutions { get; set; }
        public IssueStatus? IssueStatus { get; set; }
        public List<CommentDto>? Comments { get; set; }
        public DateTime CurrentStateEndDate { get; set; }
    }
}
