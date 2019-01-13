using System.Reflection;
using LightBDD.Core.ExecutionContext;

namespace LightBDD.NUnit3.Implementation
{
    internal class TestContextProvider
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