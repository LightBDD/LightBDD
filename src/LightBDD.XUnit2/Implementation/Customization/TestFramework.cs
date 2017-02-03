using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.XUnit2.Implementation.Customization
{
    internal class TestFramework : XunitTestFramework
    {
        public TestFramework(IMessageSink messageSink) : base(messageSink)
        {
        }

        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        {
            return new TestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
        }
    }
}