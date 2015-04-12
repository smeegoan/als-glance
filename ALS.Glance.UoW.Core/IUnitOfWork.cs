using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ALS.Glance.UoW.Core.Exceptions;

namespace ALS.Glance.UoW.Core
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Prepares the <see cref="IUnitOfWork"/> to work
        /// </summary>
        void Begin();

        /// <summary>
        /// Asynchronously prepares the <see cref="IUnitOfWork"/> to work
        /// </summary>
        /// <param name="ct">The cancelation token</param>
        /// <returns>The task to be awaited</returns>
        Task BeginAsync(CancellationToken ct);

        /// <summary>
        /// Commit the work made by this <see cref="IUnitOfWork"/>
        /// </summary>
        /// <exception cref="ConcurrencyException">
        ///     Thrown when the work can't be commited due to concurrency conflicts
        /// </exception>
        /// <exception cref="CommitException"/>
        void Commit();

        /// <summary>
        /// Asynchronously commit the work made by this <see cref="IUnitOfWork"/>
        /// </summary>
        /// <param name="ct">The cancelation token</param>
        /// <returns>The task to be awaited</returns>
        /// <exception cref="ConcurrencyException">
        ///     Thrown when the work can't be commited due to concurrency conflicts
        /// </exception>
        /// <exception cref="CommitException"/>
        Task CommitAsync(CancellationToken ct);

        /// <summary>
        /// Executes the query async based on the underline persistent provider
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<T[]> ExecuteQueryAsync<T>(IQueryable<T> query, CancellationToken ct);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<T> ExecuteQueryAndGetFirstOrDefaultAsync<T>(IQueryable<T> query, CancellationToken ct);

        /// <summary>
        /// Executes work async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="work"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<T> ExecuteWorkAsync<T>(Func<T> work, CancellationToken ct);

        /// <summary>
        /// Executes work async
        /// </summary>
        /// <param name="work"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task ExecuteWorkAsync(Action work, CancellationToken ct);
    }
}