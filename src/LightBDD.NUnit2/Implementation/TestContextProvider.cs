using System.Reflection;
using LightBDD.Framework.ExecutionContext;

namespace LightBDD.NUnit2.Implementation
{
    internal class TestContextProvider
    {
        private static readonly AsyncLocalContext<TestContextProvider> _provider = new AsyncLocalContext<TestContextProvider>();
        public MethodInfo TestMethod { get; }

        public static TestContextProvider Current => _provider.Value;

        public static void Initialize(MethodInfo testMethod)
        {
            _provider.Value = new TestContextProvider(testMethod);
        }

        public static void Clear()
        {
            _provider.Value = null;
        }

        private TestContextProvider(MethodInfo testMethod)
        {
            TestMethod = testMethod;
        }
    }
}