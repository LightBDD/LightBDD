#nullable enable
using System;
using System.Threading;

namespace LightBDD.XUnit2.Implementation.Customization
{
    internal class AssemblySettings
    {
        private static readonly AsyncLocal<AssemblySettings> AsyncLocal = new();

        public static AssemblySettings Current => AsyncLocal.Value ?? new AssemblySettings();
        public static void SetSettings(AssemblySettings settings) => AsyncLocal.Value = settings;

        public bool EnableInterClassParallelization { get; set; }
        public bool UseXUnitSkipBehavior { get; set; }
        public Exception? SetUpException { get; set; }
    }
}