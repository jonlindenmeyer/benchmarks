using BenchmarkDotNet.Attributes;
using Farfetch.Inventory.StockDomainManager.Infrastructure.CrossCutting.Extensions;
using AutoFixture;

namespace ForEachBenchmark
{
    [MemoryDiagnoser]
    public class BenchmarkExec
    {
        private readonly Fixture fixture = new Fixture();
        private readonly List<int> list = new List<int>();

        public BenchmarkExec()
        {
            list = fixture.CreateMany<int>(1000).ToList();
        }

        [Benchmark]
        public async Task ExecuteForEach()
        {
            foreach (var item in list)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1));
            }
        }

        [Benchmark]
        public async Task ExecuteForEachAsyncExtension()
        {
            await list.ForeachAsync(
                async item =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1));
                });
        }

        [Benchmark]
        public async Task ExecuteParallelForeach()
        {
            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = 10
            };

            var ct = new CancellationToken();

            await Parallel.ForEachAsync(list, parallelOptions, async (item, ct) =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1));
            });
        }
    }
}
