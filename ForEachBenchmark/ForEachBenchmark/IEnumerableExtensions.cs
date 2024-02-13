namespace Farfetch.Inventory.StockDomainManager.Infrastructure.CrossCutting.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    public static class IEnumerableExtensions
    {
        public const int DefaultMaxDegreeOfParallelism = 10;

        public static async Task ForeachAsync<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, Task> selector,
            int maxDegreeOfParallelism = DefaultMaxDegreeOfParallelism,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var semaphore = new SemaphoreSlim(Math.Max(maxDegreeOfParallelism, 1)))
            {
                var tasks = new List<Task>();

                foreach (var item in source)
                {
                    try
                    {
                        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }

                    tasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            await selector(item).ConfigureAwait(false);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }));
                }

                var task = Task.WhenAll(tasks);
                try
                {
                    await task.ConfigureAwait(false);
                }
                catch
                {
                    throw task.Exception;
                }
            }
        }

        public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, Task<TResult>> selector,
            int maxDegreeOfParallelism = DefaultMaxDegreeOfParallelism,
            CancellationToken cancellationToken = default)
        {
            using (var semaphore = new SemaphoreSlim(Math.Max(maxDegreeOfParallelism, 1)))
            {
                var tasks = new List<Task<TResult>>();

                foreach (var item in source)
                {
                    try
                    {
                        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }

                    tasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            return await selector(item).ConfigureAwait(false);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }));
                }

                var task = Task.WhenAll(tasks);
                try
                {
                    return await task.ConfigureAwait(false);
                }
                catch
                {
                    throw task.Exception;
                }
            }
        }
    }
}
