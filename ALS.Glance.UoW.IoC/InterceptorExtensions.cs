using System;
using System.Threading;
using System.Threading.Tasks;
using ALS.Glance.UoW.Core;

namespace ALS.Glance.UoW.IoC
{
    public static class InterceptorExtensions
    {
        public static Task InjectTransactionalInterceptCodeAndReturnNewTask(
            this Task task, IUnitOfWork uow, Action preReturnCode, Action finalizationCode, CancellationToken ct)
        {
            return
                task
                    .ContinueWith(
                        async t =>
                        {
                            try
                            {
                                t.ThrowIfFaulted<Exception>();
                                await task;
                                await uow.CommitAsync(ct);
                                if (preReturnCode != null)
                                    preReturnCode();
                            }
                            finally
                            {
                                if (finalizationCode != null)
                                    finalizationCode();
                            }
                        }, ct).Unwrap();
        }

        public static Task<TResult> InjectTransactionalInterceptCodeAndReturnNewTask<TResult>(
            this Task task, IUnitOfWork uow, Action preReturnCode, Action finalizationCode, CancellationToken ct)
        {
            var taskCasted = (Task<TResult>)task;
            return
                taskCasted
                    .ContinueWith(
                        async t =>
                        {
                            try
                            {
                                t.ThrowIfFaulted<Exception>();
                                var result = await taskCasted;
                                await uow.CommitAsync(ct);
                                if (preReturnCode != null)
                                    preReturnCode();
                                return result;
                            }
                            finally
                            {
                                if (finalizationCode != null)
                                    finalizationCode();
                            }
                        }, ct).Unwrap();
        }

        public static void ThrowIfFaulted<TException>(this Task task)
            where TException : Exception, new()
        {
            if (!task.IsFaulted) return;

            if (task.Exception == null)
                throw new TException();
            throw task.Exception;
        }
    }
}