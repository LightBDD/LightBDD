using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Execution;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.Extensibility;

namespace LightBDD.ScenarioHelpers;

public class TestScenarioBuilder
{
    private readonly ICoreScenarioStepsRunner _coreBuilder;
    private ExecutionContextDescriptor? _contextDescriptor;

    public TestScenarioBuilder(ICoreScenarioStepsRunner coreBuilder)
    {
        _coreBuilder = coreBuilder;
    }

    public static TestScenarioBuilder Current => new(ScenarioExecutionContext.CurrentScenarioStepsRunner);

    public Task TestScenario(params StepDescriptor[] steps) => TestScenario(steps.AsEnumerable());

    public async Task TestScenario(IEnumerable<StepDescriptor> steps)
    {
        try
        {
            await WithContextIfSet()
                .AddSteps(steps)
                .RunAsync();
        }
        catch (ScenarioExecutionException e)
        {
            e.GetOriginal().Throw();
        }
    }

    private ICoreScenarioStepsRunner WithContextIfSet()
    {
        if (_contextDescriptor != null)
            return _coreBuilder.WithContext(_contextDescriptor);
        return _coreBuilder;
    }
    public Task TestScenario(params Action[] steps)
        => TestScenario(steps.Select(TestStep.CreateAsync).ToArray());

    public Task TestScenario(params Func<Task>[] steps)
        => TestScenario(steps.Select(TestStep.Create).ToArray());

    public Task TestGroupScenario(params Func<TestCompositeStep>[] steps)
        => TestScenario(steps.Select(TestStep.CreateComposite).ToArray());

    public TestScenarioBuilder WithContext(Func<object> contextProvider, bool takeOwnership)
    {
        _contextDescriptor = new ExecutionContextDescriptor(contextProvider, takeOwnership);
        return this;
    }

    public TestScenarioBuilder WithContext(Func<IDependencyResolver, object> contextResolver)
    {
        _contextDescriptor = new ExecutionContextDescriptor(contextResolver, null);
        return this;
    }
}


public static class TestScenarioStepsRunnerExtensions
{
    public static TestScenarioBuilder Test(this ICoreScenarioStepsRunner runner) => new(runner);
}