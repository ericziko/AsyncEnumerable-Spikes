using System.Text.Json.Serialization;

namespace AsyncEnumerable.Spikes;

public class Customer(string firstName, string lastName) {
    public string FirstName { get; init; } = firstName;
    public string LastName { get; init; } = lastName;

    public string WholeName => $"{FirstName} {LastName}";
}


// https://dotnettips.wordpress.com/2023/03/09/microsoft-net-source-generators-speeding-up-json-serialization/


[JsonSerializable(typeof(Customer))]
[JsonSourceGenerationOptions(DefaultBufferSize = 32*1024)]
internal partial class CustomerJsonContext : JsonSerializerContext;

[JsonSourceGenerationOptions(DefaultBufferSize = 32*1024)]
[JsonSerializable(typeof(IEnumerable<Customer>))]
internal partial class CustomerListJsonContext : JsonSerializerContext;
