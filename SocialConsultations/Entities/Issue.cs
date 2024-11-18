using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConsultations.Entities
{
    public class Issue
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Community Community { get; set; }
        public int CommunityId { get; set; }
        public List<FileData> Files { get; set; } = new List<FileData>();
        public List<Solution> Solutions { get; set; } = new List<Solution>();
        public IssueStatus IssueStatus { get; set; } = IssueStatus.GatheringInformation;
        public List<Comment> Comments { get; set; } = new List<Comment>();

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime CurrentStateEndDate { get; set; }

    }

    public enum IssueStatus
    {
        GatheringInformation,
        Voting,
        InProgress,
        FeedbackCollection,
        Completed
    }
}
