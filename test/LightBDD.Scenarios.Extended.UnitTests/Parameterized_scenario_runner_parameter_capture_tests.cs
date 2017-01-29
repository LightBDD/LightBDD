using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Scenarios.Extended.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.Scenarios.Extended.UnitTests
{
    [TestFixture]
    public class Parameterized_scenario_runner_parameter_capture_tests : ParameterizedScenariosTestBase<NoContext>
    {

        [Test]
        public void It_should_capture_constant_parameters_in_sync_mode()
        {
            ExpectSynchronousScenarioRun();
            Runner.RunScenario(_ => Step_with_parameters(32, "32"));
            var step = CapturedSteps.Single();
            Assert.That(step.Parameters.Length, Is.EqualTo(2));

            AssertParameter(step.Parameters[0], "param1", true, GetMethodParameterInfo(nameof(Step_with_parameters), 0));
            AssertParameter(step.Parameters[1], "param2", true, GetMethodParameterInfo(nameof(Step_with_parameters), 1));

            Assert.That(step.Parameters[0].ValueEvaluator(null), Is.EqualTo(32));
            Assert.That(step.Parameters[1].ValueEvaluator(null), Is.EqualTo("32"));
        }

        [Test]
        public void It_should_capture_mutable_parameters_in_sync_mode()
        {
            ExpectSynchronousScenarioRun();

            int i = 56;

            Runner.RunScenario(_ => Step_with_parameters(i, i.ToString()));
            var step = CapturedSteps.Single();
            Assert.That(step.Parameters.Length, Is.EqualTo(2));

            AssertParameter(step.Parameters[0], "param1", false, GetMethodParameterInfo(nameof(Step_with_parameters), 0));
            AssertParameter(step.Parameters[1], "param2", false, GetMethodParameterInfo(nameof(Step_with_parameters), 1));

            Assert.That(step.Parameters[0].ValueEvaluator(null), Is.EqualTo(i));
            Assert.That(step.Parameters[1].ValueEvaluator(null), Is.EqualTo(i.ToString()));
        }

        [Test]
        public async Task It_should_capture_constant_parameters_in_async_mode()
        {
            ExpectAsynchronousScenarioRun();
            await Runner.RunScenarioAsync(_ => Step_with_parameters_async(32, "32"));
            var step = CapturedSteps.Single();
            Assert.That(step.Parameters.Length, Is.EqualTo(2));

            AssertParameter(step.Parameters[0], "param1", true, GetMethodParameterInfo(nameof(Step_with_parameters_async), 0));
            AssertParameter(step.Parameters[1], "param2", true, GetMethodParameterInfo(nameof(Step_with_parameters_async), 1));

            Assert.That(step.Parameters[0].ValueEvaluator(null), Is.EqualTo(32));
            Assert.That(step.Parameters[1].ValueEvaluator(null), Is.EqualTo("32"));
        }

        [Test]
        public async Task It_should_capture_mutable_parameters_in_async_mode()
        {
            ExpectAsynchronousScenarioRun();

            int i = 56;

            await Runner.RunScenarioAsync(_ => Step_with_parameters_async(i, i.ToString()));
            var step = CapturedSteps.Single();
            Assert.That(step.Parameters.Length, Is.EqualTo(2));

            AssertParameter(step.Parameters[0], "param1", false, GetMethodParameterInfo(nameof(Step_with_parameters_async), 0));
            AssertParameter(step.Parameters[1], "param2", false, GetMethodParameterInfo(nameof(Step_with_parameters_async), 1));

            Assert.That(step.Parameters[0].ValueEvaluator(null), Is.EqualTo(i));
            Assert.That(step.Parameters[1].ValueEvaluator(null), Is.EqualTo(i.ToString()));
        }

        [Test]
        public void It_should_not_allow_ref_parameters_in_sync_mode()
        {
            ExpectSynchronousScenarioRun();
            int val = 4;
            var ex = Assert.Throws<ArgumentException>(() => Runner.RunScenario(_ => Step_with_ref_parameters(ref val)));
            Assert.That(ex.Message, Does.Match($"Steps accepting ref or out parameters are not supported: _ => .*{nameof(Step_with_ref_parameters)}.*"));
        }

        [Test]
        public void It_should_not_allow_out_parameters_in_sync_mode()
        {
            ExpectSynchronousScenarioRun();
            int val;
            var ex = Assert.Throws<ArgumentException>(() => Runner.RunScenario(_ => Step_with_out_parameters(out val)));
            Assert.That(ex.Message, Does.Match($"Steps accepting ref or out parameters are not supported: _ => .*{nameof(Step_with_out_parameters)}.*"));
        }

        [Test]
        [TestCase("abc")]
        [TestCase("def")]
        public void It_should_capture_method_parameter(string parameter)
        {
            ExpectSynchronousScenarioRun();
            Runner.RunScenario(_ => Some_step(parameter));

            Assert.That(CapturedSteps.Single().Parameters.Single().ValueEvaluator(null), Is.EqualTo(parameter));
        }

        [Test]
        public void It_should_capture_method_call()
        {
            ExpectSynchronousScenarioRun();
            Runner.RunScenario(_ => Some_step(GetType().ToString()));

            Assert.That(CapturedSteps.Single().Parameters.Single().ValueEvaluator(null), Is.EqualTo(GetType().ToString()));
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
    }
}