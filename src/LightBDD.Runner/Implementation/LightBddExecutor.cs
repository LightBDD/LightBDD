using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Runner.Implementation;

internal class LightBddExecutor : TestFrameworkExecutor<ITestCase>
{
    private readonly Assembly _assembly;

    public LightBddExecutor(AssemblyName assemblyName, ISourceInformationProvider sourceInformationProvider, IMessageSink diagnosticMessageSink)
        : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
    {
        _assembly = Assembly.Load(new AssemblyName(AssemblyInfo.Name));
    }

    protected override ITestFrameworkDiscoverer CreateDiscoverer() => new LightBddDiscoverer(AssemblyInfo, SourceInformationProvider, DiagnosticMessageSink);

    protected override void RunTestCases(IEnumerable<ITestCase> testCases, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
    {
        var scope = GetLightBddScope();
        using var messageBus = new MessageBus(executionMessageSink, executionOptions.StopOnTestFailOrDefault());
        using var coordinator = XUnit2FeatureCoordinator.InstallSelf(_assembly, scope.Configure());
        var timer = coordinator.IntegrationContext.ExecutionTimer;
        var testAssembly = new TestAssembly(AssemblyInfo);
        messageBus.QueueMessage(new TestAssemblyStarting(testCases, testAssembly, timer.GetTime().Start.DateTime, "", ""));
        foreach (var testCase in testCases)
        {
            var runtimeType = testCase.TestMethod.TestClass.Class.ToRuntimeType();
            var testOutputHelper = new TestOutputHelper();
            var test = new LightBddTest(testCase, testCase.DisplayName);
            testOutputHelper.Initialize(messageBus, test);
            var methodInfo = testCase.TestMethod.Method.ToRuntimeMethod();
            var start = timer.GetTime();
            TestContextProvider.Initialize(methodInfo, testCase.TestMethodArguments, testOutputHelper, string.Empty);
            messageBus.QueueMessage(new TestClassConstructionStarting(test));
            var instance = Activator.CreateInstance(runtimeType);
            messageBus.QueueMessage(new TestClassConstructionFinished(test));
            try
            {
                try
                {
                    messageBus.QueueMessage(new TestStarting(test));
                    var result = methodInfo.Invoke(instance, testCase.TestMethodArguments);
                    (result as Task)?.GetAwaiter().GetResult();
                    messageBus.QueueMessage(new TestPassed(test,
                        (decimal)timer.GetTime().GetExecutionTime(start).Duration.TotalSeconds, ""));
                }
                catch (TargetInvocationException x) when (x.InnerException is IgnoreException ex)
                {
                    messageBus.QueueMessage(new TestSkipped(test, ex.Message));
                }
                catch (TargetInvocationException x)
                {
                    messageBus.QueueMessage(new TestFailed(test, (decimal)timer.GetTime().GetExecutionTime(start).Duration.TotalSeconds, "", x.InnerException));
                }
                catch (Exception ex)
                {
                    messageBus.QueueMessage(new TestFailed(test, (decimal)timer.GetTime().GetExecutionTime(start).Duration.TotalSeconds, "", ex));
                }
            }
            finally
            {
                (instance as IDisposable)?.Dispose();
                messageBus.QueueMessage(new TestFinished(test, 0, ""));
            }
        }

        messageBus.QueueMessage(new TestAssemblyFinished(testCases, testAssembly, 0, 0, 0, 0));
    }

    private LightBddScopeAttribute GetLightBddScope()
    {
        var attributes = _assembly.GetCustomAttributes<LightBddScopeAttribute>().ToArray();
        if (attributes.Length > 1)
            throw new InvalidOperationException($"Only one attribute of {typeof(LightBddScopeAttribute)} type can be defined in assembly {_assembly.FullName}");
        return attributes.FirstOrDefault() ?? new LightBddScopeAttribute();
    }
}