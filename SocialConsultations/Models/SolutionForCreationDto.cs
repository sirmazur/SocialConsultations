using SocialConsultations.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConsultations.Models
{
    public class SolutionForCreationDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<FileDataForCreationDto> Files { get; set; }
        public int IssueId { get; set; }
    }
}
