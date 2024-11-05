using System.ComponentModel.DataAnnotations;

namespace SocialConsultations.Models
{
    public class FileDataForCreationDto
    {
        [Required]
        public byte[] Data { get; set; }
        [Required]
        public string Description { get; set; }
        public Entities.DataType Type { get; set; }
    }
}
