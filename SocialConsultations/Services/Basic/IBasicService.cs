using SocialConsultations.Filters;
using SocialConsultations.Helpers;
using SocialConsultations.Models;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;

namespace SocialConsultations.Services.Basic
{
    public interface IBasicService<TDto, TEntity, TExtendedDto, TCreationDto, TUpdateDto> where TDto : class where TEntity : class where TExtendedDto : class where TCreationDto : class where TUpdateDto : class
    {
        Task<TDto> GetByIdAsync(int id);
        Task<PagedList<TDto>> GetAllAsync(IEnumerable<IFilter>? filters, ResourceParameters parameters);
        Task<PagedList<TExtendedDto>> GetFullAllAsync(IEnumerable<IFilter>? filters, ResourceParameters parameters);
        Task<TDto> CreateAsync(TCreationDto creationDto);
        Task<OperationResult<TDto>> UpdateAsync(int id, TUpdateDto creationDto);
        Task<OperationResult<TDto>> PartialUpdateAsync(int id, JsonPatchDocument<TUpdateDto> patchDocument);
        Task<(bool, TEntity?)> CheckIfIdExistsAsync(int id);
        Task<OperationResult<TDto>> DeleteByIdAsync(int id);
        Task<TEntity> GetEntityByIdAsync(int id);
        Task<TEntity> GetEntityByIdWithEagerLoadingAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TDto> GetByIdWithEagerLoadingAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TExtendedDto> GetExtendedByIdWithEagerLoadingAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<IEnumerable<TExtendedDto>> GetExtendedListWithEagerLoadingAsync(IEnumerable<int> ids, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<PagedList<TExtendedDto>> GetFullAllWithEagerLoadingAsync(IEnumerable<IFilter>? filters, ResourceParameters parameters, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<PagedList<TDto>> GetAllWithEagerLoadingAsync(IEnumerable<IFilter>? filters, ResourceParameters parameters, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TExtendedDto> GetExtendedByIdWithEagerLoadingNoTrackingAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties);
    }
}
