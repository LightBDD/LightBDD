using System.IO;
using System.Reflection;
using System.Threading;
using LightBDD.Framework.ExecutionContext;

namespace LightBDD.NUnit3.Implementation
{
    internal class TestContextProvider
    {
        private static readonly AsyncLocalContext<TestContextProvider> _provider = new AsyncLocalContext<TestContextProvider>();
        public  MethodInfo TestMethod { get; }
        public  TextWriter TestOutWriter { get; }

        public static TestContextProvider Current => _provider.Value;

        public static void Initialize(MethodInfo testMethod, TextWriter outWriter)
        {
            _provider.Value = new TestContextProvider(testMethod,outWriter);
        }

        public static void Clear()
        {
            _provider.Value = null;
        }

        private TestContextProvider(MethodInfo testMethod, TextWriter testOutWriter)
        {
            TestMethod = testMethod;
            TestOutWriter = testOutWriter;
        }
    }
}