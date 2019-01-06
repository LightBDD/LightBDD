using LightBDD.Core.Extensibility;
using LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended
{
    [TestFixture]
    public class Extended_scenario_runner_parameter_capture_tests : ExtendedScenariosTestBase<NoContext>
    {
        [Test]
        public void It_should_capture_constant_parameters_in_sync_mode()
        {
            var (stepsCapture, _) = ExpectExtendedScenarioRun();

            Runner.RunScenario(_ => Step_with_parameters(32, "32"));
            var step = stepsCapture.Single();
            Assert.That(step.Parameters.Length, Is.EqualTo(2));

            AssertParameter(step.Parameters[0], "param1", true, GetMethodParameterInfo(nameof(Step_with_parameters), 0));
            AssertParameter(step.Parameters[1], "param2", true, GetMethodParameterInfo(nameof(Step_with_parameters), 1));

            Assert.That(step.Parameters[0].ValueEvaluator(null), Is.EqualTo(32));
            Assert.That(step.Parameters[1].ValueEvaluator(null), Is.EqualTo("32"));
        }

        [Test]
        public void It_should_capture_mutable_parameters_in_sync_mode()
        {
            var (stepsCapture, _) = ExpectExtendedScenarioRun();
            var i = 56;

            Runner.RunScenario(_ => Step_with_parameters(i, i.ToString()));
            var step = stepsCapture.Single();
            Assert.That(step.Parameters.Length, Is.EqualTo(2));

            AssertParameter(step.Parameters[0], "param1", false, GetMethodParameterInfo(nameof(Step_with_parameters), 0));
            AssertParameter(step.Parameters[1], "param2", false, GetMethodParameterInfo(nameof(Step_with_parameters), 1));

            Assert.That(step.Parameters[0].ValueEvaluator(null), Is.EqualTo(i));
            Assert.That(step.Parameters[1].ValueEvaluator(null), Is.EqualTo(i.ToString()));
        }

        [Test]
        public async Task It_should_capture_constant_parameters_in_async_mode()
        {
            var (stepsCapture, _) = ExpectExtendedScenarioRun();
            await Runner.RunScenarioAsync(_ => Step_with_parameters_async(32, "32"));
            var step = stepsCapture.Single();
            Assert.That(step.Parameters.Length, Is.EqualTo(2));

            AssertParameter(step.Parameters[0], "param1", true, GetMethodParameterInfo(nameof(Step_with_parameters_async), 0));
            AssertParameter(step.Parameters[1], "param2", true, GetMethodParameterInfo(nameof(Step_with_parameters_async), 1));

            Assert.That(step.Parameters[0].ValueEvaluator(null), Is.EqualTo(32));
            Assert.That(step.Parameters[1].ValueEvaluator(null), Is.EqualTo("32"));
        }

        [Test]
        public async Task It_should_capture_mutable_parameters_in_async_mode()
        {
            var (stepsCapture, _) = ExpectExtendedScenarioRun();

            var i = 56;

            await Runner.RunScenarioAsync(_ => Step_with_parameters_async(i, i.ToString()));
            var step = stepsCapture.Single();
            Assert.That(step.Parameters.Length, Is.EqualTo(2));

            AssertParameter(step.Parameters[0], "param1", false, GetMethodParameterInfo(nameof(Step_with_parameters_async), 0));
            AssertParameter(step.Parameters[1], "param2", false, GetMethodParameterInfo(nameof(Step_with_parameters_async), 1));

            Assert.That(step.Parameters[0].ValueEvaluator(null), Is.EqualTo(i));
            Assert.That(step.Parameters[1].ValueEvaluator(null), Is.EqualTo(i.ToString()));
        }

        [Test]
        public void It_should_not_allow_ref_parameters_in_sync_mode()
        {
            ExpectExtendedScenarioRun();
            var val = 4;
            var ex = Assert.Throws<ArgumentException>(() => Runner.RunScenario(_ => Step_with_ref_parameters(ref val)));
            Assert.That(ex.Message, Does.Match($"Steps accepting ref or out parameters are not supported: _ => .*{nameof(Step_with_ref_parameters)}.*"));
        }

        [Test]
        public void It_should_not_allow_out_parameters_in_sync_mode()
        {
            ExpectExtendedScenarioRun();
            int val;
            var ex = Assert.Throws<ArgumentException>(() => Runner.RunScenario(_ => Step_with_out_parameters(out val)));
            Assert.That(ex.Message, Does.Match($"Steps accepting ref or out parameters are not supported: _ => .*{nameof(Step_with_out_parameters)}.*"));
        }

        [Test]
        [TestCase("abc")]
        [TestCase("def")]
        public void It_should_capture_method_parameter(string parameter)
        {
            var (stepsCapture, _) = ExpectExtendedScenarioRun();

            Runner.RunScenario(_ => Some_step(parameter));

            Assert.That(stepsCapture.Single().Parameters.Single().ValueEvaluator(null), Is.EqualTo(parameter));
        }

        [Test]
        public void It_should_capture_method_call()
        {
            var (stepsCapture, _) = ExpectExtendedScenarioRun();

            Runner.RunScenario(_ => Some_step(GetType().ToString()));

            Assert.That(stepsCapture.Single().Parameters.Single().ValueEvaluator(null), Is.EqualTo(GetType().ToString()));
        }

        private ParameterInfo GetMethodParameterInfo(string methodName, int parameterIndex)
        {
            return GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).GetParameters()[parameterIndex];
        }

        private void AssertParameter(ParameterDescriptor parameter, string rawName, bool isConstant, ParameterInfo parameterInfo)
        {
            Assert.That(parameter.RawName, Is.EqualTo(rawName));
            Assert.That(parameter.IsConstant, Is.EqualTo(isConstant));
            Assert.That(FormatParameterInfo(parameter.ParameterInfo), Is.EqualTo(FormatParameterInfo(parameterInfo)));
        }

        private static string FormatParameterInfo(ParameterInfo parameterInfo)
        {
            return $"{parameterInfo.ParameterType} {parameterInfo.Name}";
        }

        private void Some_step(string parameter)
        {
        }

        [Test]
        public void It_should_capture_extension_method_parameters_in_sync_mode()
        {
            var (stepsCapture, _) = ExpectExtendedScenarioRun();
            var expectedValue = 5;
            Runner.RunScenario(x => this.Extension_method_with_parameter(expectedValue));

            var step = stepsCapture.Single();
            Assert.That(step.Parameters.Length, Is.EqualTo(2));
            Assert.That(step.Parameters[0].ValueEvaluator.Invoke(null), Is.SameAs(this));
            Assert.That(step.Parameters[1].ValueEvaluator.Invoke(null), Is.EqualTo(expectedValue));
        }

        [Test]
        public async Task It_should_capture_extension_method_parameters_in_async_mode()
        {
            var (stepsCapture, _) = ExpectExtendedScenarioRun();
            var expectedValue = 5;
            await Runner.RunScenarioAsync(x => this.Extension_method_with_parameter_async(expectedValue));

            var step = stepsCapture.Single();
            Assert.That(step.Parameters.Length, Is.EqualTo(2));
            Assert.That(step.Parameters[0].ValueEvaluator.Invoke(null), Is.SameAs(this));
            Assert.That(step.Parameters[1].ValueEvaluator.Invoke(null), Is.EqualTo(expectedValue));
        }
    }


    static class MyContextExtensions
    {
        public static void Extension_method_with_parameter(
            this Extended_scenario_runner_parameter_capture_tests ctx,
            int parameter)
        {
        }

        public static Task Extension_method_with_parameter_async(
            this Extended_scenario_runner_parameter_capture_tests ctx,
            int parameter)
        {
            return Task.FromResult(0);
        }
    }
}