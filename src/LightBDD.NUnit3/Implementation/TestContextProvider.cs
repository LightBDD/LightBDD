using System.Reflection;
using LightBDD.Framework.ExecutionContext;

namespace LightBDD.NUnit3.Implementation
{
    internal class TestContextProvider
#if !NETSTANDARD1_6
        : System.MarshalByRefObject
#endif
    {
        private static readonly AsyncLocalContext<TestContextProvider> Provider = new AsyncLocalContext<TestContextProvider>();
        public  MethodInfo TestMethod { get; }
        public  object[] TestMethodArguments { get; }

        public static TestContextProvider Current => Provider.Value;

        public static void Initialize(MethodInfo testMethod, object[] testArguments)
        {
            Provider.Value = new TestContextProvider(testMethod,testArguments);
        }

        public static void Clear()
        {
            Provider.Value = null;
        }

        private TestContextProvider(MethodInfo testMethod, object[] testMethodArguments)
        {
            TestMethod = testMethod;
            TestMethodArguments = testMethodArguments;
        }
    }
}