using SocialConsultations.Filters;
using System.Linq.Expressions;

namespace SocialConsultations.Services.Basic
{
    public interface IBasicRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        IQueryable<TEntity> GetQueryableAll();
        Task<(bool,TEntity?)> CheckIfIdExistsAsync(int id);
        void DeleteAsync(TEntity entity);
        Task SaveChangesAsync();
        Task<TEntity> GetByIdWithEagerLoadingAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity> AddAsync(TEntity entity);
        IQueryable<TEntity> GetQueryableAllWithEagerLoadingAsync(params Expression<Func<TEntity, object>>[] includeProperties);
    }
}
