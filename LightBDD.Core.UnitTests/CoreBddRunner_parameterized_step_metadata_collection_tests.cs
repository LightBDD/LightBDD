using System;
using System.Linq;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Extensibility;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Core.UnitTests.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_parameterized_step_metadata_collection_tests : Steps
    {
        private IBddRunner _runner;

        [SetUp]
        public void SetUp()
        {
            _runner = new TestableBddRunner(GetType());
        }

        [Test]
        public void It_should_capture_all_steps()
        {
            _runner.Test().TestScenario(
                TestStep.CreateAsync(Given_step_with_parameter, "abc"),
                TestStep.CreateAsync(When_step_with_parameter, 123),
                TestStep.CreateAsync(Then_step_with_parameter, 3.15));

            var steps = _runner.Integrate().GetFeatureResult().GetScenarios().Single().GetSteps();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 3, "Given step with parameter \"abc\"", ExecutionStatus.Passed),
                new StepResultExpectation(2, 3, "When step with parameter \"123\"", ExecutionStatus.Passed),
                new StepResultExpectation(3, 3, "Then step with parameter \"3.15\"", ExecutionStatus.Passed)
                );
        }

        [Test]
        public void It_should_capture_steps_with_parameters_inserted_in_proper_places()
        {
            _runner.Test().TestScenario(
                TestStep.CreateAsync(Method_with_replaced_parameter_PARAM_in_name, "abc"),
                TestStep.CreateAsync(Method_with_inserted_parameter_param_in_name, "abc"),
                TestStep.CreateAsync(Method_with_appended_parameter_at_the_end_of_name, "abc"));

            var steps = _runner.Integrate().GetFeatureResult().GetScenarios().Single().GetSteps();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 3, "Method with replaced parameter \"abc\" in name", ExecutionStatus.Passed),
                new StepResultExpectation(2, 3, "Method with inserted parameter param \"abc\" in name", ExecutionStatus.Passed),
                new StepResultExpectation(3, 3, "Method with appended parameter at the end of name [param: \"abc\"]", ExecutionStatus.Passed)
            );
        }

        [Test]
        public void It_should_capture_steps_with_non_static_parameters()
        {
            _runner.Test().TestScenario(
                TestStep.CreateAsync(Given_step_with_parameter, () => "abc"),
                TestStep.CreateAsync(When_step_with_parameter, () => 1),
                TestStep.CreateAsync(Then_step_with_parameter, () => 3.14));

            var steps = _runner.Integrate().GetFeatureResult().GetScenarios().Single().GetSteps();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 3, "Given step with parameter \"abc\"", ExecutionStatus.Passed),
                new StepResultExpectation(2, 3, "When step with parameter \"1\"", ExecutionStatus.Passed),
                new StepResultExpectation(3, 3, "Then step with parameter \"3.14\"", ExecutionStatus.Passed)
                );
        }

        [Test]
        public void It_should_capture_steps_with_non_static_parameters_and_failing_parameter_evaluation()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                _runner.Test().TestScenario(
                    TestStep.CreateAsync(Given_step_with_parameter, () => "def"),
                    TestStep.CreateAsync(When_step_with_parameter, ThrowingParameterInvocation),
                    TestStep.CreateAsync(Then_step_with_parameter, () => 3.27));
            });

            Assert.That(ex.Message, Is.EqualTo(ParameterExceptionReason));

            var steps = _runner.Integrate().GetFeatureResult().GetScenarios().Single().GetSteps();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 3, "Given step with parameter \"def\"", ExecutionStatus.Passed),
                new StepResultExpectation(2, 3, "When step with parameter \"<?>\"", ExecutionStatus.Failed, ParameterExceptionReason),
                new StepResultExpectation(3, 3, "Then step with parameter \"<?>\"", ExecutionStatus.NotRun)
                );
        }

        [Test]
        public void It_should_capture_steps_with_non_static_parameters_and_failing_step()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                _runner.Test().TestScenario(
                    TestStep.CreateAsync(Given_step_with_parameter, () => "abc"),
                    TestStep.CreateAsync(When_step_with_parameter_throwing_exception, () => 5),
                    TestStep.CreateAsync(Then_step_with_parameter, () => 3.2));
            });

            Assert.That(ex.Message, Is.EqualTo(ExceptionReason));

            var steps = _runner.Integrate().GetFeatureResult().GetScenarios().Single().GetSteps();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 3, "Given step with parameter \"abc\"", ExecutionStatus.Passed),
                new StepResultExpectation(2, 3, "When step with parameter \"5\" throwing exception", ExecutionStatus.Failed, ExceptionReason),
                new StepResultExpectation(3, 3, "Then step with parameter \"<?>\"", ExecutionStatus.NotRun)
                );
        }

        private void Method_with_appended_parameter_at_the_end_of_name(object param) { }
        private void Method_with_inserted_parameter_param_in_name(object param) { }
        private void Method_with_replaced_parameter_PARAM_in_name(object param) { }
    }
}