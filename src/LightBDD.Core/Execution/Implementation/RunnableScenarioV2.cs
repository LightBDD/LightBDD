using System;
using System.Collections.Generic;
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
    public IScenarioResult Result => _result;
    public IScenarioInfo Info { get; } = null;
    public IDependencyResolver DependencyResolver { get; } = null;
    public object Context { get; } = null;
    public object Fixture { get; } = null;

    public RunnableScenarioV2(IntegrationContext integration, IScenarioInfo info,
        IEnumerable<IScenarioDecorator> decorators, ScenarioEntryMethod entryMethod)
    {
        _integration = integration;
        _entryMethod = entryMethod;
        _result = new ScenarioResult(info);
        _decoratedMethod = DecoratingExecutor.DecorateScenario(this, RunScenarioCore, decorators);
    }

    public async Task<IScenarioResult> RunAsync()
    {
        var startTime = _integration.ExecutionTimer.GetTime();
        await _decoratedMethod.Invoke();
        var endTime = _integration.ExecutionTimer.GetTime();

        _result.UpdateScenarioResult(ExecutionStatus.Passed);
        _result.UpdateResult(Array.Empty<IStepResult>(), endTime.GetExecutionTime(startTime));
        return Result;
    }

    public void ConfigureExecutionAbortOnSubStepException(Func<Exception, bool> shouldAbortExecutionFn)
    {

    }

    private async Task RunScenarioCore()
    {
        await _entryMethod.Invoke(Fixture, null!);
    }
}