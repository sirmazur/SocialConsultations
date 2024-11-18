using SocialConsultations.Entities;
using System.ComponentModel.DataAnnotations;

namespace SocialConsultations.Models
{
    public class CommentForUpdateDto
    {
        public int AuthorId { get; set; }
        public string Content { get; set; }
    }
}
