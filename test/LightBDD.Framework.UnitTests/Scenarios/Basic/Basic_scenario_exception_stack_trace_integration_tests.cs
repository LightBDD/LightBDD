using System;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios;
using LightBDD.Framework.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;
using Shouldly;

#pragma warning disable 1998

namespace LightBDD.Framework.UnitTests.Scenarios.Basic
{
    [TestFixture]
    [Ignore("call stack to be reworked")]
    //TODO: rework
    public class Basic_scenario_exception_stack_trace_integration_tests
    {
        [Test]
        public async Task RunScenario_should_expose_exception_with_simple_call_stack_when_step_throws_it()
        {
            var scenario = await TestableBddRunner.Default.RunScenario(r => r.RunScenarioAsync(Step_throwing_exception));

            var ex = scenario.ExecutionException.ShouldBeOfType<InvalidOperationException>();
            ex.AssertStackTraceMatching(
@"^\s*at LightBDD.Framework.UnitTests.Scenarios.Basic.Basic_scenario_exception_stack_trace_integration_tests.Step_throwing_exception[^\n]*
\s*at LightBDD.Framework.Scenarios.Implementation.BasicStepCompiler.StepExecutor.Execute[^\n]*
([^\n]*
)?\s*at LightBDD.Framework.Scenarios.BasicExtensions.RunScenario[^\n]*");
        }

        void Step_throwing_exception()
        {
            throw new InvalidOperationException();
        }

        [Test]
        public async Task RunScenarioAsync_should_expose_exception_with_simple_call_stack_when_async_step_throws_it_immediately()
        {
            var scenario = await TestableBddRunner.Default.RunScenario(r => r.RunScenarioAsync(Async_step_throwing_exception_immediately));

            var ex = scenario.ExecutionException.ShouldBeOfType<InvalidOperationException>();
            ex.AssertStackTraceMatching(
                @"^\s*at LightBDD.Framework.UnitTests.Scenarios.Basic.Basic_scenario_exception_stack_trace_integration_tests[^\n]*Async_step_throwing_exception_immediately[^\n]*
([^\n]*
)?\s*at LightBDD.Framework.Scenarios.BasicExtensions[^\n]*RunScenarioAsync[^\n]*");
        }

        async Task Async_step_throwing_exception_immediately()
        {
            throw new InvalidOperationException();
        }

        [Test]
        public async Task RunScenarioAsync_should_expose_exception_with_simple_call_stack_when_async_step_throws_after_await()
        {
            var scenario = await TestableBddRunner.Default.RunScenario(r => r.RunScenarioAsync(Async_step_throwing_exception_after_await));

            var ex = scenario.ExecutionException.ShouldBeOfType<InvalidOperationException>();
            ex.AssertStackTraceMatching(
                @"^\s*at LightBDD.Framework.UnitTests.Scenarios.Basic.Basic_scenario_exception_stack_trace_integration_tests[^\n]*Async_step_throwing_exception_after_await[^\n]*
([^\n]*
)?\s*at LightBDD.Framework.Scenarios.BasicExtensions[^\n]*RunScenarioAsync[^\n]*");
        }

        async Task Async_step_throwing_exception_after_await()
        {
            await Task.Yield();
            throw new InvalidOperationException();
        }
    }
}
