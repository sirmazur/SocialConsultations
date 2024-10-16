﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConsultations.Entities
{
    public class Community
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MinLength(7)]
        [MaxLength(20)]
        public string Name { get; set; }

        public bool IsPublic { get; set; }
    }
}
