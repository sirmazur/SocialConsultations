using SocialConsultations.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConsultations.Models
{
    public class IssueDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CommunityId { get; set; }
        public IssueStatus IssueStatus { get; set; } = IssueStatus.GatheringInformation;
        public DateTime CreatedAt { get; set; }
        public DateTime CurrentStateEndDate { get; set; }
    }
}
