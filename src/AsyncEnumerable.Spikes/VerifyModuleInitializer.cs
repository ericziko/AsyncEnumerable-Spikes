using System.Runtime.CompilerServices;

namespace AsyncEnumerable.Spikes;

public static class VerifyModuleInitializer {
    [ModuleInitializer]
    public static void Initialize() {
        VerifyDiffPlex.Initialize();
    }
}