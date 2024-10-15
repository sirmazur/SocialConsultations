using System.Dynamic;
using System.Reflection;

namespace SocialConsultations.Helpers
{
    public static class ObjectExtensions
    {
        public static ExpandoObject ShapeDataForObject<TSource>(
          this TSource source,
          string? fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var expandoObject = new ExpandoObject();


            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos = typeof(TSource)
                    .GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                foreach(var propertyInfo in propertyInfos)
                {
                    var propertyValue = propertyInfo.GetValue(source);
                    ((IDictionary<string, object?>)expandoObject)
                        .Add(propertyInfo.Name, propertyValue);
                }
                return expandoObject;
            }
            else
            {
                var fieldsAfterSplit = fields.Split(',');
                foreach (var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();
                    var propertyInfo = typeof(TSource)
                        .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property {propertyName} wasn't found on {typeof(TSource)}");
                    }
                    var propertyValue = propertyInfo.GetValue(source);
                    ((IDictionary<string, object?>)expandoObject)
                        .Add(propertyInfo.Name, propertyValue);
                }
                return expandoObject;
                
            }
            
        }
    }
}
