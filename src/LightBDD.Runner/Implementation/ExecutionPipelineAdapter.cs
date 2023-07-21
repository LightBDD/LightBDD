using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Discovery;
using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results;
using LightBDD.Framework.Reporting;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Runner.Implementation;

internal class ExecutionPipelineAdapter : ExecutionPipeline
{
    private readonly AsyncLocal<(ITestClass testClass, ITestCase[] cases)> _currentFeature = new();
    private readonly AsyncLocal<(ITestMethod testMethod, ITestCase[] cases)> _currentMethod = new();
    private readonly AsyncLocal<(ITestCase testCase, ITest test)> _currentTest = new();
    private readonly IMessageBus _bus;
    private readonly CancellationTokenSource _cts = new();
    private readonly ITestCollection _collection;
    private Dictionary<string, ITestCase> _allCases = new();

    public ExecutionPipelineAdapter(IMessageBus bus, ITestAssembly testAssembly, Action<LightBddConfiguration>? onConfigure)
        : base(testAssembly.Assembly.ToRuntimeAssembly(), onConfigure)
    {
        _bus = bus;
        _collection = LightBddTestCollection.Create(testAssembly);
    }

    public async Task<ITestRunResult> Execute(IEnumerable<ITestCase> testCases)
    {
        _allCases = testCases.ToDictionary(c => c.UniqueID);
        return await Execute(_allCases.Values.Select(ConvertTestCase).ToArray(), _cts.Token);
    }

    protected override void OnBeforeScenario(EventTime time, IScenarioInfo scenarioInfo, ScenarioCase scenario)
    {
        var testCase = _allCases[scenario.RuntimeId!];
        var test = new LightBddTest(testCase);
        _currentTest.Value = (testCase, test);

        TestOutputHelpers.Install(_bus, test);
        Send(new TestCaseStarting(testCase));
        Send(new TestStarting(test));
    }

    protected override void OnAfterScenario(EventTime time, IScenarioResult result)
    {
        var (testCase, test) = _currentTest.Value;
        var duration = GetDuration(result.ExecutionTime);


        var output = (TestOutputHelpers.Current as TestOutputHelper)?.Output;
        if (result.Status == ExecutionStatus.Failed)
            Send(new TestFailed(test, duration, output, result.ExecutionException));
        else if (result.Status == ExecutionStatus.Ignored)
            Send(new TestSkipped(test, output));
        else if (result.Status is ExecutionStatus.Passed or ExecutionStatus.Bypassed)
            Send(new TestPassed(test, duration, output));

        Send(new TestFinished(test, duration, output));
        Send(new TestCaseFinished(testCase, duration, 1,
            result.Status == ExecutionStatus.Failed ? 1 : 0,
            result.Status == ExecutionStatus.Ignored ? 1 : 0));
    }

    protected override void OnBeforeScenarioGroup(EventTime time, MethodInfo scenarioMethod, IReadOnlyList<ScenarioCase> scenarios)
    {
        //TODO: optimize
        var cases = scenarios.Select(s => _allCases[s.RuntimeId!]).ToArray();
        var testMethod = cases.First().TestMethod;
        _currentMethod.Value = (testMethod, cases);
        Send(new TestMethodStarting(cases, testMethod));
    }

    protected override void OnAfterScenarioGroup(EventTime endTime, IReadOnlyList<IScenarioResult> results)
    {
        var (testMethod, cases) = _currentMethod.Value;
        var duration = (decimal)ExecutionTimeSummary.Calculate(results.Select(s => s.ExecutionTime)).Duration.TotalSeconds;
        var failed = results.Count(r => r.Status == ExecutionStatus.Failed);
        var skipped = results.Count(r => r.Status == ExecutionStatus.Ignored);
        var total = results.Count;
        Send(new TestMethodFinished(cases, testMethod, duration, total, failed, skipped));
    }

    protected override void OnBeforeTestRunStart(EventTime time, ITestRunInfo testRunInfo, IReadOnlyList<ScenarioCase> scenarios)
    {
        Send(new TestCollectionStarting(_allCases.Values, _collection));
    }

    protected override void OnBeforeFeatureStart(EventTime time, IFeatureInfo featureInfo, ScenarioCase[] scenarios)
    {
        var cases = scenarios.Select(s => _allCases[s.RuntimeId!]).ToArray();
        var testClass = cases.First().TestMethod.TestClass;
        _currentFeature.Value = (testClass, cases);
        Send(new TestClassStarting(_currentFeature.Value.cases, _currentFeature.Value.testClass));
    }

    protected override void OnAfterFeatureFinished(EventTime time, IFeatureResult result)
    {
        var (testClass, cases) = _currentFeature.Value;
        var duration = (decimal)ExecutionTimeSummary.Calculate(result.GetScenarios().Select(s => s.ExecutionTime)).Duration.TotalSeconds;
        var failed = result.CountScenariosWithStatus(ExecutionStatus.Failed);
        var skipped = result.CountScenariosWithStatus(ExecutionStatus.Ignored);
        var total = result.GetScenarios().Count();
        Send(new TestClassFinished(cases, testClass, duration, total, failed, skipped));
    }

    protected override void OnAfterTestRunFinish(EventTime time, ITestRunResult result)
    {
        var failed = result.Features.Sum(f => f.CountScenariosWithStatus(ExecutionStatus.Failed));
        var skipped = result.Features.Sum(f => f.CountScenariosWithStatus(ExecutionStatus.Ignored));
        var total = result.Features.Sum(f => f.GetScenarios().Count());

        Send(new TestCollectionFinished(_allCases.Values, _collection, GetDuration(result.ExecutionTime), total, failed, skipped));
    }

    private static decimal GetDuration(ExecutionTime t) => (decimal)t.Duration.TotalSeconds;

    private ScenarioCase ConvertTestCase(ITestCase c)
    {
        return ScenarioCase.CreateParameterized(c.TestMethod.TestClass.Class.ToRuntimeType().GetTypeInfo(), c.TestMethod.Method.ToRuntimeMethod(), c.TestMethodArguments)
            .WithRuntimeId(c.UniqueID);
    }

    private void Send(IMessageSinkMessage message)
    {
        if (_cts.IsCancellationRequested)
            return;

        if (!_bus.QueueMessage(message))
            _cts.Cancel();
    }
}