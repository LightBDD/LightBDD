using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Runner.Implementation;

internal class LightBddTestFrameworkExecutor : XunitTestFrameworkExecutor
{
    private readonly IXunitTestCollectionFactory _collectionFactory;

    public LightBddTestFrameworkExecutor(AssemblyName assemblyName, ISourceInformationProvider sourceInformationProvider, IMessageSink diagnosticMessageSink)
        : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
    {
        _collectionFactory = new LightBddCollectionFactory(AssemblyInfo, DiagnosticMessageSink);
    }

    protected override ITestFrameworkDiscoverer CreateDiscoverer() => new XunitTestFrameworkDiscoverer(AssemblyInfo, SourceInformationProvider, DiagnosticMessageSink, _collectionFactory);

    protected override async void RunTestCases(IEnumerable<IXunitTestCase> testCases, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
    {
        using var assemblyRunner = new LightBddTestAssemblyRunner(TestAssembly, testCases, DiagnosticMessageSink, executionMessageSink, executionOptions);
        await assemblyRunner.RunAsync();
    }
}