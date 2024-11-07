using SocialConsultations.Filters;

namespace SocialConsultations.Helpers
{
    /// <summary>
    /// Resource parameters for pagination, search, ordering and field selection
    /// </summary>
    public class ResourceParameters
    {
        const int maxPageSize = 100000;
        /// <summary>
        /// Search query string
        /// </summary>
        public string? SearchQuery { get; set; }
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 100000;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
        /// <summary>
        /// Orderby "{Field} {Order}" where Order is asc or desc
        /// </summary>
        public string OrderBy { get; set; } = "Id";
        /// <summary>
        /// Fields for data shaping
        /// </summary>
        public string? Fields { get; set; }
    }
}
