using System;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.Framework.Scenarios.Fluent;
using LightBDD.UnitTests.Helpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;
#pragma warning disable 1998

namespace LightBDD.Framework.UnitTests.Scenarios.Fluent
{
    [TestFixture]
    public class Fluent_scenario_exception_stack_trace_integration_tests
    {
        private IBddRunner _runner;

        [SetUp]
        public void SetUp()
        {
            _runner = TestableFeatureRunnerRepository.GetRunner(GetType()).GetBddRunner(this);
        }

        [Test]
        public void RunAsync_should_expose_exception_with_simple_call_stack_when_step_throws_it()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _runner.NewScenario().AddSteps(Step_throwing_exception).RunAsync());
            ex.AssertStackTraceMatching(
                @"^\s*at LightBDD.Framework.UnitTests.Scenarios.Fluent.Fluent_scenario_exception_stack_trace_integration_tests.Step_throwing_exception\(\) [^\n]*
\s*at LightBDD.Framework.Scenarios.Basic.Implementation.BasicStepCompiler.StepExecutor.Execute\(Object context, Object\[\] args\)[^\n]*
--- End of stack trace from previous location where exception was thrown ---
\s*at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw\(\)
\s*at LightBDD.Framework.Scenarios.Fluent.Implementation.ScenarioBuilder`1.<RunAsync>d__4.MoveNext\(\)[^\n]*");
        }

        void Step_throwing_exception()
        {
            throw new InvalidOperationException();
        }
    }
}
