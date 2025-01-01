using System.Text.Json;
using JetBrains.Annotations;

namespace AsyncEnumerable.Spikes.SerializationDeserialization;

[TestFixture]
public class CustomerSerializationTests {

    [Test, Explicit]
    public Task Should_be_able_to_serialize_10_customers() {
        var maxCount = 10;
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

    [Test, Explicit]
    public Task Should_be_able_to_deserialize_10_customers() {
        var maxCount = 10;
        var filePath = GetFilePath(maxCount);
        using var fs = new FileStream(filePath, FileMode.Open);
        var customers = JsonSerializer.DeserializeAsync<Customer[]>(fs);
        return Verify(customers);
    }

    [Test, Explicit]
    public async Task Should_be_able_to_serialize_and_deserialize_six_customers() {
        int six = 6;
        var maxCount = six;
        var filePath = GetFilePath(maxCount);

        var customers = Enumerable.Range(1, maxCount)
            .Select(i => new Customer($"First {i}", $"Last {i}"));

        var jsonSerializerOptions = new JsonSerializerOptions {
            WriteIndented = true
        };

        await using (var createStream = File.Create(filePath)) {
            await JsonSerializer.SerializeAsync(createStream, customers, jsonSerializerOptions);
        }

        var result = await File.ReadAllTextAsync(filePath);
        await Verify(result);
    }

    [TestCase(1)] // Creates a file that is 74 bytes in milliseconds
    [TestCase(10)] // Creates a file that is 735 byes in milliseconds 
    [TestCase(10_000_000)] // Creates file that is 965.6 MB in 3 seconds on Mac Mini
    [TestCase(100_000_000)] // Creates file that is 10.06 GB in 23 seconds on Mac Mini
    [Explicit]
    public async Task Should_be_able_to_serialize_and_deserialize(int maxCount) {
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
    [TestCase(10)] // Deserializes in 8ms on Mac mini
    [TestCase(10_000_000)] // Deserializes in 8 seconds on Mac mini 
    public async Task Deserialization_Memory_Spike(int maxCount) {
        var filePath = GetFilePath(maxCount);
        await using var fileStream = File.OpenRead(filePath);
        var customers = await JsonSerializer
            .DeserializeAsync(fileStream, CustomerListJsonContext.Default.IEnumerableCustomer);
        Console.WriteLine($"Deserialize Customers Count: {customers!.ToList().Count:N0}");
    }

    [TestCase(10)] // Deserializes in 8ms on Mac mini
    [TestCase(10_000_000)] // Deserializes in 8 seconds on Mac mini 
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
        var task = new CustomerSerializationTests().Deserialization_Memory_Spike(10_000_000);
        Task.WaitAll(task);
    }
    
    // ReSharper disable once UnusedMember.Global
    public static void Deserialize_Async_Enumerable() {
        var task = new CustomerSerializationTests().Deserialization_Memory_Spike_V2(10_000_000);
        Task.WaitAll(task);
    }

    private static string GetFilePath(int maxCount) {
        var fileName = $"{maxCount}_customers.json";
        var filePath = CurrentFile.Relative(Path.Combine("TestData", fileName));
        return filePath;
    }


}