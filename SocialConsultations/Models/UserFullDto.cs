using SocialConsultations.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocialConsultations.Models
{
    public class UserFullDto
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }


        public string? Email { get; set; }
        public bool Confirmed { get; set; } = false;

    }
}
