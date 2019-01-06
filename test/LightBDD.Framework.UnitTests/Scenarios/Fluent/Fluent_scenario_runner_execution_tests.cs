using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.Framework.Scenarios.Contextual;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;
using System;

namespace LightBDD.Framework.UnitTests.Scenarios.Fluent
{
    [TestFixture]
    public class Fluent_scenario_runner_execution_tests : Steps
    {
        private IBddRunner _runner;

        [SetUp]
        public void SetUp()
        {
            _runner = TestableFeatureRunnerRepository.GetRunner(GetType()).GetBddRunner(this);
        }

        [Test]
        public void It_should_run_noncontextual_scenarios()
        {
            var ex = Assert.ThrowsAsync<Exception>(() => _runner
                .AddSteps(Step_one)
                .RunAsync());

            Assert.That(ex.Message, Is.EqualTo(nameof(Step_one)));
        }

        [Test]
        public void It_should_run_contextual_scenarios()
        {
            var ex = Assert.ThrowsAsync<Exception>(() => _runner
                .WithContext(new object())
                .AddSteps(_ => Step_one_async_action())
                .RunAsync());

            Assert.That(ex.Message, Is.EqualTo(nameof(Step_one_async_action)));
        }
    }
}