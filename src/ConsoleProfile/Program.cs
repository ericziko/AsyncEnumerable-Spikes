using AsyncEnumerable.Spikes.SerializationDeserialization;

namespace ConsoleProfile;

internal static class Program {
    static async Task Main() {
        await new CustomerSerializationTests().DeserializeAsyncEnumerable_Test(10_000_000);
    }
}