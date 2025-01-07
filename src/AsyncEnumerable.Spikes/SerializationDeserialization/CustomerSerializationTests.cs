using System.Text.Json;

namespace AsyncEnumerable.Spikes.SerializationDeserialization;

[TestFixture]
public class CustomerSerializationTests {
    // ReSharper disable MemberCanBePrivate.Global
    // ReSharper restore MemberCanBePrivate.Global


    [TestCase(TotalRecords.One)] // Creates a file that is 74 bytes in milliseconds
    [TestCase(TotalRecords.Ten)] // Creates a file that is 735 byes in milliseconds 
    [TestCase(TotalRecords.OneThousand)] // Creates file that is 965.6 MB in 3 seconds on Mac Mini
    [TestCase(TotalRecords.TenMillion)] // Creates file that is 965.6 MB in 3 seconds on Mac Mini
    [TestCase(TotalRecords.OneHundredMillion)] // Creates file that is 10.06 GB in 23 seconds on Mac Mini
    [Explicit]
    public async Task Should_be_able_to_serialize(int maxCount) {
        var filePath = GetFilePath(maxCount);
        if (File.Exists(filePath)) {
            return;
        }
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
    [TestCase(TotalRecords.Ten)] // Deserializes in 8ms on Mac mini
    [TestCase(TotalRecords.OneThousand)] // Deserializes in 32 seconds on Mac mini
    [TestCase(TotalRecords.TenMillion)] // Deserializes in 8 seconds on Mac mini 
    [TestCase(TotalRecords.OneHundredMillion)] // Deserializes in 8 seconds on Mac mini 
    [Explicit]
    public async Task DeserializeAsync_Test(int totalRecords) {
        int result = await DeserializeAsync(totalRecords);
        Assert.That(result, Is.EqualTo(totalRecords));
    }
    public static async Task<int> DeserializeAsync(int totalRecords) {
        var filePath = GetFilePath(totalRecords);
        await using var fileStream = File.OpenRead(filePath);
        var customers = await JsonSerializer
            .DeserializeAsync(fileStream, CustomerListJsonContext.Default.IEnumerableCustomer);
        int count = 0;
        foreach (var unused in customers!) {
            count++;
        }
        return count;
    }

    [TestCase(TotalRecords.Ten)] // Deserializes in 8ms on Mac mini
    [TestCase(TotalRecords.OneThousand)] // Deserializes in 32 seconds on Mac mini
    [TestCase(TotalRecords.TenMillion)] // Deserializes in 8 seconds on Mac mini 
    [TestCase(TotalRecords.OneHundredMillion)] // Deserializes in 32 seconds on Mac mini
    [Explicit]
    public async Task DeserializeAsyncEnumerable_Test(int totalRecords) {
        var count = await DeserializeAsyncEnumerable(totalRecords);
        Assert.That(count, Is.EqualTo(totalRecords));
    }
    public static async Task<int> DeserializeAsyncEnumerable(int totalRecords) {
        var filePath = GetFilePath(totalRecords);
        await using var fileStream = File.OpenRead(filePath);
        var customers = JsonSerializer
            .DeserializeAsyncEnumerable(fileStream, CustomerJsonContext.Default.Customer);
        int count = 0;
        await foreach (var unused in customers) {
            count++;
        }
        return count;
    }

    private static string GetFilePath(int totalRecords) {
        var fileName = $"{totalRecords}_customers.json";
        var filePath = CurrentFile.Relative(Path.Combine("TestData", fileName));
        return filePath;
    }
}