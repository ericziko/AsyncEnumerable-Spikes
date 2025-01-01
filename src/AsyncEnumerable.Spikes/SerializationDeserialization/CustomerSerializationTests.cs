using System.Text.Json;

namespace AsyncEnumerable.Spikes.SerializationDeserialization;

[TestFixture]
public class CustomerSerializationTests {
    private const int One = 1;
    private const int Ten = 10;
    private const int OneHundredMillion = 100_000_000;
    private const int TenMillion = 10_000_000;
    
    [Test, Explicit]
    public Task Should_be_able_to_serialize_10_customers() {
        var maxCount = Ten;
        var customers = Enumerable.Range(1, maxCount)
            .Select(i => new Customer($"First {i}", $"Last {i}")).ToList();
        var json = JsonSerializer.Serialize(customers, options: new JsonSerializerOptions {
            WriteIndented = true
        });
        var filePath = GetFilePath(maxCount);
        File.WriteAllText(filePath, json);
        var result = File.ReadAllText(filePath);
        return Verify(result);
    }


    [TestCase(One)] // Creates a file that is 74 bytes in milliseconds
    [TestCase(Ten)] // Creates a file that is 735 byes in milliseconds 
    [TestCase(TenMillion)] // Creates file that is 965.6 MB in 3 seconds on Mac Mini
    [TestCase(OneHundredMillion)] // Creates file that is 10.06 GB in 23 seconds on Mac Mini
    [Explicit]
    public async Task Should_be_able_to_serialize(int maxCount) {
        var filePath = GetFilePath(maxCount);
        var customers = Enumerable.Range(1, maxCount)
            .Select(i => new Customer($"First {i}", $"Last {i}"));

        await using var fileStream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(
            fileStream,
            customers,
            typeof(IEnumerable<Customer>),
            CustomerListJsonContext.Default
        );
    }
    [TestCase(Ten)] // Deserializes in 8ms on Mac mini
    [TestCase(TenMillion)] // Deserializes in 8 seconds on Mac mini 
    [TestCase(OneHundredMillion)] // Deserializes in 8 seconds on Mac mini 
    public async Task Deserialization_Memory_Spike(int maxCount) {
        var filePath = GetFilePath(maxCount);
        await using var fileStream = File.OpenRead(filePath);
        var customers = await JsonSerializer
            .DeserializeAsync(fileStream, CustomerListJsonContext.Default.IEnumerableCustomer);
        int count = 0;
        foreach (var unused in customers!) {
            count++;
        }
        Console.WriteLine($"Deserialize Customers Count: {count}");
    }

    [TestCase(Ten)] // Deserializes in 8ms on Mac mini
    [TestCase(TenMillion)] // Deserializes in 8 seconds on Mac mini 
    [TestCase(OneHundredMillion)] // Deserializes in 32 seconds on Mac mini
    public async Task Deserialization_Memory_Spike_V2(int maxCount) {
        var filePath = GetFilePath(maxCount);
        await using var fileStream = File.OpenRead(filePath);
        var customers = JsonSerializer
            .DeserializeAsyncEnumerable(fileStream, CustomerJsonContext.Default.Customer);
        int count = 0;
        await foreach (var unused in customers) {
            count++;
        }
        Console.WriteLine($"DeserializeAsyncEnumerable Customers Count: {count:N0}");
    }

    // ReSharper disable once UnusedMember.Global
    public static void DeserializeEnumerable() {
        var task = new CustomerSerializationTests().Deserialization_Memory_Spike(OneHundredMillion);
        Task.WaitAll(task);
    }
    
    // ReSharper disable once UnusedMember.Global
    public static void Deserialize_Async_Enumerable() {
        var task = new CustomerSerializationTests().Deserialization_Memory_Spike_V2(OneHundredMillion);
        Task.WaitAll(task);
    }

    private static string GetFilePath(int maxCount) {
        var fileName = $"{maxCount}_customers.json";
        var filePath = CurrentFile.Relative(Path.Combine("TestData", fileName));
        return filePath;
    }
}