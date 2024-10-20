﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace SocialConsultations.Entities
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
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
        public bool Confirmed { get; set; } = false;

    }
}
