using ALS.Glance.UoW.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ALS.Glance.UoW.EF
{
    public class EFRepository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class
    {
        protected DbContext DbContext { get; private set; }
        protected DbSet<TEntity> DbSet { get; private set; }

        public EFRepository(DbContext dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException("dbContext");

            DbContext = dbContext;
            DbSet = dbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }

        public IQueryable<TEntity> GetAll<T>(int pageSize, int pageNumber, Expression<Func<TEntity, T>> order)
        {
            return
                DbSet
                    .OrderBy(order)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);
        }

        public IQueryable<TEntity> GetAllAndFetch(params Expression<Func<TEntity, object>>[] propertiesToFetch)
        {
            return
                propertiesToFetch
                    .Aggregate<Expression<Func<TEntity, object>>, IQueryable<TEntity>>(
                        DbSet, (current, expression) => current.Include(expression));
        }

        public TEntity GetById(TKey id)
        {
            return DbSet.Find(id);
        }

        public Task<TEntity> GetByIdAsync(TKey id, CancellationToken ct)
        {
            return DbSet.FindAsync(ct, id);
        }

        public TEntity Add(TEntity entity)
        {
            var dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State == EntityState.Detached)
                return DbSet.Add(entity);

            dbEntityEntry.State = EntityState.Added;
            return dbEntityEntry.Entity;
        }

        public Task<TEntity> AddAsync(TEntity entity, CancellationToken ct)
        {
            return Task.Factory.StartNew(() => Add(entity), ct);
        }

        public IEnumerable<TEntity> Add(IEnumerable<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException("entities");
            return entities.Select(Add);
        }

        public Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities, CancellationToken ct)
        {
            return Task.Factory.StartNew(() => Add(entities), ct);
        }

        public TEntity Update(TEntity entity)
        {
            var dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State == EntityState.Detached)
                DbSet.Attach(entity);
            dbEntityEntry.State = EntityState.Modified;

            return dbEntityEntry.Entity;
        }

        public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken ct)
        {
            return Task.Factory.StartNew(() => Update(entity), ct);
        }

        public IEnumerable<TEntity> Update(IEnumerable<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException("entities");
            return entities.Select(Update);
        }

        public Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken ct)
        {
            return Task.Factory.StartNew(() => Update(entities), ct);
        }

        public TEntity Delete(TEntity entity)
        {
            var dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Deleted)
            {
                dbEntityEntry.State = EntityState.Deleted;
                return dbEntityEntry.Entity;
            }

            DbSet.Attach(entity);
            DbSet.Remove(entity);

            return entity;
        }

        public Task<TEntity> DeleteAsync(TEntity entity, CancellationToken ct)
        {
            return Task.Factory.StartNew(() => Delete(entity), ct);
        }

        public IEnumerable<TEntity> Delete(IEnumerable<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException("entities");
            return entities.Select(Delete);
        }

        public Task<IEnumerable<TEntity>> DeleteAsync(IEnumerable<TEntity> entities, CancellationToken ct)
        {
            return Task.Factory.StartNew(() => Update(entities), ct);
        }

        public long Total()
        {
            return DbSet.LongCount();
        }

        public async Task<long> TotalAsync(CancellationToken ct)
        {
            return await DbSet.LongCountAsync(ct);
        }
    }
}