using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Runner.Implementation;

internal class LightBddExecutor : TestFrameworkExecutor<ITestCase>
{

    public LightBddExecutor(AssemblyName assemblyName, ISourceInformationProvider sourceInformationProvider, IMessageSink diagnosticMessageSink)
        : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
    {
    }

    protected override ITestFrameworkDiscoverer CreateDiscoverer() => new LightBddDiscoverer(AssemblyInfo, SourceInformationProvider, DiagnosticMessageSink);

    protected override void RunTestCases(IEnumerable<ITestCase> testCases, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
    {
        //TODO: handle exceptions
        var scope = GetLightBddScope();
        using var messageBus = new MessageBus(executionMessageSink, executionOptions.StopOnTestFailOrDefault());

        new ExecutionPipelineAdapter(messageBus, AssemblyInfo, scope.Configure)
            .Execute(testCases)
            .GetAwaiter().GetResult();
    }


    private LightBddScopeAttribute GetLightBddScope()
    {
        var assembly = AssemblyInfo.ToRuntimeAssembly();

        var attributes = assembly.GetCustomAttributes<LightBddScopeAttribute>().ToArray();
        if (attributes.Length > 1)
            throw new InvalidOperationException($"Only one attribute of {typeof(LightBddScopeAttribute)} type can be defined in assembly {assembly.FullName}");
        return attributes.FirstOrDefault() ?? new LightBddScopeAttribute();
    }
}