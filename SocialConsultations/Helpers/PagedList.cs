using Microsoft.EntityFrameworkCore;

namespace SocialConsultations.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList(List<T> items, int totalCount, int currentPage, int pageSize)
        {
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            AddRange(items);
        }

        public static async Task<PagedList<T>> CreateAsync(
            IQueryable<T> source, int currentPage, int pageSize)
        {
            var count = source.Count();
            var items = await source
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, currentPage, pageSize);
        }
    }
}
