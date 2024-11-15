using AutoMapper;
using AutoMapper.Internal;
using SocialConsultations.Filters;
using SocialConsultations.Helpers;
using SocialConsultations.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;

namespace SocialConsultations.Services.Basic
{
    public class BasicService<TDto, TEntity, TExtendedDto, TCreationDto, TUpdateDto> where TEntity : class where TDto : class where TExtendedDto : class where TCreationDto : class where TUpdateDto : class
    {
        protected readonly IMapper _mapper;
        protected readonly IBasicRepository<TEntity> _basicRepository;
        public BasicService(IMapper mapper, IBasicRepository<TEntity> basicRepository)
        {
            _mapper=mapper;
            _basicRepository=basicRepository;
        }

        public async Task<TDto> CreateAsync(TCreationDto creationDto)
        {
            var entity = _mapper.Map<TEntity>(creationDto);
            await _basicRepository.AddAsync(entity);
            await _basicRepository.SaveChangesAsync();
            var entityToReturn = _mapper.Map<TDto>(entity);
            return entityToReturn;
        }

        public async Task<TDto> GetByIdAsync(int id)
        {
            var item = await _basicRepository.GetByIdAsync(id);
            var itemToReturn = _mapper.Map<TDto>(item);
            return itemToReturn;
        }

        public async Task<TEntity> GetEntityByIdAsync(int id)
        {
            var item = await _basicRepository.GetByIdAsync(id);
            return item;
        }

        public async Task<TDto> GetByIdWithEagerLoadingAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var item = await _basicRepository.GetByIdWithEagerLoadingAsync(id, includeProperties);
            var itemToReturn = _mapper.Map<TDto>(item);
            return itemToReturn;
        }

