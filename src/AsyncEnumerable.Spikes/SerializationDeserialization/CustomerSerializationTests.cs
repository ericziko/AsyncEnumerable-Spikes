using System.Text.Json;

namespace AsyncEnumerable.Spikes;

[TestFixture]
public class CustomerSerializationTests {

    [Test]
    public Task Should_be_able_to_serialize_10_customers() {
        var maxCount = 10;
        var customers = Enumerable.Range(1, maxCount).Select(i => new Customer($"First {i}", $"Last {i}")).ToList();
        // Serialize customers to file named customers.json in current directory
        var json = JsonSerializer.Serialize(customers,new JsonSerializerOptions {
            WriteIndented = true
        });
        var fileName = $"{maxCount}_customers.json";
        File.WriteAllText(fileName, json);
        var result = File.ReadAllText(fileName);
        return Verify(result);
    }

    [Test]
    public void What_directory_am_I_in() {
        Console.WriteLine(Environment.CurrentDirectory);
        Console.WriteLine(CurrentFile.Directory());
    }
    
}