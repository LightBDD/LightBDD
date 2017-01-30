using System.IO;
using System.Reflection;
using System.Threading;

namespace LightBDD.Integration
{
    internal class TestContextProvider
    {
        private static readonly AsyncLocal<TestContextProvider> _provider = new AsyncLocal<TestContextProvider>();
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