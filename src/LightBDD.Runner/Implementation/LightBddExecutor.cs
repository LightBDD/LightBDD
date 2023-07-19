using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Discovery;
using LightBDD.Core.Execution;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Reporting;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Runner.Implementation;

internal class LightBddExecutor : TestFrameworkExecutor<ITestCase>
{
    private readonly Assembly _assembly;
    private readonly TestAssembly _testAssembly;

    public LightBddExecutor(AssemblyName assemblyName, ISourceInformationProvider sourceInformationProvider, IMessageSink diagnosticMessageSink)
        : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
    {
        _testAssembly = new TestAssembly(AssemblyInfo);
        _assembly = ((ReflectionAssemblyInfo)AssemblyInfo).Assembly;
    }

    protected override ITestFrameworkDiscoverer CreateDiscoverer() => new LightBddDiscoverer(AssemblyInfo, SourceInformationProvider, DiagnosticMessageSink);

    protected override void RunTestCases(IEnumerable<ITestCase> testCases, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
    {
        var scope = GetLightBddScope();
        using var messageBus = new MessageBus(new WrapMessageSink(executionMessageSink, "exec"), executionOptions.StopOnTestFailOrDefault());

        var cases = testCases.ToArray();

        void OnConfigure(LightBddConfiguration cfg)
        {
            scope.Configure(cfg);
            cfg.ProgressNotifierConfiguration().Prepend(new XunitBusAdapter(messageBus, _testAssembly, cases));
        }

        try
        {
            var result = new ExecutionPipeline(_assembly, OnConfigure).Execute(cases.Select(ConvertTestCase)).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            //send diagnostic message
        }
    }

    private ScenarioCase ConvertTestCase(ITestCase c)
    {
        return ScenarioCase.CreateParameterized(c.TestMethod.TestClass.Class.ToRuntimeType().GetTypeInfo(), c.TestMethod.Method.ToRuntimeMethod(), c.TestMethodArguments)
            .WithRuntimeId(c.UniqueID);
    }

    private LightBddScopeAttribute GetLightBddScope()
    {
        var attributes = _assembly.GetCustomAttributes<LightBddScopeAttribute>().ToArray();
        if (attributes.Length > 1)
            throw new InvalidOperationException($"Only one attribute of {typeof(LightBddScopeAttribute)} type can be defined in assembly {_assembly.FullName}");
        return attributes.FirstOrDefault() ?? new LightBddScopeAttribute();
    }
}

internal class XunitBusAdapter : IProgressNotifier
{
    private readonly MessageBus _bus;
    private readonly Dictionary<string, ITestCase> _testCases;
    private readonly TestCollection _dummyTestCollection;
    private readonly TestAssembly _testAssembly;
    private readonly Dictionary<string, ITestCase[]> _testClassCases;
    private readonly Dictionary<(string method, string type), ITestCase[]> _testMethodCases;
    private readonly ConcurrentDictionary<string, LightBddTest> _tests = new();

    public XunitBusAdapter(MessageBus bus, TestAssembly assembly, ITestCase[] testCases)
    {
        _bus = bus;
        _testAssembly = assembly;
        _testCases = testCases.ToDictionary(x => x.UniqueID);
        _testClassCases = testCases.GroupBy(x => x.TestMethod.TestClass.Class.Name).ToDictionary(x => x.Key, x => x.ToArray());
        _testMethodCases = testCases
            .GroupBy(x => (method: x.TestMethod.Method.Name, type: x.TestMethod.TestClass.Class.Name))
            .ToDictionary(x => x.Key, x => x.ToArray());
        _dummyTestCollection = new TestCollection(_testAssembly, null, "LightBdd Collection");
    }

