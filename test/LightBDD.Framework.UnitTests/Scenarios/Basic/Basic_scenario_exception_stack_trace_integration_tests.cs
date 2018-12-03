using System;
using System.Threading.Tasks;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.UnitTests.Helpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;
#pragma warning disable 1998

namespace LightBDD.Framework.UnitTests.Scenarios.Basic
{
    [TestFixture]
    public class Basic_scenario_exception_stack_trace_integration_tests
    {
        private IBddRunner _runner;

        [SetUp]
        public void SetUp()
        {
            _runner = TestableFeatureRunnerRepository.GetRunner(GetType()).GetBddRunner(this);
        }

        [Test]
        public void RunScenario_should_expose_exception_with_simple_call_stack_when_step_throws_it()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _runner.RunScenario(Step_throwing_exception));
            ex.AssertStackTraceMatching(
@"^\s*at LightBDD.Framework.UnitTests.Scenarios.Basic.Basic_scenario_exception_stack_trace_integration_tests.Step_throwing_exception[^\n]*
\s*at LightBDD.Framework.Scenarios.Basic.Implementation.BasicStepCompiler.StepExecutor.Execute[^\n]*
--- End of stack trace from previous location where exception was thrown ---
([^\n]*
)?\s*at LightBDD.Framework.Scenarios.Basic.BasicScenarioExtensions.RunScenario[^\n]*");
        }

        void Step_throwing_exception()
        {
            throw new InvalidOperationException();
        }

        [Test]
        public void RunScenarioAsync_should_expose_exception_with_simple_call_stack_when_async_step_throws_it_immediately()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _runner.RunScenarioAsync(Async_step_throwing_exception_immediately));
            ex.AssertStackTraceMatching(
                @"^\s*at LightBDD.Framework.UnitTests.Scenarios.Basic.Basic_scenario_exception_stack_trace_integration_tests[^\n]*Async_step_throwing_exception_immediately[^\n]*
--- End of stack trace from previous location where exception was thrown ---
([^\n]*
)?\s*at LightBDD.Framework.Scenarios.Basic.BasicScenarioExtensions[^\n]*RunScenarioAsync[^\n]*");
        }

        async Task Async_step_throwing_exception_immediately()
        {
            throw new InvalidOperationException();
        }

        [Test]
        public void RunScenarioAsync_should_expose_exception_with_simple_call_stack_when_async_step_throws_after_await()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _runner.RunScenarioAsync(Async_step_throwing_exception_after_await));
            ex.AssertStackTraceMatching(
                @"^\s*at LightBDD.Framework.UnitTests.Scenarios.Basic.Basic_scenario_exception_stack_trace_integration_tests[^\n]*Async_step_throwing_exception_after_await[^\n]*
--- End of stack trace from previous location where exception was thrown ---
([^\n]*
)?\s*at LightBDD.Framework.Scenarios.Basic.BasicScenarioExtensions[^\n]*RunScenarioAsync[^\n]*");
        }

        async Task Async_step_throwing_exception_after_await()
        {
            await Task.Yield();
            throw new InvalidOperationException();
        }

        [Test]
        public void RunScenarioActionsAsync_should_expose_exception_with_simple_call_stack_when_async_void_step_throws_after_await()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _runner.RunScenarioActionsAsync(Async_void_step_throwing_exception_after_await));
            ex.AssertStackTraceMatching(
                @"^\s*at LightBDD.Framework.UnitTests.Scenarios.Basic.Basic_scenario_exception_stack_trace_integration_tests[^\n]*Async_void_step_throwing_exception_after_await[^\n]*
--- End of stack trace from previous location where exception was thrown ---
([^\n]*
){0,2}\s*at LightBDD.Core.Execution.Implementation.AsyncStepSynchronizationContext.RunWithSelf[^\n]*
--- End of stack trace from previous location where exception was thrown ---
([^\n]*
)?\s*at LightBDD.Framework.Scenarios.Basic.BasicScenarioExtensions[^\n]*RunScenarioActionsAsync[^\n]*");
        }

        async void Async_void_step_throwing_exception_after_await()
        {
            await Task.Yield();
            throw new InvalidOperationException();
        }
    }
}
