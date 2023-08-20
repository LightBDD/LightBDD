using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    public class RunnableScenario_complex_parameter_step_execution_tests : Steps
    {
        private const string ExpectedText = "expected";
        private const string ValueText = "actual";
        private const string ExceptionText = "exception";

        [Test]
        public async Task Runner_should_capture_complex_parameter_results()
        {
            var scenarioResult = await TestableScenarioFactory.Default.RunScenario(runner => runner.Test()
                    .TestScenario(
                        TestStep.CreateAsync(Step_with_parameters,
                            Complex.Failed(),
                            Complex.NotProvided(),
                            Complex.Exception())));

            scenarioResult.Status.ShouldBe(ExecutionStatus.Failed);

            var result = scenarioResult.GetSteps().Single();
            Assert.That(result.Parameters.Count, Is.EqualTo(3));
            AssertParameter(result.Parameters[0], "arg1", ParameterVerificationStatus.Failure, $"{ExpectedText}/{ValueText}");
            AssertParameter(result.Parameters[1], "arg2", ParameterVerificationStatus.NotProvided, $"{ExpectedText}");
            AssertParameter(result.Parameters[2], "arg3", ParameterVerificationStatus.Exception, $"{ExceptionText}");
        }

        [Test]
        [TestCase(ParameterVerificationStatus.Exception)]
        [TestCase(ParameterVerificationStatus.Failure)]
        [TestCase(ParameterVerificationStatus.NotProvided)]
        public async Task Runner_should_fail_step_with_non_successful_parameters(ParameterVerificationStatus status)
        {
            var scenarioResult = await TestableScenarioFactory.Default.RunScenario(runner => runner.Test()
                    .TestScenario(
                        TestStep.CreateAsync(Step_with_parameters,
                            new Complex(null, null, status, "msg1"),
                            new Complex(null, null, ParameterVerificationStatus.Success, "msg2"),
                            new Complex(null, null, status, "msg3")
                        )));

            scenarioResult.Status.ShouldBe(ExecutionStatus.Failed);
            scenarioResult.ExecutionException.ShouldBeOfType<InvalidOperationException>()
                .Message.ShouldBe($"Parameter \'arg1\' verification failed: msg1{Environment.NewLine}Parameter \'arg3\' verification failed: msg3");

            var step = scenarioResult.GetSteps().Single();
            Assert.That(step.Status, Is.EqualTo(ExecutionStatus.Failed));
            Assert.That(step.StatusDetails, Is.EqualTo($"Step 1 Failed: System.InvalidOperationException: Parameter \'arg1\' verification failed: msg1{Environment.NewLine}\tParameter \'arg3\' verification failed: msg3"));
        }

        [Test]
        public async Task Runner_should_honor_ignored_steps()
        {
            var scenarioResult = await TestableScenarioFactory.Default.RunScenario(runner => runner.Test()
                    .TestScenario(
                        TestStep.CreateAsync(Ignored_step_with_parameter,
                            new Complex(null, null, ParameterVerificationStatus.Failure, "msg")
                        )));

            Assert.That(scenarioResult.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.Ignored));
        }

        [Test]
        public async Task Runner_should_honor_bypassed_steps()
        {
            var scenarioResult = await TestableScenarioFactory.Default.RunScenario(runner => runner.Test()
                    .TestScenario(
                        TestStep.CreateAsync(Bypassed_step_with_parameter,
                            new Complex(null, null, ParameterVerificationStatus.Failure, "msg")
                        )));

            Assert.That(scenarioResult.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.Bypassed));
        }

        [Test]
        public async Task Runner_should_process_failed_steps_first()
        {
            var scenarioResult = await TestableScenarioFactory.Default.RunScenario(runner => runner.Test()
                    .TestScenario(
                        TestStep.CreateAsync(Failed_step_with_parameter,
                            new Complex(null, null, ParameterVerificationStatus.Failure, "msg")
                        )));

            var result = scenarioResult.GetSteps().Single();
            Assert.That(result.Status, Is.EqualTo(ExecutionStatus.Failed));
            Assert.That(result.StatusDetails, Is.EqualTo("Step 1 Failed: System.Exception: exception reason"));
        }

        [Test]
        public async Task Runner_should_include_complex_parameters_for_failed_steps()
        {
            var scenarioResult = await TestableScenarioFactory.Default.RunScenario(runner => runner.Test()
                    .TestScenario(
                        TestStep.CreateAsync(Failed_step_with_parameter,
                            new Complex(null, null, ParameterVerificationStatus.Failure, "msg")
                        )));

            var result = scenarioResult.GetSteps().Single();
            var parameter = result.Parameters.SingleOrDefault();
            Assert.That(parameter, Is.Not.Null);
            Assert.That(parameter.Details.VerificationStatus, Is.EqualTo(ParameterVerificationStatus.Failure));
            Assert.That(parameter.Details.VerificationMessage, Is.EqualTo("msg"));
        }

        private void AssertParameter(IParameterResult parameter, string name, ParameterVerificationStatus status, string message)
        {
            Assert.That(parameter.Name, Is.EqualTo(name));
            Assert.That(parameter.Details, Is.Not.Null);
            Assert.That(parameter.Details.VerificationStatus, Is.EqualTo(status));
            Assert.That(parameter.Details.VerificationMessage, Is.EqualTo(message));
        }

        private void Step_with_parameters(Complex arg1, Complex arg2, Complex arg3) { }
        private void Ignored_step_with_parameter(Complex arg) { StepExecution.Current.Ignore("reason"); }
        private void Bypassed_step_with_parameter(Complex arg) { StepExecution.Current.Bypass("reason"); }
        private void Failed_step_with_parameter(Complex arg) { throw new Exception("exception reason"); }


        private class Complex : IComplexParameter
        {
            public Complex(string expected, string actual, ParameterVerificationStatus status, string message = null)
            {
                Details = new TestResults.TestInlineParameterDetails(expected, actual, status, message);
            }

            public void SetValueFormattingService(IValueFormattingService formattingService)
            {
            }

            public IParameterDetails Details { get; }

            public static Complex Failed()
            {
                return new Complex(ExpectedText, ValueText, ParameterVerificationStatus.Failure, $"{ExpectedText}/{ValueText}");
            }

            public static Complex NotProvided()
            {
                return new Complex(ExpectedText, null, ParameterVerificationStatus.NotProvided, $"{ExpectedText}");
            }

            public static Complex Exception()
            {
                return new Complex(ExpectedText, null, ParameterVerificationStatus.Exception, $"{ExceptionText}");
            }
        }
    }
}