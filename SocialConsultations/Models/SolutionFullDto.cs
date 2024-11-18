using SocialConsultations.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConsultations.Models
{
    public class SolutionFullDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<FileDataDto> Files { get; set; }
        public List<UserDto> UserVotes { get; set; }
        public int IssueId { get; set; }
    }
}
