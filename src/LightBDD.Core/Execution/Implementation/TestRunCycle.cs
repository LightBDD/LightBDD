using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Execution.Implementation.GlobalSetUp;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Implementation;

namespace LightBDD.Core.Execution.Implementation;

internal class TestRunCycle
{
    private readonly IntegrationContext _integrationContext;
    private readonly FeatureRunnerRepository _runnerRepository;
    private readonly IDependencyContainer _container;
    private readonly GlobalSetUpRegistry _globalSetUp;
    private EventTime _startTime;
    private readonly ITestRunInfo _info;

    public TestRunCycle(IntegrationContext integrationContext, FeatureRunnerRepository runnerRepository)
    {
        _integrationContext = integrationContext;
        _runnerRepository = runnerRepository;
        _container = _integrationContext.DependencyContainer;
        _globalSetUp = _integrationContext.Configuration.Get<ExecutionExtensionsConfiguration>().GlobalSetUpRegistry;
        _info = _integrationContext.MetadataProvider.GetTestRunInfo();
    }

    public void Start()
    {
        _startTime = _integrationContext.ExecutionTimer.GetTime();
        _integrationContext.ProgressNotifier.Notify(new TestRunStarting(_startTime, _info));
        Task.Run(RunGlobalSetUp).GetAwaiter().GetResult();
    }

    public ITestRunResult Finish()
    {
        //TODO: collect global setup/tear down on test run result.
        var featureResults = CollectFeatures();
        try
        {
            Task.Run(RunGlobalTearDown).GetAwaiter().GetResult();
        }
        finally
        {
            _container.Dispose();
        }

        var endTime = _integrationContext.ExecutionTimer.GetTime();
        var result = new TestRunResult(_info, endTime.GetExecutionTime(_startTime), featureResults);

        _integrationContext.ProgressNotifier.Notify(new TestRunFinished(endTime, result));
        return result;
    }

    private IEnumerable<IFeatureResult> CollectFeatures()
    {
        //Beware that runner repository can theoretically be still running some tests and it won't wait for them to finish. See FeatureRunner.CreateScenarioRunner()
        foreach (var runner in _runnerRepository.AllRunners)
        {
            runner.Dispose();
            yield return runner.GetFeatureResult();
        }
    }


    private Task RunGlobalTearDown() => _globalSetUp.TearDownAsync(_container);
    private Task RunGlobalSetUp() => _globalSetUp.SetUpAsync(_container);
}