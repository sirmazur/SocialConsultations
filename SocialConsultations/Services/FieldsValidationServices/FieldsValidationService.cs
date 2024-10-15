using System.Reflection;

namespace SocialConsultations.Services.FieldsValidationServices
{
    public class FieldsValidationService : IFieldsValidationService
    {

        public bool TypeHasProperties<T>(string? fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }
            var fieldsAfterSplit = fields.Split(',');
            foreach (var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();
                var propertyInfo = typeof(T)
                    .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
