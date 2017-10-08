using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_hierarchical_step_execution_with_context_tests : Steps
    {
        #region Setup/Teardown

        private IFeatureRunner _feature;
        private IBddRunner _runner;
        private List<(string step, object context)> _capturedSteps;

        [SetUp]
        public void SetUp()
        {
            _feature = TestableFeatureRunnerRepository.GetRunner(GetType());
            _runner = _feature.GetBddRunner(this);
            _capturedSteps = new List<(string step, object context)>();
        }

        #endregion

        [Test]
        public void Runner_should_execute_composite_steps_with_own_context_objects()
        {
            _runner.Test().TestGroupScenario(
                StepGroupWithContext1,
                StepGroupWithContext2,
                StepGroupWithoutContext);

            var stepGroups = _capturedSteps.GroupBy(x => x.context).ToArray();
            Assert.That(stepGroups.Length, Is.EqualTo(3), "There should be 3 different contexts");
            Assert.That(stepGroups.All(x => x.Count() == 2), Is.True, "There should be 2 steps per context");
            Assert.That(stepGroups.Any(x => x.Key == null), Is.True, "One group should have no context");

            var actualSteps = stepGroups.Select(x => x.Select(y => y.step).ToArray()).ToArray();
            var expectedSteps = new[]
            {
                new []{"step1","step2"},
                new []{"stepA","stepB"},
                new []{"stepN1","stepN2"}
            };

            Assert.That(actualSteps, Is.EqualTo(expectedSteps));
        }

        [Test]
        public void Runner_should_instantiate_context_just_before_run_so_its_failure_would_be_included_in_results()
        {
            Assert.Throws<InvalidOperationException>(() =>  _runner.Test().TestGroupScenario(StepGroupWithInvalidContext));

            var scenario = _feature.GetFeatureResult().GetScenarios().Single();
            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Failed));
            Assert.That(scenario.StatusDetails, Is.EqualTo("Step 1: Sub-steps context initialization failed: abc"));
        }

        private TestCompositeStep StepGroupWithInvalidContext()
        {
            return new TestCompositeStep(
                () => throw new Exception("abc"),
                MakeStep("step"));
        }

        private TestCompositeStep StepGroupWithContext1()
        {
            return new TestCompositeStep(
                () => Guid.NewGuid(),
                MakeStep("step1"),
                MakeStep("step2"));
        }

        private TestCompositeStep StepGroupWithContext2()
        {
            return new TestCompositeStep(
                () => Guid.NewGuid(),
                MakeStep("stepA"),
                MakeStep("stepB"));
        }

        private TestCompositeStep StepGroupWithoutContext()
        {
            return new TestCompositeStep(
                MakeStep("stepN1"),
                MakeStep("stepN2"));
        }

        private StepDescriptor MakeStep(string name)
        {
            return new StepDescriptor(name, (ctx, args) =>
            {
                _capturedSteps.Add((name, ctx));
                return Task.FromResult(DefaultStepResultDescriptor.Instance);
            });
        }
    }
}