using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConsultations.Entities
{
    public class FileData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public byte[] Data { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DataType Type { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public Solution? Solution { get; set; }

        public DateTime? DeletionMarkTimestamp { get; set; }
    }
    public enum DataType
    {
        Image,
        Other
    }
}
