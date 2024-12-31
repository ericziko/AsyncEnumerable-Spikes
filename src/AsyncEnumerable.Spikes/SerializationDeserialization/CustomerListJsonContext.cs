using System.Text.Json.Serialization;

namespace AsyncEnumerable.Spikes.SerializationDeserialization;

[JsonSourceGenerationOptions(
    DefaultBufferSize = 32 * 1024,
    PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified,
    WriteIndented = false,
    AllowTrailingCommas = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
[JsonSerializable(typeof(IEnumerable<Customer>))]
internal partial class CustomerListJsonContext : JsonSerializerContext;