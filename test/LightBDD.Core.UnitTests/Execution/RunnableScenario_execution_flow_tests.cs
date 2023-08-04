using System;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using NUnit.Framework;
using Shouldly;
using IgnoreException = LightBDD.Core.Execution.IgnoreException;

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    public class RunnableScenario_execution_flow_tests
    {
        [Test]
        public async Task Successful_run_should_result_with_passed_scenario()
        {
            var result = await TestableScenarioFactory.Default.RunScenario(_ => Task.CompletedTask);

            result.Status.ShouldBe(ExecutionStatus.Passed);
            result.StatusDetails.ShouldBeNull();
            result.ExecutionException.ShouldBeNull();
            result.ExecutionTime.ShouldNotBe(ExecutionTime.None);
            result.GetSteps().ShouldBeEmpty();
        }

        [Test]
        public async Task Throwing_IgnoreException_should_result_with_ignored_scenario()
        {
            var result = await TestableScenarioFactory.Default.RunScenario(_ => throw new IgnoreException("reason"));
            result.Status.ShouldBe(ExecutionStatus.Ignored);
            result.StatusDetails.ShouldBe("Scenario Ignored: reason");
            result.ExecutionException.ShouldBeOfType<IgnoreException>();
            result.ExecutionTime.ShouldNotBe(ExecutionTime.None);
            result.GetSteps().ShouldBeEmpty();
        }

        [Test]
        public async Task Throwing_StepBypassException_should_result_with_bypassed_scenario()
        {
            var result = await TestableScenarioFactory.Default.RunScenario(_ => throw new BypassException("reason"));
            result.Status.ShouldBe(ExecutionStatus.Bypassed);
            result.StatusDetails.ShouldBe("Scenario Bypassed: reason");
            result.ExecutionException.ShouldBeOfType<BypassException>();
            result.ExecutionTime.ShouldNotBe(ExecutionTime.None);
            result.GetSteps().ShouldBeEmpty();
        }

        [Test]
        public async Task Throwing_Exception_should_result_with_failed_scenario()
        {
            var exception = new Exception("reason");
            var result = await TestableScenarioFactory.Default.RunScenario(_ => throw exception);
            result.Status.ShouldBe(ExecutionStatus.Failed);
            result.StatusDetails.ShouldBe("Scenario Failed: System.Exception: reason");
            result.ExecutionException.ShouldBe(exception);
            result.ExecutionTime.ShouldNotBe(ExecutionTime.None);
            result.GetSteps().ShouldBeEmpty();
        }

        [Test]
        public async Task Run_result_should_be_available_via_Result_property()
        {
            var scenario = TestableScenarioFactory.Default.RunScenario(_ => Task.CompletedTask);
            var result = await scenario;
            result.ShouldBeSameAs(scenario.Result);
        }
    }
}
