using System.Linq;
using LightBDD.Core.Extensibility;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Core.UnitTests.TestableIntegration;
using Xunit;

namespace LightBDD.Core.UnitTests
{
    public class CoreBddRunner_parameterized_step_metadata_collection_tests : Steps
    {
        private readonly IBddRunner _runner;

        public CoreBddRunner_parameterized_step_metadata_collection_tests()
        {
            _runner = new TestableBddRunner(GetType());
        }

        [Fact]
        public void It_should_capture_all_steps()
        {
            _runner.TestParameterizedScenario(
                TestSyntax.ParameterizedWithConstant(Given_step_with_parameter, "abc"),
                TestSyntax.ParameterizedWithConstant(When_step_with_parameter, 123),
                TestSyntax.ParameterizedWithConstant(Then_step_with_parameter, 3.15));

            var steps = _runner.Integrate().GetFeatureResult().GetScenarios().Single().GetSteps();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, "Given step with parameter \"abc\"", ExecutionStatus.Passed),
                new StepResultExpectation(2, "When step with parameter \"123\"", ExecutionStatus.Passed),
                new StepResultExpectation(3, "Then step with parameter \"3.15\"", ExecutionStatus.Passed)
                );
        }

        [Fact]
        public void It_should_capture_steps_with_parameters_inserted_in_proper_places()
        {
            _runner.TestParameterizedScenario(
                TestSyntax.ParameterizedWithConstant(Method_with_replaced_parameter_PARAM_in_name, "abc"),
                TestSyntax.ParameterizedWithConstant(Method_with_inserted_parameter_param_in_name, "abc"),
                TestSyntax.ParameterizedWithConstant(Method_with_appended_parameter_at_the_end_of_name, "abc"));

            var steps = _runner.Integrate().GetFeatureResult().GetScenarios().Single().GetSteps();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, "Method with replaced parameter \"abc\" in name", ExecutionStatus.Passed),
                new StepResultExpectation(2, "Method with inserted parameter param \"abc\" in name", ExecutionStatus.Passed),
                new StepResultExpectation(3, "Method with appended parameter at the end of name [param: \"abc\"]", ExecutionStatus.Passed)
            );
        }

        private void Method_with_appended_parameter_at_the_end_of_name(object param) { }
        private void Method_with_inserted_parameter_param_in_name(object param) { }
        private void Method_with_replaced_parameter_PARAM_in_name(object param){}
        private void Given_step_with_parameter(object parameter) { }
        private void When_step_with_parameter(object parameter) { }
        private void Then_step_with_parameter(object parameter) { }
    }
}