using AutoMapper;
using SocialConsultations.Helpers;

namespace SocialConsultations.Profiles
{
    public class PagedListConverter<TEntity, TDto> : ITypeConverter<PagedList<TEntity>, PagedList<TDto>>
    {
        public PagedList<TDto> Convert(PagedList<TEntity> source, PagedList<TDto> destination, ResolutionContext context)
        {
            var items = context.Mapper.Map<List<TEntity>, List<TDto>>(source);
            return new PagedList<TDto>(items, source.TotalCount, source.CurrentPage, source.PageSize);
        }
    }
}
