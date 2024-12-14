using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConsultations.Entities
{
    public class Solution
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [DeleteBehavior(DeleteBehavior.SetNull)]
        public List<FileData> Files { get; set; } = new List<FileData>();
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public List<User> UserVotes { get; set; } = new List<User>();
        public Issue Issue { get; set; }
        public int IssueId { get; set; }
    }
}
