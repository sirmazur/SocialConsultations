using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConsultations.Entities
{
    public class Issue
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public Community Community { get; set; }
        public int CommunityId { get; set; }
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public List<FileData> Files { get; set; } = new List<FileData>();
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public List<Solution> Solutions { get; set; } = new List<Solution>();
        public IssueStatus IssueStatus { get; set; } = IssueStatus.GatheringInformation;
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public List<Comment> Comments { get; set; } = new List<Comment>();

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime CurrentStateEndDate { get; set; }

    }

    public enum IssueStatus
    {
        GatheringInformation, // commenting, adding files, adding solutions
        Voting, // commenting, adding files, voting
        InProgress, // commenting, adding files
        FeedbackCollection, // commenting, adding files
        Completed // no more interactions allowed
    }
}
