using SocialConsultations.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConsultations.Models
{
    public class UserForClientCreation
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
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
    }
}
