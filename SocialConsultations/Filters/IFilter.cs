namespace SocialConsultations.Filters
{
    public interface IFilter
    {
        public string FieldName { get; set; }
        public object Value { get; set; }        
    }
}
