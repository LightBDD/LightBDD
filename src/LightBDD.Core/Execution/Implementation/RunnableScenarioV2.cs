#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Implementation;

namespace LightBDD.Core.Execution.Implementation;

internal class RunnableScenarioV2 : IRunnableScenarioV2, IScenario
{
    private readonly IntegrationContext _integration;
    private readonly ScenarioEntryMethod _entryMethod;
    private readonly Func<Task> _decoratedMethod;
    private readonly ScenarioResult _result;
    private readonly FixtureManager _fixtureManager = new();
    public IScenarioResult Result => _result;
    public IScenarioInfo Info => Result.Info;
    public IDependencyResolver DependencyResolver { get; } = null;
    public object Context { get; } = null;
    public object Fixture => _fixtureManager.Fixture ?? throw new InvalidOperationException("Fixture not initialized");

    public RunnableScenarioV2(IntegrationContext integration, IScenarioInfo info, IEnumerable<IScenarioDecorator> decorators, ScenarioEntryMethod entryMethod)
    {
        _integration = integration;
        _entryMethod = entryMethod;
        _result = new ScenarioResult(info);
        _decoratedMethod = DecoratingExecutor.DecorateScenario(this, () => AsyncStepSynchronizationContext.Execute(RunScenarioCore), decorators);
    }

    public async Task<IScenarioResult> RunAsync()
    {
        var startTime = _integration.ExecutionTimer.GetTime();
        try
        {
            await _fixtureManager.InitializeAsync(_result.Info.Parent.FeatureType);
            await _decoratedMethod.Invoke();
            _result.UpdateScenarioResultV2(ExecutionStatus.Passed);
        }
        catch (Exception ex)
        {
            ExceptionProcessor.UpdateStatus(_result.UpdateScenarioResultV2, ex, _integration.Configuration);
        }
        await CleanupScenario();
        var endTime = _integration.ExecutionTimer.GetTime();
        _result.UpdateResult(Array.Empty<IStepResult>(), endTime.GetExecutionTime(startTime));
        return Result;
    }

    private async Task CleanupScenario()
    {
        var collector = new ExceptionCollector();
        await _fixtureManager.DisposeAsync(collector);
        var exception = collector.Collect();
        if (exception != null)
            ExceptionProcessor.UpdateStatus(_result.UpdateScenarioResultV2, exception, _integration.Configuration);
    }

    public void ConfigureExecutionAbortOnSubStepException(Func<Exception, bool> shouldAbortExecutionFn)
    {

    }

    private async Task RunScenarioCore()
    {
        await _entryMethod.Invoke(Fixture, null!);
    }
}