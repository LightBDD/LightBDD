using System;
using System.IO;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution;

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Ignore("Exception flow needs to be re-implemented")]
//TODO: rework
public class ExecutionPipeline_exception_handling_tests
{
    class MyFeature
    {
        [TestScenario]
        [ThrowingDecorator]
        public async Task Scenario_with_throwing_decorator() 
            => await TestScenarioBuilder.Current.TestScenario(Some_step);

        [TestScenario]
        public async Task Scenario_with_throwing_decorator_on_step() 
            => await TestScenarioBuilder.Current.TestScenario(Throwing_step);

        void Some_step() { }

        [ThrowingDecorator]
        void Throwing_step() { }
    }

    [Test]
    public async Task It_should_propagate_exception_thrown_from_step_decorator_with_simple_stack_trace()
    {
        var scenario = await TestableCoreExecutionPipeline.Default.ExecuteScenario<MyFeature>(f => f.Scenario_with_throwing_decorator());
        
        scenario.Status.ShouldBe(ExecutionStatus.Failed);
        var ex = scenario.ExecutionException;

        ex.AssertStackTraceMatching(
            @"^\s*at LightBDD.Core.UnitTests.RunnableScenario_decorators_tests.MyThrowingDecorator.ProcessStatus[^\n]*
\s*at LightBDD.Core.UnitTests.RunnableScenario_decorators_tests.MyThrowingDecorator.ExecuteAsync[^\n]*
\s*at LightBDD.Core.Execution.Implementation.DecoratingExecutor[^\n]+ExecuteAsync[^\n]*
([^\n]*
)?\s*at LightBDD.UnitTests.Helpers.TestableIntegration.TestSyntaxRunner.TestScenario[^\n]*");
    }

    [Test]
    public async Task It_should_propagate_exception_thrown_from_async_step_decorator_with_simple_stack_trace()
    {
        var scenario = await TestableCoreExecutionPipeline.Default.ExecuteScenario<MyFeature>(f => f.Scenario_with_throwing_decorator_on_step());

        scenario.Status.ShouldBe(ExecutionStatus.Failed);
        var ex = scenario.ExecutionException;

        ex.AssertStackTraceMatching(
            @"^\s*at LightBDD.Core.UnitTests.RunnableScenario_decorators_tests.MyThrowingDecorator.ProcessStatus[^\n]*
\s*at LightBDD.Core.UnitTests.RunnableScenario_decorators_tests.MyThrowingDecorator[^\n]+ProcessStatusAsync[^\n]*
([^\n]*
)?\s*at LightBDD.UnitTests.Helpers.TestableIntegration.TestSyntaxRunner[^\n]+TestScenarioAsync[^\n]*");
    }

    private class ThrowingDecorator : Attribute, IScenarioDecoratorAttribute, IStepDecoratorAttribute
    {
        public int Order { get; }

        public Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
        {
            throw new IOException("boom");
        }

        public Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
        {
            throw new IOException("boom");
        }
    }
}