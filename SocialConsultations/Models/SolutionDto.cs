﻿using SocialConsultations.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConsultations.Models
{
    public class SolutionDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int IssueId { get; set; }
    }
}
