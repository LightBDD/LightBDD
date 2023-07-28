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
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Implementation;

namespace LightBDD.Core.Execution.Implementation;

internal class RunnableScenarioV2 : IRunnableScenarioV2, IScenario, IRunStageContext
{
    private readonly ScenarioEntryMethod _entryMethod;
    private readonly Func<Task> _decoratedMethod;
    private readonly ScenarioResult _result;
    private readonly FixtureManager _fixtureManager;
    private IDependencyContainer? _scenarioScope;
    public IScenarioResult Result => _result;
    public IScenarioInfo Info => Result.Info;
    public Func<Exception, bool> ShouldAbortSubStepExecution { get; private set; } = _ => true;
    public IDependencyContainer DependencyContainer => _scenarioScope ?? throw new InvalidOperationException("Scenario not running");
    public IDependencyResolver DependencyResolver => _scenarioScope ?? Engine.DependencyContainer;
    public object Context => Fixture;
    public object Fixture => _fixtureManager.Fixture ?? throw new InvalidOperationException("Fixture not initialized");
    public EngineContext Engine { get; }
    IMetadataInfo IRunStageContext.Info => Info;

    public RunnableScenarioV2(EngineContext engine, IScenarioInfo info, IEnumerable<IScenarioDecorator> decorators, ScenarioEntryMethod entryMethod)
    {
        Engine = engine;
        _fixtureManager = new(engine.FixtureFactory);
        _entryMethod = entryMethod;
        _result = new ScenarioResult(info);
        _decoratedMethod = DecoratingExecutor.DecorateScenario(this, () => AsyncStepSynchronizationContext.Execute(RunScenarioCore), decorators);
    }

    public async Task<IScenarioResult> RunAsync()
    {
        var startTime = Engine.ExecutionTimer.GetTime();
        _scenarioScope = Engine.DependencyContainer.BeginScope(LifetimeScope.Scenario);
        try
        {
            SetScenarioContext();
            Engine.ProgressNotifier.Notify(new ScenarioStarting(startTime, Result.Info));
            await _fixtureManager.InitializeAsync(_result.Info.Parent.FeatureType);
            await _decoratedMethod.Invoke();
            _result.UpdateScenarioResultV2(ExecutionStatus.Passed);
        }
        catch (Exception ex)
        {
            ExceptionProcessor.UpdateStatus(_result.UpdateScenarioResultV2, ex, Engine.Configuration);
        }

        ResetScenarioContext();
        await CleanupScenario();
        var endTime = Engine.ExecutionTimer.GetTime();
        _result.UpdateTime(endTime.GetExecutionTime(startTime));
        Engine.ProgressNotifier.Notify(new ScenarioFinished(endTime, Result));
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
            ExceptionProcessor.UpdateStatus(_result.UpdateScenarioResultV2, exception, Engine.Configuration);
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
        ShouldAbortSubStepExecution = shouldAbortExecutionFn;
    }

    private async Task RunScenarioCore()
    {
        var stepsRunner = new StepGroupRunner(this, string.Empty);
        ScenarioExecutionContext.Current.Get<CurrentScenarioProperty>().StepsRunner = stepsRunner;
        try
        {
            await _entryMethod.Invoke(Fixture, stepsRunner);
        }
        finally
        {
            ScenarioExecutionContext.Current.Get<CurrentScenarioProperty>().StepsRunner = null;
            _result.UpdateResults(stepsRunner.GetResults());
        }
    }
}