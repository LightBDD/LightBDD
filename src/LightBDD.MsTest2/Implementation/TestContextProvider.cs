using System.Reflection;
using LightBDD.Framework.ExecutionContext;

namespace LightBDD.MsTest2.Implementation
{
    internal class TestContextProvider
#if !NETSTANDARD1_3
        : System.MarshalByRefObject
#endif
    {
        private static readonly AsyncLocalContext<TestContextProvider> Provider = new AsyncLocalContext<TestContextProvider>();
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