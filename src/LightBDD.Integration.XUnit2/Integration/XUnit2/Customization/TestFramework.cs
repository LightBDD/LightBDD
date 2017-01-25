using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Integration.XUnit2.Customization
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