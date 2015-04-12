using ALS.Glance.UoW.Core;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ALS.Glance.UoW.Core.Exceptions;

namespace ALS.Glance.UoW.EF
{
    public abstract class EFUnitOfWork : ScopeEnabledUnitOfWork, IEFUnitOfWork
    {
        private static readonly Task<int> BeginTask = Task.FromResult(0);
        private readonly DbContext _context;

        protected EFUnitOfWork(DbContext context)
        {
            _context = context;
        }

        ~EFUnitOfWork()
        {
            Dispose(false);
        }

        #region IEFUnitOfWork

        public DbContext Context
        {
            get { return _context; }
        }

        public override async Task<T[]> ExecuteQueryAsync<T>(IQueryable<T> query, CancellationToken ct)
        {
            return await query.ToArrayAsync(ct);
        }

        public override async Task<T> ExecuteQueryAndGetFirstOrDefaultAsync<T>(IQueryable<T> query, CancellationToken ct)
        {
            return await query.FirstOrDefaultAsync(ct);
        }

        public override Task<T> ExecuteWorkAsync<T>(Func<T> work, CancellationToken ct)
        {
            return Task.Factory.StartNew(work, ct);
        }

        public override Task ExecuteWorkAsync(Action work, CancellationToken ct)
        {
            return Task.Factory.StartNew(work, ct);
        }

        #endregion

        #region ScopeEnabledUnitOfWork

        protected override void OnBegin()
        {

        }

        protected override Task OnBeginAsync(CancellationToken ct)
        {
            return BeginTask;
        }

        protected override void OnCommit()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new ConcurrencyException(e);
            }
        }

        protected override async Task OnCommitAsync(CancellationToken ct)
        {
            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new ConcurrencyException(e);
            }
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _context.Dispose();
        }

        #endregion
    }
}