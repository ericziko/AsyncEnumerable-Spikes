using System.Text.Json.Serialization;

namespace AsyncEnumerable.Spikes.SerializationDeserialization;

[JsonSerializable(typeof(Customer))]
[JsonSourceGenerationOptions(
    DefaultBufferSize = 32 * 1024,
    PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified,
    WriteIndented = false,
    AllowTrailingCommas = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
internal partial class CustomerJsonContext : JsonSerializerContext;