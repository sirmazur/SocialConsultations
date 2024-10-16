using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConsultations.Entities
{
    public class Issue
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }
}
