using SocialConsultations.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using SocialConsultations.Entities;

namespace SocialConsultations.Services.Basic
{
    public class BasicRepository<TEntity> : IBasicRepository<TEntity> where TEntity : class
    {

        private readonly ConsultationsContext _context;
        public BasicRepository(ConsultationsContext context)
        {
            _context = context;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            return entity;
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            if(typeof(TEntity) == typeof(Issue))
            {
                return await _context.Set<Issue>().Include(c => c.Files).FirstOrDefaultAsync(c => c.Id == id) as TEntity;
            }
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public IQueryable<TEntity> GetQueryableAll()
        {
            return _context.Set<TEntity>();
        }


        public async Task<(bool,TEntity?)> CheckIfIdExistsAsync(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            return (entity != null,entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }
     

        public async Task<TEntity> GetByIdWithEagerLoadingAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.SingleOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task<TEntity> GetByIdWithEagerLoadingNoTrackingAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = _context.Set<TEntity>().AsNoTracking();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.SingleOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public IQueryable<TEntity> GetQueryableAllWithEagerLoadingAsync(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

    }
}
