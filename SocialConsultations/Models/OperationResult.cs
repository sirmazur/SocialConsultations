namespace SocialConsultations.Models
{
    public class OperationResult<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
        public int HttpResponseCode { get; set; }
    }
}
