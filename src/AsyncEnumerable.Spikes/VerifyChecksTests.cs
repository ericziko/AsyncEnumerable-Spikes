namespace AsyncEnumerable.Spikes;

[TestFixture]
public class VerifyChecksTests {
    [Test]
    public Task Run() {
        return VerifyChecks.Run();
    }
}