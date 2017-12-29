using System;
using System.Threading.Tasks;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.UnitTests.Helpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;
#pragma warning disable 1998

namespace LightBDD.Framework.UnitTests.Scenarios.Extended
{
    [TestFixture]
    public class Extended_scenario_exception_stack_trace_integration_tests
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
            var ex = Assert.Throws<InvalidOperationException>(() => _runner.RunScenario(_ => Step_throwing_exception()));
            ex.AssertStackTraceMatching(
@"^\s*at LightBDD.Framework.UnitTests.Scenarios.Extended.Extended_scenario_exception_stack_trace_integration_tests.Step_throwing_exception\(\)[^\n]*
\s*at lambda_method\(Closure , NoContext , Object[] \)
--- End of stack trace from previous location where exception was thrown ---
\s*at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw\(\)[^\n]*
\s*at LightBDD.Framework.Scenarios.Extended.ExtendedScenarioExtensions.RunScenario\[TContext\]\(IBddRunner`1 runner, Expression`1\[\] steps\)[^\n]*");
        }

        void Step_throwing_exception()
        {
            throw new InvalidOperationException();
        }

        [Test]
        public void RunScenarioAsync_should_expose_exception_with_simple_call_stack_when_async_step_throws_it_immediatelly()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _runner.RunScenarioAsync(_ => Async_step_throwing_exception_immediatelly()));
            ex.AssertStackTraceMatching(
                @"^\s*at LightBDD.Framework.UnitTests.Scenarios.Extended.Extended_scenario_exception_stack_trace_integration_tests.<Async_step_throwing_exception_immediatelly>d__5.MoveNext\(\)[^\n]*
--- End of stack trace from previous location where exception was thrown ---
\s*at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw\(\)[^\n]*
\s*at LightBDD.Framework.Scenarios.Extended.ExtendedScenarioExtensions.<RunScenarioAsync>d__1`1.MoveNext\(\)[^\n]*");
        }

        async Task Async_step_throwing_exception_immediatelly()
        {
            throw new InvalidOperationException();
        }

        [Test]
        public void RunScenarioAsync_should_expose_exception_with_simple_call_stack_when_async_step_throws_after_await()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _runner.RunScenarioAsync(_ => Async_step_throwing_exception_after_await()));
            ex.AssertStackTraceMatching(
                @"^\s*at LightBDD.Framework.UnitTests.Scenarios.Extended.Extended_scenario_exception_stack_trace_integration_tests.<Async_step_throwing_exception_after_await>d__7.MoveNext\(\)[^\n]*
--- End of stack trace from previous location where exception was thrown ---
\s*at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw\(\)
\s*at LightBDD.Framework.Scenarios.Extended.ExtendedScenarioExtensions.<RunScenarioAsync>d__1`1.MoveNext\(\)[^\n]*");
        }

        async Task Async_step_throwing_exception_after_await()
        {
            await Task.Yield();
            throw new InvalidOperationException();
        }

        [Test]
        public void RunScenarioActionsAsync_should_expose_exception_with_simple_call_stack_when_async_void_step_throws_after_await()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _runner.RunScenarioActionsAsync(_ => Async_void_step_throwing_exception_after_await()));
            ex.AssertStackTraceMatching(
                @"^\s*at LightBDD.Framework.UnitTests.Scenarios.Extended.Extended_scenario_exception_stack_trace_integration_tests.<Async_void_step_throwing_exception_after_await>d__9.MoveNext\(\)[^\n]*
--- End of stack trace from previous location where exception was thrown ---
\s*at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw\(\)[^\n]*
\s*at LightBDD.Core.Execution.Implementation.AsyncStepSynchronizationContext.RunWithSelf\(SendOrPostCallback d, Object s\)[^\n]*
--- End of stack trace from previous location where exception was thrown ---
\s*at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw\(\)[^\n]*
\s*at LightBDD.Framework.Scenarios.Extended.ExtendedScenarioExtensions.<RunScenarioActionsAsync>d__2`1.MoveNext\(\)[^\n]*");
        }

        async void Async_void_step_throwing_exception_after_await()
        {
            await Task.Yield();
            throw new InvalidOperationException();
        }
    }
}
