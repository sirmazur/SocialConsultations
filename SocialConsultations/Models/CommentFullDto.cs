using SocialConsultations.Entities;
using System.ComponentModel.DataAnnotations;

namespace SocialConsultations.Models
{
    public class CommentFullDto
    {
        public int Id { get; set; }
        public UserFullDto Author { get; set; }
        public int AuthorId { get; set; }
        public IssueDto Issue { get; set; }
        public int IssueId { get; set; }
        public string Content { get; set; }
        public ICollection<UserDto> Upvotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public IssueStatus IssueStatus { get; set; }
    }
}
