using System;
using System.Runtime.CompilerServices;
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
@"^\s*at LightBDD.Framework.UnitTests.Scenarios.Extended.Extended_scenario_exception_stack_trace_integration_tests.Step_throwing_exception[^\n]*
\s*at [^\n]*lambda_method[^\n]*
([^\n]*
)?\s*at LightBDD.Framework.Scenarios.Extended.ExtendedScenarioExtensions.RunScenario[^\n]*");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void Step_throwing_exception()
        {
            throw new InvalidOperationException();
        }

        [Test]
        public void RunScenarioAsync_should_expose_exception_with_simple_call_stack_when_async_step_throws_it_immediately()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _runner.RunScenarioAsync(_ => Async_step_throwing_exception_immediately()));
            ex.AssertStackTraceMatching(
                @"^\s*at LightBDD.Framework.UnitTests.Scenarios.Extended.Extended_scenario_exception_stack_trace_integration_tests[^\n]*Async_step_throwing_exception_immediately[^\n]*
([^\n]*
)?\s*at LightBDD.Framework.Scenarios.Extended.ExtendedScenarioExtensions[^\n]*RunScenarioAsync[^\n]*");
        }

        async Task Async_step_throwing_exception_immediately()
        {
            throw new InvalidOperationException();
        }

        [Test]
        public void RunScenarioAsync_should_expose_exception_with_simple_call_stack_when_async_step_throws_after_await()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _runner.RunScenarioAsync(_ => Async_step_throwing_exception_after_await()));
            ex.AssertStackTraceMatching(
                @"^\s*at LightBDD.Framework.UnitTests.Scenarios.Extended.Extended_scenario_exception_stack_trace_integration_tests[^\n]*Async_step_throwing_exception_after_await[^\n]*
([^\n]*
)?\s*at LightBDD.Framework.Scenarios.Extended.ExtendedScenarioExtensions[^\n]*RunScenarioAsync[^\n]*");
        }

        async Task Async_step_throwing_exception_after_await()
        {
            await Task.Yield();
            throw new InvalidOperationException();
        }
    }
}
