using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests
{
    [TestFixture]
    [FeatureDescription("Runner tests description")]
    [Label("Ticket-1")]
    public class BDD_runner_tests : SomeSteps
    {
        private AbstractBDDRunner _subject;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _subject = new TestableBDDRunner(GetType(), MockRepository.GenerateMock<IProgressNotifier>());
        }

        #endregion

        [Test]
        public void Should_collect_feature_details()
        {
            Assert.That(_subject.Result.Name, Is.EqualTo("BDD runner tests"));
            Assert.That(_subject.Result.Label, Is.EqualTo("Ticket-1"));
            Assert.That(_subject.Result.Description, Is.EqualTo("Runner tests description"));
        }

        [Test]
        public void Should_collect_scenario_result()
        {
            _subject.RunScenario(Step_one, Step_two);
            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect scenario result"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));
            Assert.That(result.Categories, Is.Empty);

            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "Step one",  ResultStatus.Passed),
                new StepResultExpectation(2, "Step two", ResultStatus.Passed)
            });
        }

        [Test]
        [Category("Category A"), Category("Category B"), ScenarioCategory("Category C")]
        public void Should_capture_all_categories()
        {
            _subject.RunScenario(Step_one);
            Assert.That(
                _subject.Result.Scenarios.Single().Categories.ToArray(),
                Is.EquivalentTo(new[] { "Category A", "Category B", "Category C" }));
        }
        [Test]
        public void Should_collect_scenario_result_via_fluent_interfaces()
        {
            _subject.NewScenario().Run(Step_one, Step_two);

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect scenario result via fluent interfaces"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "Step one", ResultStatus.Passed),
                new StepResultExpectation(2, "Step two", ResultStatus.Passed)
            });
        }

        [Test]
        public void Should_collect_scenario_result_for_explicitly_named_scenario()
        {
            const string scenarioName = "my scenario";
#pragma warning disable 0618
            _subject.RunScenario(scenarioName, Step_one, Step_two);
#pragma warning restore 0618
            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo(scenarioName));
            Assert.That(result.Label, Is.Null);
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "Step one", ResultStatus.Passed),
                new StepResultExpectation(2, "Step two", ResultStatus.Passed)
            });
        }

        [Test]
        public void Should_collect_scenario_result_for_explicitly_named_scenario_with_label()
        {
            const string scenarioName = "my scenario";
            const string scenarioLabel = "label";
#pragma warning disable 0618
            _subject.RunScenario(scenarioName, scenarioLabel, Step_one, Step_two);
#pragma warning restore 0618
            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo(scenarioName));
            Assert.That(result.Label, Is.EqualTo(scenarioLabel));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "Step one", ResultStatus.Passed),
                new StepResultExpectation(2, "Step two", ResultStatus.Passed)
            });
        }

        [Test]
        public void Should_collect_scenario_result_for_failing_scenario()
        {
            try
            {
                _subject.RunScenario(Step_one, Step_throwing_exception, Step_two);
            }
            catch
            {
            }
            const string expectedStatusDetails = "exception text";

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect scenario result for failing scenario"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Failed));
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "Step one", ResultStatus.Passed),
                new StepResultExpectation(2, "Step throwing exception", ResultStatus.Failed, expectedStatusDetails),
                new StepResultExpectation(3, "Step two", ResultStatus.NotRun)
            });
            Assert.That(result.StatusDetails, Is.EqualTo("Step 2: " + expectedStatusDetails));
        }

        [Test]
        public void Should_collect_scenario_result_for_passing_scenario_with_bypassed_steps()
        {
            _subject.RunScenario(Step_one, Step_with_bypass, Step_with_bypass2, Step_two);

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect scenario result for passing scenario with bypassed steps"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Bypassed));
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "Step one", ResultStatus.Passed),
                new StepResultExpectation(2, "Step with bypass", ResultStatus.Bypassed, BypassReason),
                new StepResultExpectation(3, "Step with bypass2", ResultStatus.Bypassed, BypassReason2),
                new StepResultExpectation(4, "Step two", ResultStatus.Passed)
            });
            Assert.That(result.StatusDetails, Is.EqualTo(
                "Step 2: " + BypassReason + Environment.NewLine +
                "Step 3: " + BypassReason2));
        }

        [Test]
        public void Should_collect_scenario_result_for_failing_scenario_with_bypassed_steps()
        {
            try
            {
                _subject.RunScenario(Step_one, Step_with_bypass, Step_with_bypass2, Step_throwing_exception, Step_two);
            }
            catch { }

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect scenario result for failing scenario with bypassed steps"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Failed));
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "Step one", ResultStatus.Passed),
                new StepResultExpectation(2, "Step with bypass", ResultStatus.Bypassed, BypassReason),
                new StepResultExpectation(3, "Step with bypass2", ResultStatus.Bypassed, BypassReason2),
                new StepResultExpectation(4, "Step throwing exception", ResultStatus.Failed, ExceptionText),
                new StepResultExpectation(5, "Step two", ResultStatus.NotRun)
            });
            Assert.That(result.StatusDetails, Is.EqualTo(
                "Step 2: " + BypassReason + Environment.NewLine +
                "Step 3: " + BypassReason2 + Environment.NewLine +
                "Step 4: " + ExceptionText));
        }

        [Test]
        public void Should_collect_scenario_result_for_ignored_scenario_with_bypassed_steps()
        {
            try
            {
                _subject.RunScenario(Step_one, Step_with_bypass, Step_with_bypass2, Step_with_ignore_assertion, Step_two);
            }
            catch { }

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect scenario result for ignored scenario with bypassed steps"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Ignored));
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "Step one", ResultStatus.Passed),
                new StepResultExpectation(2, "Step with bypass", ResultStatus.Bypassed, BypassReason),
                new StepResultExpectation(3, "Step with bypass2", ResultStatus.Bypassed, BypassReason2),
                new StepResultExpectation(4, "Step with ignore assertion", ResultStatus.Ignored, IgnoreReason),
                new StepResultExpectation(5, "Step two", ResultStatus.NotRun)
            });
            Assert.That(result.StatusDetails, Is.EqualTo(
                "Step 2: " + BypassReason + Environment.NewLine +
                "Step 3: " + BypassReason2 + Environment.NewLine +
                "Step 4: " + IgnoreReason));
        }

        [Test]
        public void Should_collect_scenario_result_for_ignored_scenario_steps()
        {
            try
            {
                _subject.RunScenario(Step_one, Step_with_ignore_assertion, Step_two);
            }
            catch
            {
            }
            const string expectedStatusDetails = "some reason";

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect scenario result for ignored scenario steps"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Ignored));
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "Step one", ResultStatus.Passed),
                new StepResultExpectation(2, "Step with ignore assertion", ResultStatus.Ignored, expectedStatusDetails),
                new StepResultExpectation(3, "Step two", ResultStatus.NotRun)
            });
            Assert.That(result.StatusDetails, Is.EqualTo("Step 2: " + expectedStatusDetails));
        }

        [Test]
        [Label("Label 1")]
        public void Should_include_labels_in_result()
        {
            _subject.RunScenario(Step_one, Step_two);
            Assert.That(_subject.Result.Label, Is.EqualTo("Ticket-1"));
            Assert.That(_subject.Result.Scenarios.Single().Label, Is.EqualTo("Label 1"));
        }

        [Test]
        public void Should_pass_exception_to_runner_caller()
        {
            Assert.Throws<InvalidOperationException>(() => _subject.RunScenario(Step_throwing_exception));
        }

        [Test]
        public void Should_pass_ignore_exception_to_runner_caller()
        {
            Assert.Throws<IgnoreException>(() => _subject.RunScenario(Step_with_ignore_assertion));
        }

        [Test]
        public void Should_pass_inconclusive_exception_to_runner_caller()
        {
            Assert.Throws<InconclusiveException>(() => _subject.RunScenario(Step_with_inconclusive_assertion));
        }

        [Test]
        public void Should_run_scenario_be_thread_safe()
        {
            var scenarios = new List<string>();
            for (int i = 0; i < 3000; ++i)
                scenarios.Add(i.ToString(CultureInfo.InvariantCulture));

            scenarios.AsParallel().ForAll(scenario => _subject.NewScenario(scenario).Run(Step_one, Step_two));

            Assert.That(_subject.Result.Scenarios.Select(s => s.Name).ToArray(), Is.EquivalentTo(scenarios));
        }

        [Test]
        public void Should_use_console_progress_notifier_by_default()
        {
            using (new ConsoleInterceptor())
                Assert.That(new TestableBDDRunner(GetType()).ProgressNotifier, Is.InstanceOf<ConsoleProgressNotifier>());
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\n\r")]
        public void Should_not_allow_to_run_scenarios_without_name(string name)
        {
            var exception = Assert.Throws<ArgumentException>(() => _subject.NewScenario(name));
            Assert.That(exception.Message, Is.EqualTo("Unable to create scenario without name"));
        }

        [Test]
        public void AbstractBDDRunner_should_throw_if_initialized_with_null_type_parameter()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new TestableBDDRunner(null, new TestableMetadataProvider(), new ConsoleProgressNotifier()));
            Assert.That(ex.Message, Is.StringContaining("featureTestClass"));
        }

        [Test]
        public void AbstractBDDRunner_should_throw_if_initialized_with_null_metadataProvider_parameter()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new TestableBDDRunner(typeof(BDD_runner_tests), null, new ConsoleProgressNotifier()));
            Assert.That(ex.Message, Is.StringContaining("metadataProvider"));
        }

        [Test]
        public void AbstractBDDRunner_should_throw_if_initialized_with_null_progressNotifier_parameter()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new TestableBDDRunner(typeof(BDD_runner_tests), new TestableMetadataProvider(), null));
            Assert.That(ex.Message, Is.StringContaining("progressNotifier"));
        }

        [Test]
        public void Execution_results_should_print_user_friendly_output()
        {
            _subject.NewScenario("a").WithLabel("Label-1").Run(Step_one, Step_two);
            try { _subject.NewScenario("b").Run(call => Step_throwing_exception_MESSAGE("abc")); }
            catch { }

            var result = _subject.Result;
            var scenarios = result.Scenarios.ToArray();

            Assert.That(result.ToString(), Is.EqualTo("BDD runner tests [Ticket-1]"));
            Assert.That(scenarios.Select(s => s.ToString()).ToArray(), Is.EqualTo(new[] { "a [Label-1]: Passed", "b: Failed (Step 1: abc)" }));
            Assert.That(scenarios[0].Steps.Select(s => s.ToString()).ToArray(), Is.EqualTo(new[] { "1 Step one: Passed", "2 Step two: Passed" }));
            Assert.That(scenarios[1].Steps.Select(s => s.ToString()).ToArray(), Is.EqualTo(new[] { "1 CALL Step throwing exception \"abc\": Failed (abc)" }));
        }
    }
}
