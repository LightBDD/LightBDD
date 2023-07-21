using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Runner.Implementation;

internal class LightBddTestFramework : TestFramework
{
    public LightBddTestFramework(IMessageSink messageSink) : base(messageSink)
    {
    }

    protected override ITestFrameworkDiscoverer CreateDiscoverer(IAssemblyInfo assemblyInfo)
    {
        return new XunitTestFrameworkDiscoverer(assemblyInfo, SourceInformationProvider, DiagnosticMessageSink, new LightBddCollectionFactory(assemblyInfo, DiagnosticMessageSink));
    }

    protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
    {
        return new LightBddTestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
    }
}