using AsyncEnumerable.Spikes.SerializationDeserialization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Benchmarks;

[MemoryDiagnoser]
public class DeserializeAsyncVsDeserializeAsyncEnumerable {
    
    [Params(
        TotalRecords.One,
        TotalRecords.Ten, 
        TotalRecords.OneThousand,
        TotalRecords.TenMillion,
        TotalRecords.OneHundredMillion

        )] 
    public int Counts { get; set; }
    
    [Benchmark(Baseline = true)]
    public Task<int> DeserializeAsync() {
        return CustomerSerializationTests.DeserializeAsync(Counts);
    }
    
    [Benchmark]
    public Task<int> DeserializeAsyncEnumerable() {
        return CustomerSerializationTests.DeserializeAsyncEnumerable(Counts);
    }
}

class Program {
    static void Main(string[] args) {
        Summary summary = BenchmarkRunner.Run<DeserializeAsyncVsDeserializeAsyncEnumerable>();
    }
}