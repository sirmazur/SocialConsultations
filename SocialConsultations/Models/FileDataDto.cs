using System.ComponentModel.DataAnnotations;

namespace SocialConsultations.Models
{
    public class FileDataDto
    {
        public int Id { get; set; }
        public byte[] Data { get; set; }
        public string Description { get; set; }
        public Entities.DataType Type { get; set; }
    }
}
