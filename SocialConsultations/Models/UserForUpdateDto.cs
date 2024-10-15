using SocialConsultations.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConsultations.Models
{
    public class UserForUpdateDto
    {

        [Required]
        [MinLength(1)]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(30)]
        public string Surname { get; set; }

        [Required]
        [MinLength(7)]
        [MaxLength(20)]
        public string Password { get; set; }
        [Required]
        public string? Email { get; set; }

    }
}
