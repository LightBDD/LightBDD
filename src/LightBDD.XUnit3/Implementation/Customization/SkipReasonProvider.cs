using System.Threading;

namespace LightBDD.XUnit3.Implementation.Customization
{
    internal static class SkipReasonProvider
    {
        private static readonly AsyncLocal<string> Value = new();

        public static string Current
        {
            get => Value.Value;
            set => Value.Value = value;
        }
    }
}
