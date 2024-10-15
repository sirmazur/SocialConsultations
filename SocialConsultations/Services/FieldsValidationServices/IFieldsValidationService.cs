namespace SocialConsultations.Services.FieldsValidationServices
{
    public interface IFieldsValidationService
    {
        bool TypeHasProperties<T>(string? fields);
    }
}