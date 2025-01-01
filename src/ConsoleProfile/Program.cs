using AsyncEnumerable.Spikes.SerializationDeserialization;

namespace ConsoleProfile;

internal static class Program {
    static async Task Main() {
        // await new CustomerSerializationTests().Deserialization_with_DeserializeAsync_Spike(10_000_000);
        await new CustomerSerializationTests().Deserialization_with_DeserializeAsyncEnumerable_Spike(10_000_000);
    }
}