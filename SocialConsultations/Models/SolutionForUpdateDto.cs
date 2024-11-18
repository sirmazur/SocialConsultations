using SocialConsultations.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConsultations.Models
{
    public class SolutionForUpdateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<FileDataForCreationDto> Files { get; set; }


    }
}