        public async Task<TExtendedDto> GetExtendedByIdWithEagerLoadingAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var item = await _basicRepository.GetByIdWithEagerLoadingAsync(id, includeProperties);
            var itemToReturn = _mapper.Map<TExtendedDto>(item);
            return itemToReturn;
        }

        public async Task<TExtendedDto> GetExtendedByIdWithEagerLoadingNoTrackingAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var item = await _basicRepository.GetByIdWithEagerLoadingNoTrackingAsync(id, includeProperties);
            var itemToReturn = _mapper.Map<TExtendedDto>(item);
            return itemToReturn;
        }

        public async Task<TEntity> GetEntityByIdWithEagerLoadingAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return await _basicRepository.GetByIdWithEagerLoadingAsync(id, includeProperties);
        }

        public async Task<PagedList<TDto>> GetAllAsync(IEnumerable<IFilter>? filters, ResourceParameters parameters)
        {
            var listToReturn = _basicRepository.GetQueryableAll();
            if(filters is not null)
            foreach (var filter in filters)
            {
                listToReturn = FilterEntity(listToReturn, filter);             
            }

            if (!string.IsNullOrWhiteSpace(parameters.SearchQuery))
            {
                listToReturn = SearchEntityByProperty(listToReturn, parameters.SearchQuery);
            }

            listToReturn = ApplyOrdering(listToReturn, parameters.OrderBy);

            var finalList = await PagedList<TEntity>
                .CreateAsync(listToReturn, 
                parameters.PageNumber, 
                parameters.PageSize);
            var finalListToReturn = _mapper.Map<PagedList<TDto>>(finalList);
            return finalListToReturn;
        }

        public virtual async Task<PagedList<TDto>> GetAllWithEagerLoadingAsync(IEnumerable<IFilter>? filters, ResourceParameters parameters, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var listToReturn = _basicRepository.GetQueryableAllWithEagerLoadingAsync(includeProperties);
            if (filters is not null)
                foreach (var filter in filters)
                {
                    listToReturn = FilterEntity(listToReturn, filter);
                }

            if (!string.IsNullOrWhiteSpace(parameters.SearchQuery))
            {
                listToReturn = SearchEntityByProperty(listToReturn, parameters.SearchQuery);
            }

            listToReturn = ApplyOrdering(listToReturn, parameters.OrderBy);

            var finalList = await PagedList<TEntity>
                .CreateAsync(listToReturn,
                parameters.PageNumber,
                parameters.PageSize);
            var finalListToReturn = _mapper.Map<PagedList<TDto>>(finalList);
            return finalListToReturn;
        }

        public async Task<PagedList<TExtendedDto>> GetFullAllAsync(IEnumerable<IFilter>? filters, ResourceParameters parameters)
        {
            var listToReturn = _basicRepository.GetQueryableAll();
            if(filters is not null)
            foreach (var filter in filters)
            {
                listToReturn = FilterEntity(listToReturn, filter);
            }

            if (!string.IsNullOrWhiteSpace(parameters.SearchQuery))
            {
                listToReturn = SearchEntityByProperty(listToReturn, parameters.SearchQuery);
            }

            listToReturn = ApplyOrdering(listToReturn, parameters.OrderBy);

            var finalList = await PagedList<TEntity>
                .CreateAsync(listToReturn,
                parameters.PageNumber,
                parameters.PageSize);
            var finalListToReturn = _mapper.Map<PagedList<TExtendedDto>>(finalList);
            return finalListToReturn;
        }

        public virtual async Task<PagedList<TExtendedDto>> GetFullAllWithEagerLoadingAsync(IEnumerable<IFilter>? filters, ResourceParameters parameters, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var listToReturn = _basicRepository.GetQueryableAllWithEagerLoadingAsync(includeProperties);
            if (filters is not null)
                foreach (var filter in filters)
                {
                    listToReturn = FilterEntity(listToReturn, filter);
                }

            if (!string.IsNullOrWhiteSpace(parameters.SearchQuery))
            {
                listToReturn = SearchEntityByProperty(listToReturn, parameters.SearchQuery);
            }

            listToReturn = ApplyOrdering(listToReturn, parameters.OrderBy);

            var finalList = await PagedList<TEntity>
                .CreateAsync(listToReturn,
                parameters.PageNumber,
                parameters.PageSize);
            var finalListToReturn = _mapper.Map<PagedList<TExtendedDto>>(finalList);
            return finalListToReturn;
        }

        protected static IQueryable<TEntity> ApplyOrdering(IQueryable<TEntity> source, string orderBy)
        {
            var orderParams = orderBy.Split(',');

            foreach(var param in orderParams)
            {
                var trimmedParam = param.Trim();
                var orderDescending = trimmedParam.EndsWith(" desc");
                var indexOfFirstSpace = trimmedParam.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ? trimmedParam : trimmedParam.Remove(indexOfFirstSpace);

                var property = typeof(TEntity).GetProperty(propertyName);
                if (property == null)
                {
                    throw new ArgumentException($"Property {propertyName} does not exist.");
                }

                var parameter = Expression.Parameter(typeof(TEntity), "entity");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExpression = Expression.Lambda(propertyAccess, parameter);

                var resultExpression = Expression.Call(typeof(Queryable), orderDescending ? "OrderByDescending" : "OrderBy", new Type[] { typeof(TEntity), property.PropertyType }, source.Expression, Expression.Quote(orderByExpression));
                source = source.Provider.CreateQuery<TEntity>(resultExpression);
            }
            return source;
        }

        protected static IQueryable<TEntity> FilterEntity(IQueryable<TEntity> source, IFilter filter)
        {
                var entityType = typeof(TEntity);
                var parameter = Expression.Parameter(entityType, "entity");
                var searchProperty = entityType.GetProperty(filter.FieldName) ?? throw new ArgumentException($"Property {filter.FieldName} does not exist.");

                Expression finalExpression = Expression.Constant(false);

                if(filter is NumericFilter && searchProperty.PropertyType == typeof(int?) || searchProperty.PropertyType == typeof(int))
                {
                    var propertyExpression = Expression.Property(parameter, searchProperty);
                    var filterValue = Expression.Constant(filter.Value, typeof(int?));
                    var equalsExpression = Expression.Equal(propertyExpression, filterValue);
                    finalExpression = Expression.OrElse(finalExpression, equalsExpression);
                }
                else
                if(filter is TextFilter && searchProperty.PropertyType == typeof(string))
                {
                    var propertyExpression = Expression.Property(parameter, searchProperty);
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var containsExpression = Expression.Call(propertyExpression, containsMethod, Expression.Constant(Convert.ToString(filter.Value)));
                    finalExpression = Expression.OrElse(finalExpression, containsExpression);
                }

            
            var lambda = Expression.Lambda<Func<TEntity, bool>>(finalExpression, parameter);
                source = source.Where(lambda);
            

            return source;
        }

        protected static IQueryable<TEntity> SearchEntityByProperty(IQueryable<TEntity> source, string searchQuery)
        {

                var entityType = typeof(TEntity);
                var parameter = Expression.Parameter(entityType, "entity");
                var searchProperties = entityType.GetProperties();

                Expression finalExpression = Expression.Constant(false);

                foreach (var propertyInfo in searchProperties)
                {
                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        var propertyExpression = Expression.Property(parameter, propertyInfo);
                        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        var containsExpression = Expression.Call(propertyExpression, containsMethod, Expression.Constant(searchQuery));

                        finalExpression = Expression.OrElse(finalExpression, containsExpression);
                    }
                }

                var lambda = Expression.Lambda<Func<TEntity, bool>>(finalExpression, parameter);
                source = source.Where(lambda);           

            return source;
        }

        public async Task<IEnumerable<TExtendedDto>> GetExtendedListWithEagerLoadingAsync(IEnumerable<int> ids, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            List<TExtendedDto> list = new List<TExtendedDto>();
            foreach (var id in ids) 
            {
                var add = await _basicRepository.GetByIdWithEagerLoadingAsync(id, includeProperties);
                list.Add(_mapper.Map<TExtendedDto>(add));
            }
            return list;
        }

        public async Task<OperationResult<TDto>> UpdateAsync(int id, TUpdateDto creationDto)
        {
            var item = await _basicRepository.GetByIdAsync(id);
            if( item == null)
            {
                return new OperationResult<TDto>
                {
                    IsSuccess = false,
                    ErrorMessage = $"Entity of type {typeof(TEntity).Name} with id={id} does not exist.",
                    HttpResponseCode = 404
                };
            }
            else
            {
                _mapper.Map(creationDto, item);
                await _basicRepository.SaveChangesAsync();
                return new OperationResult<TDto>
                {
                    IsSuccess = true,
                    HttpResponseCode = 204
                };
            }
        }

        public async Task<OperationResult<TDto>> PartialUpdateAsync(int id, JsonPatchDocument<TUpdateDto> patchDocument)
        {
            var item = await _basicRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new OperationResult<TDto>
                {
                    IsSuccess = false,
                    ErrorMessage = $"Entity of type {typeof(TEntity).Name} with id={id} does not exist.",
                    HttpResponseCode = 404
                };
            }
            else
            {
                var itemToPatch = _mapper.Map<TUpdateDto>(item);
                patchDocument.ApplyTo(itemToPatch);
                _mapper.Map(itemToPatch, item);
                await _basicRepository.SaveChangesAsync();
                return new OperationResult<TDto>
                {
                    IsSuccess = true,
                    HttpResponseCode = 204
                };
            }
        }

        public async Task<(bool,TEntity?)> CheckIfIdExistsAsync(int id)
        {
            return await _basicRepository.CheckIfIdExistsAsync(id);
        }
        public virtual async Task<OperationResult<TDto>> DeleteByIdAsync(int id)
        {
            (bool exists, TEntity ? entity) result = await _basicRepository.CheckIfIdExistsAsync(id);
            if (result.exists == false)
            {
                return new OperationResult<TDto>
                {
                    IsSuccess = false,
                    ErrorMessage = $"Entity of type {typeof(TEntity).Name} with id={id} does not exist.",
                    HttpResponseCode = 404
                };
            }
            else
            {
                _basicRepository.DeleteAsync(result.entity);
                await _basicRepository.SaveChangesAsync();
                return new OperationResult<TDto>
                {
                    IsSuccess = true,
                    HttpResponseCode = 204
                };
            }
        }



    }
}
