using SocialConsultations.Entities;
using System.ComponentModel.DataAnnotations;

namespace SocialConsultations.Models
{
    public class CommentForCreationDto
    {
        public int AuthorId { get; set; }
        public int IssueId { get; set; }
        public string Content { get; set; }
        public IssueStatus IssueStatus { get; set; }
    }
}
