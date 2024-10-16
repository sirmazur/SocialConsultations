using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConsultations.Entities
{
    public class Poll
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }
}
