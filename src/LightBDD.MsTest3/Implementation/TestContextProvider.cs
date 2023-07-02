using System.Reflection;
using System.Threading;

namespace LightBDD.MsTest3.Implementation
{
    internal class TestContextProvider
    {
        private static readonly AsyncLocal<TestContextProvider> Provider = new();
        public MethodInfo TestMethod { get; }
        public object[] TestMethodArguments { get; }

        public static TestContextProvider Current => Provider.Value;

        public static void Initialize(MethodInfo testMethod, object[] arguments)
        {
            Provider.Value = new TestContextProvider(testMethod, arguments);
        }

        public static void Clear()
        {
            Provider.Value = null;
        }

        private TestContextProvider(MethodInfo testMethod, object[] arguments)
        {
            TestMethod = testMethod;
            TestMethodArguments = arguments;
        }
    }
}