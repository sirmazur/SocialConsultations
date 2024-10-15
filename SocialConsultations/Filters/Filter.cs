namespace SocialConsultations.Filters
{
    public abstract class Filter : IFilter
    {
        public string FieldName { get; set; }
        public object Value { get; set; }

        public Filter(string fieldName, object value) 
        {
            FieldName = fieldName;
            Value = value;
        }
    }
}
