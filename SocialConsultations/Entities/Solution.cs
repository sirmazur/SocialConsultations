namespace SocialConsultations.Entities
{
    public class Solution
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public List<FileData> Files { get; set; }
        public int IssueId { get; set; }
        public Issue Issue { get; set; }
    }
}
