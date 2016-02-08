using System;
using System.Collections.Generic;
using LightBDD.Core.UnitTests.TestableIntegration;
using Xunit;

namespace LightBDD.Core.UnitTests
{
    public class CoreBddRunner_parameterized_step_execution_tests
    {
        private readonly IBddRunner _runner;
        private readonly List<Tuple<string, object>> _executedSteps = new List<Tuple<string, object>>();

        public CoreBddRunner_parameterized_step_execution_tests()
        {
            _runner = new TestableBddRunner(GetType());
        }

        [Fact]
        public void Runner_should_call_steps_with_parameters()
        {
            _runner.TestParameterizedScenario(
                TestSyntax.ParameterizedWithConstant(Given_step_one, "abc"),
                TestSyntax.ParameterizedWithConstant(When_step_two, 123),
                TestSyntax.ParameterizedWithConstant(Then_step_three, 3.25));

            var expected = new[]
            {
                Tuple.Create("Given_step_one", (object)"abc"),
                Tuple.Create("When_step_two", (object)123),
                Tuple.Create("Then_step_three", (object)3.25)
            };

            Assert.Equal(expected, _executedSteps);
        }

        [Fact]
        public void Runner_should_evaluate_step_parameters_once()
        {
            int number = 0;
            _runner.TestParameterizedScenario(
                TestSyntax.ParameterizedWithFunction(Given_step_one, () => ++number),
                TestSyntax.ParameterizedWithFunction(When_step_two, () => ++number),
                TestSyntax.ParameterizedWithFunction(Then_step_three, () => ++number));

            var expected = new[]
            {
                Tuple.Create("Given_step_one", (object)1),
                Tuple.Create("When_step_two", (object)2),
                Tuple.Create("Then_step_three", (object)3)
            };

            Assert.Equal(expected, _executedSteps);
        }

        [Fact]
        public void Runner_should_evaluate_step_parameters_just_before_step_execution()
        {
            var ex = Assert.Throws<Exception>(() => _runner.TestParameterizedScenario(
                TestSyntax.ParameterizedWithFunction(Given_step_one, () => 5),
                TestSyntax.ParameterizedWithFunction(When_step_two, () => { throw new Exception("reason"); }),
                TestSyntax.ParameterizedWithFunction(Then_step_three, () => 3)));

            Assert.Equal("reason", ex.Message);

            var expected = new[] { Tuple.Create("Given_step_one", (object)5) };
            Assert.Equal(expected, _executedSteps);
        }

        private void Given_step_one(object parameter) { _executedSteps.Add(Tuple.Create("Given_step_one", parameter)); }
        private void When_step_two(object parameter) { _executedSteps.Add(Tuple.Create("When_step_two", parameter)); }
        private void Then_step_three(object parameter) { _executedSteps.Add(Tuple.Create("Then_step_three", parameter)); }
    }
}