    public void Notify(ProgressEvent e)
    {
        switch (e)
        {
            case TestRunStarting trs:
                QueueMessage(new TestAssemblyStarting(_testCases.Values, _testAssembly, trs.Time.Start.DateTime, "", ""));
                QueueMessage(new TestCollectionStarting(_testCases.Values, _dummyTestCollection));
                break;
            case TestRunFinished trf:
                var failed = trf.Result.Features.Sum(f => f.CountScenariosWithStatus(ExecutionStatus.Failed));
                var skipped = trf.Result.Features.Sum(f => f.CountScenariosWithStatus(ExecutionStatus.Ignored));
                var total = trf.Result.Features.Sum(f => f.GetScenarios().Count());
                QueueMessage(new TestCollectionFinished(_testCases.Values, _dummyTestCollection, (decimal)trf.Result.ExecutionTime.Duration.TotalSeconds, total, failed, skipped));
                QueueMessage(new TestAssemblyFinished(_testCases.Values, _testAssembly, (decimal)trf.Result.ExecutionTime.Duration.TotalSeconds, total, failed, skipped));
                break;
            case FeatureStarting fs:
                QueueMessage(new TestClassStarting(_testClassCases[fs.Feature.RuntimeId], new TestClass(_dummyTestCollection, new ReflectionTypeInfo(fs.Feature.FeatureType))));
                break;
            case FeatureFinished ff:
                QueueMessage(new TestClassFinished(_testClassCases[ff.Result.Info.RuntimeId],
                    new TestClass(_dummyTestCollection, new ReflectionTypeInfo(ff.Result.Info.FeatureType)),
                    (decimal)ExecutionTimeSummary.Calculate(ff.Result.GetScenarios().Select(s => s.ExecutionTime))
                        .Duration.TotalSeconds,
                    ff.Result.GetScenarios().Count(), ff.Result.CountScenariosWithStatus(ExecutionStatus.Failed), ff.Result.CountScenariosWithStatus(ExecutionStatus.Ignored)));
                break;
            case ScenarioInitializing si:
                TestOutputHelpers.Install(_bus, new LightBddTest(_testCases[si.Scenario.RuntimeId]));
                break;
            case ScenarioStarting ss:
            {
                var testCase = _testCases[ss.Scenario.RuntimeId];
                QueueMessage(new TestMethodStarting(_testMethodCases[(testCase.TestMethod.Method.Name, testCase.TestMethod.TestClass.Class.Name)], testCase.TestMethod));
                QueueMessage(new TestCaseStarting(testCase));
                QueueMessage(new TestStarting(_tests.GetOrAdd(ss.Scenario.RuntimeId,new LightBddTest(testCase))));
                break;
            }
            case ScenarioFinished sf:
            {
                var testCase = _testCases[sf.Result.Info.RuntimeId];
                var test = _tests[sf.Result.Info.RuntimeId];
                var output = (TestOutputHelpers.Current as TestOutputHelper)?.Output;
                if (sf.Result.Status == ExecutionStatus.Failed)
                    QueueMessage(new TestFailed(test, (decimal)sf.Result.ExecutionTime.Duration.TotalSeconds, output, sf.Result.ExecutionException));
                else if (sf.Result.Status == ExecutionStatus.Ignored)
                    QueueMessage(new TestSkipped(test, output));
                else if (sf.Result.Status is ExecutionStatus.Passed or ExecutionStatus.Bypassed)
                    QueueMessage(new TestPassed(test, (decimal)sf.Result.ExecutionTime.Duration.TotalSeconds, output));
                QueueMessage(new TestFinished(test, (decimal)sf.Result.ExecutionTime.Duration.TotalSeconds, output));
                QueueMessage(new TestCaseFinished(testCase, (decimal)sf.Result.ExecutionTime.Duration.TotalSeconds, 1,
                    sf.Result.Status == ExecutionStatus.Failed ? 1 : 0,
                    sf.Result.Status == ExecutionStatus.Ignored ? 1 : 0));
                QueueMessage(new TestMethodFinished(_testMethodCases[(testCase.TestMethod.Method.Name, testCase.TestMethod.TestClass.Class.Name)],testCase.TestMethod, (decimal)sf.Result.ExecutionTime.Duration.TotalSeconds, 1,
                    sf.Result.Status == ExecutionStatus.Failed ? 1 : 0,
                    sf.Result.Status == ExecutionStatus.Ignored ? 1 : 0));
                break;
            }
        }
    }

    void QueueMessage(IMessageSinkMessage msg)
    {
        if (!_bus.QueueMessage(msg))
            throw new InvalidOperationException();
    }
}

internal class TestOutputHelpers
{
    private static readonly AsyncLocal<ITestOutputHelper> Helpers = new();

    public static ITestOutputHelper Current => Helpers.Value ?? throw new InvalidOperationException("No scenario is executed at this moment");

    public static void Install(MessageBus bus, ITest test)
    {
        var helper = new TestOutputHelper();
        helper.Initialize(bus, test);
        Helpers.Value = helper;
    }

    public static void Clear() => Helpers.Value = null;
}