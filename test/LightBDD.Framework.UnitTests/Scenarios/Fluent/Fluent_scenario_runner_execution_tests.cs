using LightBDD.Framework.Extensibility;
using LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios;
using LightBDD.Framework.UnitTests.Helpers;
using Shouldly;

namespace LightBDD.Framework.UnitTests.Scenarios.Fluent
{
    [TestFixture]
    public class Fluent_scenario_runner_execution_tests : Steps
    {
        [Test]
        public async Task It_should_run_noncontextual_scenarios()
        {
            var scenario = await TestableBddRunner.Default.RunScenario(r => r
                .AddSteps(Step_one)
                .RunAsync());

            var ex = scenario.ExecutionException.ShouldBeOfType<Exception>();
            Assert.That(ex.Message, Is.EqualTo(nameof(Step_one)));
        }

        [Test]
        public async Task It_should_run_contextual_scenarios()
        {
            var scenario = await TestableBddRunner.Default.RunScenario(r => r
                .WithContext(new object())
                .AddSteps(_ => Step_one_async_action())
                .RunAsync());

            var ex = scenario.ExecutionException.ShouldBeOfType<Exception>();
            Assert.That(ex.Message, Is.EqualTo(nameof(Step_one_async_action)));
        }
    }
}