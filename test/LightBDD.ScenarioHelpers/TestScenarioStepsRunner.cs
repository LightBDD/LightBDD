using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Execution;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.Extensibility;

namespace LightBDD.ScenarioHelpers;

public class TestScenarioStepsRunner
{
    private readonly ICoreScenarioStepsRunner _coreBuilder;
    private ExecutionContextDescriptor? _contextDescriptor;

    public TestScenarioStepsRunner(ICoreScenarioStepsRunner coreBuilder)
    {
        _coreBuilder = coreBuilder;
    }

    public static TestScenarioStepsRunner Current => new(ScenarioExecutionContext.CurrentScenarioStepsRunner);

    public async Task TestScenario(params StepDescriptor[] steps)
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
    public async Task TestScenario(params Action[] steps)
    {
        await TestScenario(steps.Select(TestStep.CreateAsync).ToArray());
    }

    public Task TestScenario(params Func<Task>[] steps)
    {
        return TestScenario(steps.Select(TestStep.Create).ToArray());
    }

    public async Task TestGroupScenario(params Func<TestCompositeStep>[] steps)
    {
        await TestScenario(steps.Select(TestStep.CreateComposite).ToArray());
    }

    public TestScenarioStepsRunner WithContext(Func<object> contextProvider, bool takeOwnership)
    {
        _contextDescriptor = new ExecutionContextDescriptor(contextProvider, takeOwnership);
        return this;
    }

    public TestScenarioStepsRunner WithContext(Func<IDependencyResolver, object> contextResolver)
    {
        _contextDescriptor = new ExecutionContextDescriptor(contextResolver, null);
        return this;
    }
}


public static class TestScenarioStepsRunnerExtensions
{
    public static TestScenarioStepsRunner Test(this ICoreScenarioStepsRunner runner) => new(runner);
}