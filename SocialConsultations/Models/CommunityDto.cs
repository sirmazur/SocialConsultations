using SocialConsultations.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocialConsultations.Models
{
    public class CommunityDto
    {

        public int Id { get; set; }


        public string Name { get; set; }


        public string Description { get; set; }

        public FileDataDto? Avatar { get; set; }

        public FileDataDto? Background { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool IsPublic { get; set; }
    }
}
