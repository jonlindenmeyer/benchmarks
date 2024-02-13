```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.3803/22H2/2022Update)
Intel Core i7-10510U CPU 1.80GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.101
  [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2


```
| Method                       | Mean     | Error    | StdDev   | Allocated |
|----------------------------- |---------:|---------:|---------:|----------:|
| ExecuteForEach               | 15.644 s | 0.0225 s | 0.0210 s | 255.59 KB |
| ExecuteForEachAsyncExtension |  1.563 s | 0.0127 s | 0.0119 s | 652.74 KB |
| ExecuteParallelForeach       |  1.563 s | 0.0074 s | 0.0069 s | 275.09 KB |
