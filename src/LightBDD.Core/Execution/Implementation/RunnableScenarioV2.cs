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
    private readonly ScenarioEntryMethod _entryMethod;
    private readonly Func<Task> _decoratedMethod;

    public RunnableScenarioV2(IScenarioInfo info, IEnumerable<IScenarioDecorator> decorators, ScenarioEntryMethod entryMethod)
    {
        _entryMethod = entryMethod;
        Result = new ScenarioResult(info);
        _decoratedMethod = DecoratingExecutor.DecorateScenario(this, RunScenarioCore, decorators);
    }

    public IScenarioResult Result { get; }

    public async Task<IScenarioResult> RunAsync()
    {
        await _decoratedMethod.Invoke();
        return Result;
    }

    public void ConfigureExecutionAbortOnSubStepException(Func<Exception, bool> shouldAbortExecutionFn)
    {
        
    }

    private async Task RunScenarioCore()
    {
        await _entryMethod.Invoke(Fixture, null!);
    }

    public IScenarioInfo Info { get; } = null;
    public IDependencyResolver DependencyResolver { get; } = null;
    public object Context { get; } = null;
    public object Fixture { get; } = null;
}