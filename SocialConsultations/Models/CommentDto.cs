using SocialConsultations.Entities;
using System.ComponentModel.DataAnnotations;

namespace SocialConsultations.Models
{
    public class CommentDto
    {
        public int Id { get; set; }
        public UserDto Author { get; set; }
        public int AuthorId { get; set; }
        public string Content { get; set; }
    }
}
