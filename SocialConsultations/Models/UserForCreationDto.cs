using SocialConsultations.Entities;
using System.ComponentModel.DataAnnotations;

namespace SocialConsultations.Models
{
    public class UserForCreationDto
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

        public string Description { get; set; } = String.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }

        public Guid ConfirmationCode { get; set; } = Guid.NewGuid();
    }
}
