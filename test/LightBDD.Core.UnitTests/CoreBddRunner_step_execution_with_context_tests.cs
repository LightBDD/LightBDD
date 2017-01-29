using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_step_execution_with_context_tests
    {
        private IBddRunner _runner;
        private IFeatureBddRunner _feature;

        [SetUp]
        public void SetUp()
        {
            _feature = TestableFeatureBddRunnerFactory.GetRunner(GetType());
            _runner = _feature.GetRunner(this);
        }

        [Test]
        public void Runner_should_instantiate_context_just_before_run_so_its_failure_would_be_included_in_results()
        {
            Assert.Throws<InvalidOperationException>(() => _runner.Test()
                .WithContext(() => { throw new InvalidOperationException("abc"); })
                .TestScenario(Given_step_one));

            var scenario = _feature.GetFeatureResult().GetScenarios().Single();
            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Failed));
            Assert.That(scenario.StatusDetails, Is.EqualTo("Context initialization failed: abc"));
        }

        [Test]
        public void Runner_should_instantiate_context_and_use_it_in_step_and_parameter_execution()
        {
            var someParameterInfo = GetType().GetMethod(nameof(Given_step_two), BindingFlags.NonPublic | BindingFlags.Instance).GetParameters()[0];

            var list = new List<object>();

            _runner.Test()
                .WithContext(() => new object())
                .TestScenario(
                    new StepDescriptor("test1", (ctx, args) => { list.Add(ctx); return Task.CompletedTask; }, ParameterDescriptor.FromInvocation(someParameterInfo, ctx => { list.Add(ctx); return ctx; })),
                    new StepDescriptor("test1", (ctx, args) => { list.Add(ctx); return Task.CompletedTask; }, ParameterDescriptor.FromInvocation(someParameterInfo, ctx => { list.Add(ctx); return ctx; })));

            Assert.That(list.Count, Is.EqualTo(4));
            Assert.That(list.Distinct().Count, Is.EqualTo(1), "Only one instance of object should be created");
        }

        [Test]
        public void Runner_should_instantiate_context_for_every_scenario()
        {
            var list = new List<object>();

            var runnerWithContext = _runner.Test().WithContext(() => new object());
            runnerWithContext.TestScenario(
                    new StepDescriptor("test1", (ctx, args) => { list.Add(ctx); return Task.CompletedTask; }),
                    new StepDescriptor("test1", (ctx, args) => { list.Add(ctx); return Task.CompletedTask; }));

            runnerWithContext.TestScenario(
                    new StepDescriptor("test1", (ctx, args) => { list.Add(ctx); return Task.CompletedTask; }),
                    new StepDescriptor("test1", (ctx, args) => { list.Add(ctx); return Task.CompletedTask; }));

            Assert.That(list.Count, Is.EqualTo(4));
            Assert.That(list.Distinct().Count, Is.EqualTo(2), "Each scenario should have own context");
        }

        private void Given_step_one() { }
        private void Given_step_two(int parameter) { }
    }
}