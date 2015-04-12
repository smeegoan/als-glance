using ALS.Glance.UoW.Core.Exceptions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ALS.Glance.UoW.Core
{
    public abstract class ScopeEnabledUnitOfWork : IUnitOfWork
    {
        private int _currentScope;
        // ReSharper disable once UnusedMember.Local
        private readonly Guid _privateId = Guid.NewGuid();

        public void Begin()
        {
            var s = IncrementScope();
            if (s == 1)
                OnBegin();
        }

        public async Task BeginAsync(CancellationToken ct)
        {
            var s = IncrementScope();
            if (s == 1)
                await OnBeginAsync(ct);
        }

        public void Commit()
        {
            var s = DecrementScope();
            if (s < 0)
                throw new UndefinedScopeException();
            if (s != 0) return;

            try
            {
                OnCommit();
            }
            catch (UnitOfWorkException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CommitException(e);
            }
        }

        public async Task CommitAsync(CancellationToken ct)
        {
            var s = DecrementScope();
            if (s < 0)
                throw new UndefinedScopeException();
            if (s != 0) return;

            try
            {
                await OnCommitAsync(ct);
            }
            catch (UnitOfWorkException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CommitException(e);
            }
        }

        public abstract Task<T[]> ExecuteQueryAsync<T>(IQueryable<T> query, CancellationToken ct);
        public abstract Task<T> ExecuteQueryAndGetFirstOrDefaultAsync<T>(IQueryable<T> query, CancellationToken ct);
        public abstract Task<T> ExecuteWorkAsync<T>(Func<T> work, CancellationToken ct);
        public abstract Task ExecuteWorkAsync(Action work, CancellationToken ct);

        protected abstract void OnBegin();
        protected abstract Task OnBeginAsync(CancellationToken ct);
        protected abstract void OnCommit();
        protected abstract Task OnCommitAsync(CancellationToken ct);

        public abstract void Dispose();

        private int DecrementScope()
        {
            return Interlocked.Decrement(ref _currentScope);
        }

        private int IncrementScope()
        {
            return Interlocked.Increment(ref _currentScope);
        }
    }
}