using LightBDD.Core.ExecutionContext;

namespace LightBDD.XUnit2.Implementation.Customization
{
    internal class AssemblySettings
    {
        private static readonly AsyncLocalContext<AssemblySettings> AsyncLocal = new AsyncLocalContext<AssemblySettings>();

        public static AssemblySettings Current => AsyncLocal.Value ?? new AssemblySettings();
        public static void SetSettings(AssemblySettings settings) => AsyncLocal.Value = settings;

        public bool EnableInterClassParallelization { get; set; }
        public bool UseXUnitSkipBehavior { get; set; }
    }
}