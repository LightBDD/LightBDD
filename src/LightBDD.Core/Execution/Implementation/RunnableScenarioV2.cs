#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.ExecutionContext.Implementation;
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
    private IDependencyContainer? _scenarioScope;
    public IScenarioResult Result => _result;
    public IScenarioInfo Info => Result.Info;
    public IDependencyResolver DependencyResolver => _scenarioScope ?? _integration.DependencyContainer;
    public object Context { get; } = new object();
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
        _scenarioScope = _integration.DependencyContainer.BeginScope(LifetimeScope.Scenario);
        try
        {
            SetScenarioContext();
            await _fixtureManager.InitializeAsync(_result.Info.Parent.FeatureType);
            await _decoratedMethod.Invoke();
            _result.UpdateScenarioResultV2(ExecutionStatus.Passed);
        }
        catch (Exception ex)
        {
            ExceptionProcessor.UpdateStatus(_result.UpdateScenarioResultV2, ex, _integration.Configuration);
        }

        ResetScenarioContext();
        await CleanupScenario();
        var endTime = _integration.ExecutionTimer.GetTime();
        _result.UpdateResult(Array.Empty<IStepResult>(), endTime.GetExecutionTime(startTime));
        return Result;
    }

    private void SetScenarioContext()
    {
        ScenarioExecutionContext.Current = new ScenarioExecutionContext();
        ScenarioExecutionContext.Current.Get<CurrentScenarioProperty>().Scenario = this;
    }

    private void ResetScenarioContext()
    {
        ScenarioExecutionContext.Current = null;
    }

    private async Task CleanupScenario()
    {
        var collector = new ExceptionCollector();
        await _fixtureManager.DisposeAsync(collector);
        collector.Capture(DisposeScenarioScope);
        var exception = collector.Collect();
        if (exception != null)
            ExceptionProcessor.UpdateStatus(_result.UpdateScenarioResultV2, exception, _integration.Configuration);
    }

    private void DisposeScenarioScope()
    {
        try
        {
            _scenarioScope?.Dispose();
            _scenarioScope = null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"DI Scope Dispose() failed: {ex.Message}", ex);
        }
    }

    public void ConfigureExecutionAbortOnSubStepException(Func<Exception, bool> shouldAbortExecutionFn)
    {

    }

    private async Task RunScenarioCore()
    {
        await _entryMethod.Invoke(Fixture, null!);
    }
}