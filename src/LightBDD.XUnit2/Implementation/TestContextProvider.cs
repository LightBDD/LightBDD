using System.Reflection;
using LightBDD.Core.ExecutionContext;
using Xunit.Abstractions;

namespace LightBDD.XUnit2.Implementation
{
    internal class TestContextProvider
    {
        private static readonly AsyncLocalContext<TestContextProvider> Provider = new AsyncLocalContext<TestContextProvider>();
        public MethodInfo TestMethod { get; }
        public object[] TestMethodArguments { get; }
        public ITestOutputHelper OutputHelper { get; }
        public string SkipReason { get; }

        public static TestContextProvider Current => Provider.Value;

        public static void Initialize(MethodInfo testMethod, object[] arguments, ITestOutputHelper testOutputHelper,
            string skipReason)
        {
            Provider.Value = new TestContextProvider(testMethod, arguments, testOutputHelper,skipReason);
        }

        public static void Clear()
        {
            Provider.Value = null;
        }

        private TestContextProvider(MethodInfo testMethod, object[] arguments, ITestOutputHelper outputHelper,
            string skipReason)
        {
            TestMethod = testMethod;
            TestMethodArguments = arguments;
            OutputHelper = outputHelper;
            SkipReason = skipReason;
        }
    }
}