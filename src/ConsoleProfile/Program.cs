using AsyncEnumerable.Spikes.SerializationDeserialization;

namespace ConsoleProfile;

class Program {
    static async Task Main(string[] args) {
        Console.WriteLine("Hello, World!");
        // await new CustomerSerializationTests().Deserialization_Memory_Spike(10_000_000);
        await new CustomerSerializationTests().Deserialization_Memory_Spike_V2(10_000_000);
    }
}