using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class RunnableScenario_parameterized_step_execution_tests
    {
        private readonly List<Tuple<string, object>> _executedSteps = new();

        [Test]
        public async Task Runner_should_call_steps_with_parameters()
        {
            await TestableScenarioFactory.Default.RunScenario(r => r.Test().TestScenario(
                TestStep.CreateAsync(Given_step_one, "abc"),
                TestStep.CreateAsync(When_step_two, 123),
                TestStep.CreateAsync(Then_step_three, 3.25),
                TestStep.CreateAsync(Then_another_step, (double?)3.25),
                TestStep.CreateAsync(Then_another_step, (double?)null)));

            var expected = new[]
            {
                Tuple.Create(nameof(Given_step_one), (object)"abc"),
                Tuple.Create(nameof(When_step_two), (object)123),
                Tuple.Create(nameof(Then_step_three), (object)3.25),
                Tuple.Create(nameof(Then_another_step), (object)3.25),
                Tuple.Create(nameof(Then_another_step), (object)null)
            };

            Assert.That(_executedSteps, Is.EqualTo(expected));
        }

        [Test]
        public async Task Runner_should_evaluate_step_parameters_once()
        {
            var number = 0;
            await TestableScenarioFactory.Default.RunScenario(r => r.Test().TestScenario(
                TestStep.CreateAsync(Given_step_one, () => (++number).ToString()),
                TestStep.CreateAsync(When_step_two, () => ++number),
                TestStep.CreateAsync(Then_step_three, () => (double)++number)));

            var expected = new[]
            {
                Tuple.Create(nameof(Given_step_one), (object)"1"),
                Tuple.Create(nameof(When_step_two), (object)2),
                Tuple.Create(nameof(Then_step_three), (object)3.0)
            };

            Assert.That(_executedSteps, Is.EqualTo(expected));
        }

        [Test]
        public async Task Runner_should_evaluate_step_parameters_just_before_step_execution()
        {
            var result = await TestableScenarioFactory.Default.RunScenario(r => r.Test().TestScenario(
                TestStep.CreateAsync(Given_step_one, () => "def"),
                TestStep.CreateAsync<int>(When_step_two, () => throw new Exception("reason")),
                TestStep.CreateAsync(Then_step_three, () => 3.14)));

            var ex = result.ExecutionException.ShouldBeOfType<Exception>();
            Assert.That(ex.Message, Is.EqualTo("reason"));

            var expected = new[] { Tuple.Create(nameof(Given_step_one), (object)"def") };
            Assert.That(_executedSteps, Is.EqualTo(expected));
        }

        private void Given_step_one(string parameter) { _executedSteps.Add(Tuple.Create(nameof(Given_step_one), (object)parameter)); }
        private void When_step_two(int parameter) { _executedSteps.Add(Tuple.Create(nameof(When_step_two), (object)parameter)); }
        private void Then_step_three(double parameter) { _executedSteps.Add(Tuple.Create(nameof(Then_step_three), (object)parameter)); }
        private void Then_another_step(double? parameter) { _executedSteps.Add(Tuple.Create(nameof(Then_another_step), (object)parameter)); }
    }
}