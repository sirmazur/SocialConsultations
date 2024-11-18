using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConsultations.Entities
{
    public class Comment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [InverseProperty("Comments")]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public User Author { get; set; }
        public int AuthorId { get; set; }
        [Required]
        [MinLength(1)]
        public string Content { get; set; }

        [InverseProperty("UpvotedComments")]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public ICollection<User> Upvotes { get; set; } = new List<User>();
    }
}
