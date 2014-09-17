using System.Linq;
using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.Results.Implementation;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests
{
    [TestFixture]
    [FeatureDescription("Runner tests description")]
    [Label("Ticket-1")]
    public class BDD_runner_tests_with_context
    {
        private AbstractBDDRunner _subject;
        private IProgressNotifier _progressNotifier;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _progressNotifier = MockRepository.GenerateMock<IProgressNotifier>();
            _subject = new TestableBDDRunner(GetType(), _progressNotifier);
        }

        #endregion

        class CustomContext
        {
            public int Shared { get; set; }
        }

        [Test]
        public void Should_execute_tests_with_shared_context()
        {
            _subject.RunScenario<CustomContext>(
                Given_shared_value_is_5,
                Then_shared_value_is_passed_to_second_step_and_still_match_5);
        }

        [Test]
        public void Should_execute_tests_with_shared_context_passed_explicitly()
        {
            _subject.RunScenario(new CustomContext { Shared = 3 },
                Step_checking_shared_value_eq_3);
        }

        [Test]
        [Label("Label-1")]
        public void Should_collect_results_for_scenario_with_shared_context_passed_explicitly()
        {
            _subject.RunScenario(new CustomContext { Shared = 3 },
                Step_checking_shared_value_eq_3);

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect results for scenario with shared context passed explicitly"));
            Assert.That(result.Label, Is.EqualTo("Label-1"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));
            Assert.That(result.Steps, Is.EqualTo(new[]
            {
                new StepResult(1, "Step checking shared value eq 3", ResultStatus.Passed)
            }));
        }

        [Test]
        [Label("Label-12")]
        public void Should_collect_results_for_scenario_with_shared_context()
        {
            _subject.RunScenario<CustomContext>(
                Given_shared_value_is_5,
                Then_shared_value_is_passed_to_second_step_and_still_match_5);

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect results for scenario with shared context"));
            Assert.That(result.Label, Is.EqualTo("Label-12"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));
            Assert.That(result.Steps, Is.EqualTo(new[]
            {
                new StepResult(1, "Given shared value is 5", ResultStatus.Passed),
                new StepResult(2, "Then shared value is passed to second step and still match 5", ResultStatus.Passed)
            }));
        }

        [Test]
        public void Should_collect_results_for_customized_scenario_with_shared_context()
        {
            const string scenarioName = "Scenario name";
            const string label = "Label-12";

            _subject.RunScenario<CustomContext>(scenarioName, label,
                Given_shared_value_is_5,
                Then_shared_value_is_passed_to_second_step_and_still_match_5);

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo(scenarioName));
            Assert.That(result.Label, Is.EqualTo(label));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));
            Assert.That(result.Steps, Is.EqualTo(new[]
            {
                new StepResult(1, "Given shared value is 5", ResultStatus.Passed),
                new StepResult(2, "Then shared value is passed to second step and still match 5", ResultStatus.Passed)
            }));
        }

        [Test]
        public void Should_collect_results_for_customized_scenario_with_shared_context_but_no_label()
        {
            const string scenarioName = "Scenario name";

            _subject.RunScenario<CustomContext>(scenarioName,
                Given_shared_value_is_5,
                Then_shared_value_is_passed_to_second_step_and_still_match_5);

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo(scenarioName));
            Assert.That(result.Label, Is.Null);
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));
            Assert.That(result.Steps, Is.EqualTo(new[]
            {
                new StepResult(1, "Given shared value is 5", ResultStatus.Passed),
                new StepResult(2, "Then shared value is passed to second step and still match 5", ResultStatus.Passed)
            }));
        }

        [Test]
        public void Should_collect_results_for_customized_scenario_with_shared_context_passed_explicitly()
        {
            const string scenarioName = "Scenario name";
            const string label = "Label-1";

            _subject.RunScenario(new CustomContext { Shared = 3 }, scenarioName, label,
                Step_checking_shared_value_eq_3);

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo(scenarioName));
            Assert.That(result.Label, Is.EqualTo(label));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));
            Assert.That(result.Steps, Is.EqualTo(new[]
            {
                new StepResult(1, "Step checking shared value eq 3", ResultStatus.Passed)
            }));
        }

        [Test]
        public void Should_collect_results_for_customized_scenario_with_shared_context_passed_explicitly_but_no_label()
        {
            const string scenarioName = "Scenario name";

            _subject.RunScenario(new CustomContext { Shared = 3 }, scenarioName,
                Step_checking_shared_value_eq_3);

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo(scenarioName));
            Assert.That(result.Label, Is.Null);
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));
            Assert.That(result.Steps, Is.EqualTo(new[]
            {
                new StepResult(1, "Step checking shared value eq 3", ResultStatus.Passed)
            }));
        }

        private void Step_checking_shared_value_eq_3(CustomContext ctx)
        {
            Assert.That(ctx.Shared, Is.EqualTo(3));
        }

        private void Then_shared_value_is_passed_to_second_step_and_still_match_5(CustomContext ctx)
        {
            Assert.That(ctx.Shared, Is.EqualTo(5));
        }

        private void Given_shared_value_is_5(CustomContext ctx)
        {
            ctx.Shared = 5;
        }
    }
}