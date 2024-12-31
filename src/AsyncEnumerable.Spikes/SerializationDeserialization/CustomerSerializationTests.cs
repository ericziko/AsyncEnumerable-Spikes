using System.Text.Json;
using System.Text.Json.Serialization;

namespace AsyncEnumerable.Spikes.SerializationDeserialization;

[TestFixture]
public class CustomerSerializationTests {

    [Test]
    public Task Should_be_able_to_serialize_10_customers() {
        var maxCount = 10;
        var customers = Enumerable.Range(1, maxCount)
            .Select(i => new Customer($"First {i}", $"Last {i}")).ToList();
        var json = JsonSerializer.Serialize(customers, options: new JsonSerializerOptions {
            WriteIndented = true
        });
        var fileName = $"{maxCount}_customers.json";
        var filePath = CurrentFile.Relative(Path.Combine("TestData", fileName));
        File.WriteAllText(filePath, json);
        var result = File.ReadAllText(filePath);
        return Verify(result);
    }

    [Test]
    public Task Should_be_able_to_deserialize_10_customers() {
        var maxCount = 10;
        var fileName = $"{maxCount}_customers.json";
        var filePath = CurrentFile.Relative(Path.Combine("TestData", fileName));
        using var fs = new FileStream(filePath, FileMode.Open);
        var customers = JsonSerializer.DeserializeAsync<Customer[]>(fs);
        return Verify(customers);
    }

    [Test]
    public async Task Should_be_able_to_serialize_and_deserialize_six_customers() {
        int six = 6;
        var maxCount = six;
        var fileName = $"{maxCount}_customers.json";
        var filePath = CurrentFile.Relative(Path.Combine("TestData", fileName));

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

    readonly JsonSerializerOptions _jsonSerializerOptions = new() {
        WriteIndented = false,
        DefaultBufferSize = 32 * 1024,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    [TestCase(10)]
    [TestCase(10_000_000)]
    [TestCase(100_000_000)]
    public async Task Should_be_able_to_serialize_and_deserialize(int maxCount) {
        var fileName = $"{maxCount}_customers.json";
        var filePath = CurrentFile.Relative(Path.Combine("TestData", fileName));
        var customers = Enumerable.Range(1, maxCount)
            .Select(i => new Customer($"First {i}", $"Last {i}"));

        await using var createStream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(
            createStream,
            customers,
            typeof(IEnumerable<Customer>),
            CustomerListJsonContext.Default
        );
    }


}