using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Core.Results;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using Moq;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_step_execution_with_context_tests
    {
        private IBddRunner _runner;
        private IFeatureRunner _feature;

        [SetUp]
        public void SetUp()
        {
            _feature = TestableFeatureRunnerRepository.GetRunner(GetType());
            _runner = _feature.GetBddRunner(this);
        }

        [Test]
        public void Runner_should_instantiate_context_just_before_run_so_its_failure_would_be_included_in_results()
        {
            Assert.Throws<InvalidOperationException>(() => _runner.Test()
                .WithContext(() => throw new InvalidOperationException("abc"), false)
                .TestScenario(Given_step_one));

            var scenario = _feature.GetFeatureResult().GetScenarios().Single();
            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Failed));
            Assert.That(scenario.StatusDetails, Is.EqualTo("Scenario: Context initialization failed: abc"));
        }

        [Test]
        public void Runner_should_instantiate_context_and_use_it_in_step_and_parameter_execution()
        {
            var someParameterInfo = GetType().GetMethod(nameof(Given_step_two), BindingFlags.NonPublic | BindingFlags.Instance).GetParameters()[0];

            var list = new List<object>();

            _runner.Test()
                .WithContext(() => new object(), false)
                .TestScenario(
                    new StepDescriptor("test1", (ctx, args) => { list.Add(ctx); return Task.FromResult(DefaultStepResultDescriptor.Instance); }, ParameterDescriptor.FromInvocation(someParameterInfo, ctx => { list.Add(ctx); return ctx; })),
                    new StepDescriptor("test1", (ctx, args) => { list.Add(ctx); return Task.FromResult(DefaultStepResultDescriptor.Instance); }, ParameterDescriptor.FromInvocation(someParameterInfo, ctx => { list.Add(ctx); return ctx; })));

            Assert.That(list.Count, Is.EqualTo(4));
            Assert.That(list.Distinct().Count, Is.EqualTo(1), "Only one instance of object should be created");
        }

        [Test]
        public void Runner_should_instantiate_context_for_every_scenario()
        {
            var list = new List<object>();

            var runnerWithContext = _runner.Test().WithContext(() => new object(), false);
            runnerWithContext.TestScenario(
                    new StepDescriptor("test1", (ctx, args) => { list.Add(ctx); return Task.FromResult(DefaultStepResultDescriptor.Instance); }),
                    new StepDescriptor("test1", (ctx, args) => { list.Add(ctx); return Task.FromResult(DefaultStepResultDescriptor.Instance); }));

            runnerWithContext.TestScenario(
                    new StepDescriptor("test1", (ctx, args) => { list.Add(ctx); return Task.FromResult(DefaultStepResultDescriptor.Instance); }),
                    new StepDescriptor("test1", (ctx, args) => { list.Add(ctx); return Task.FromResult(DefaultStepResultDescriptor.Instance); }));

            Assert.That(list.Count, Is.EqualTo(4));
            Assert.That(list.Distinct().Count, Is.EqualTo(2), "Each scenario should have own context");
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Runner_should_dispose_context_depending_on_takeOwnership_flag(bool shouldDispose)
        {
            var context = Mock.Of<IDisposable>();
            _runner.Test().WithContext(() => context, shouldDispose).TestScenario(Given_step_one);
            Mock.Get(context).Verify(x => x.Dispose(), Times.Exactly(shouldDispose ? 1 : 0));
        }

        [Test]
        public void Runner_should_propagate_context_disposal_exception()
        {
            var exception = new Exception("foo");
            var context = Mock.Of<IDisposable>();
            Mock.Get(context).Setup(x => x.Dispose()).Throws(exception);
            var ex = Assert.Throws<InvalidOperationException>(() => _runner.Test().WithContext(() => context, true).TestScenario(Given_step_one));
            Assert.That(ex.Message, Is.EqualTo($"Failed to dispose dependency '{context.GetType().Name}': foo"));
            Assert.That(ex.InnerException, Is.SameAs(exception));
            Assert.That(ex.StackTrace, Is.Not.Null);
        }

        [Test]
        public void Runner_should_propagate_context_disposal_exception_together_with_failing_scenario()
        {
            var exception = new InvalidOperationException("foo");
            var context = Mock.Of<IDisposable>();
            Mock.Get(context).Setup(x => x.Dispose()).Throws(exception);
            var ex = Assert.Throws<AggregateException>(() => _runner.Test().WithContext(() => context, true).TestScenario(Step_throwing_exception));

            Assert.That(ex.InnerExceptions.Select(x => $"{x.GetType().Name}|{x.Message}").ToArray(),
                Is.EquivalentTo(new[]
                {
                    $"{nameof(Exception)}|bar",
                    $"{nameof(InvalidOperationException)}|Failed to dispose dependency '{context.GetType().Name}': foo"
                }));
        }

        private void Step_throwing_exception()
        {
            throw new Exception("bar");
        }
        private void Given_step_one() { }
        private void Given_step_two(int parameter) { }
    }
}