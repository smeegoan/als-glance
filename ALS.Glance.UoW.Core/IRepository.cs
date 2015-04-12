using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ALS.Glance.UoW.Core
{
    public interface IRepository<TEntity, in TKey> where TEntity : class
    {
        IQueryable<TEntity> GetAll();

        IQueryable<TEntity> GetAll<T>(int pageSize, int pageNumber, Expression<Func<TEntity, T>> order);

        IQueryable<TEntity> GetAllAndFetch(params Expression<Func<TEntity, object>>[] propertiesToFetch);

        TEntity GetById(TKey id);

        Task<TEntity> GetByIdAsync(TKey id, CancellationToken ct);

        TEntity Add(TEntity entity);

        Task<TEntity> AddAsync(TEntity entity, CancellationToken ct);

        IEnumerable<TEntity> Add(IEnumerable<TEntity> entities);

        Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities, CancellationToken ct);

        TEntity Update(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken ct);

        IEnumerable<TEntity> Update(IEnumerable<TEntity> entities);

        Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken ct);

        TEntity Delete(TEntity entity);

        Task<TEntity> DeleteAsync(TEntity entity, CancellationToken ct);

        IEnumerable<TEntity> Delete(IEnumerable<TEntity> entities);

        Task<IEnumerable<TEntity>> DeleteAsync(IEnumerable<TEntity> entities, CancellationToken ct);

        long Total();

        Task<long> TotalAsync(CancellationToken ct);
    }
}