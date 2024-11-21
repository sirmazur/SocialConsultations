using SocialConsultations.Entities;

namespace SocialConsultations.Models
{
    public class IssueFullDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public CommunityDto Community { get; set; }
        public int CommunityId { get; set; }
        public List<FileDataDto> Files { get; set; }
        public List<SolutionFullDto> Solutions { get; set; }
        public IssueStatus IssueStatus { get; set; }
        public List<CommentFullDto> Comments { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime CurrentStateEndDate { get; set; }
    }
}
