using System.Reflection;
using System.Threading;

namespace LightBDD.NUnit3.Implementation
{
    internal class TestContextProvider
    {
        private static readonly AsyncLocal<TestContextProvider> Provider = new AsyncLocal<TestContextProvider>();
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