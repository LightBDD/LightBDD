using System;
using System.IO;
using System.Text;
using LightBDD.Formatters;
using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.Results.Implementation;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests.Notification
{
    [TestFixture]
    public class ConsoleProgressNotifierTests
    {
        private TextWriter _originalOut;
        private StringBuilder _buffer;
        private IProgressNotifier _subject;

        [SetUp]
        public void SetUp()
        {
            _originalOut = Console.Out;
            _buffer = new StringBuilder();
            Console.SetOut(new StringWriter(_buffer));
            _subject = new ConsoleProgressNotifier();
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetOut(_originalOut);
        }

        [Test]
        public void NotifyFeatureStart_should_print_feature_full_details()
        {
            string featureName = "feature name";
            string featureDescription = "some description";
            string label = "MY-LABEL-1";
            string expectedText = string.Format("FEATURE: [{0}] {1}{2}  {3}{2}", label, featureName, Environment.NewLine, featureDescription);

            _subject.NotifyFeatureStart(featureName, featureDescription, label);
            Assert.That(_buffer.ToString(), Is.EqualTo(expectedText));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void NotifyFeatureStart_should_print_feature_details_if_no_label_is_provided(string emptyLabel)
        {
            string featureName = "feature name";
            string featureDescription = "some description";
            string expectedText = string.Format("FEATURE: {0}{1}  {2}{1}", featureName, Environment.NewLine, featureDescription);

            _subject.NotifyFeatureStart(featureName, featureDescription, emptyLabel);
            Assert.That(_buffer.ToString(), Is.EqualTo(expectedText));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void NotifyFeatureStart_should_print_feature_details_if_no_description_is_provided(string emptyDescription)
        {
            string featureName = "feature name";
            string label = "MY-LABEL-1";
            string expectedText = string.Format("FEATURE: [{0}] {1}{2}", label, featureName, Environment.NewLine);

            _subject.NotifyFeatureStart(featureName, emptyDescription, label);
            Assert.That(_buffer.ToString(), Is.EqualTo(expectedText));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void NotifyFeatureStart_should_print_feature_details_if_no_description_nor_label_is_provided(string emptyDetails)
        {
            string featureName = "feature name";
            string expectedText = string.Format("FEATURE: {0}{1}", featureName, Environment.NewLine);

            _subject.NotifyFeatureStart(featureName, emptyDetails, emptyDetails);
            Assert.That(_buffer.ToString(), Is.EqualTo(expectedText));
        }

        [Test]
        public void NotifyScenarioStart_should_print_scenario_full_details()
        {
            string scenarioName = "scenario name";
            string label = "MY-LABEL-1";
            string expectedText = string.Format("SCENARIO: [{0}] {1}{2}", label, scenarioName, Environment.NewLine);

            _subject.NotifyScenarioStart(scenarioName, label);
            Assert.That(_buffer.ToString(), Is.EqualTo(expectedText));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void NotifyScenarioStart_should_print_scenario_details_if_no_label_is_provided(string emptyLabel)
        {
            string scenarioName = "scenario name";
            string expectedText = string.Format("SCENARIO: {0}{1}", scenarioName, Environment.NewLine);

            _subject.NotifyScenarioStart(scenarioName, emptyLabel);
            Assert.That(_buffer.ToString(), Is.EqualTo(expectedText));
        }

        [Test]
        [TestCase(ResultStatus.Failed)]
        [TestCase(ResultStatus.Ignored)]
        [TestCase(ResultStatus.Passed)]
        [TestCase(ResultStatus.NotRun)]
        public void NotifyScenarioFinished_should_print_scenario_result(ResultStatus status)
        {
            var executionTime = new TimeSpan(0, 0, 27);
            string expectedText = string.Format("  SCENARIO RESULT: {0} after {1}{2}", status, executionTime.FormatPretty(), Environment.NewLine);

            var result = MockRepository.GenerateMock<IScenarioResult>();
            result.Stub(r => r.Status).Return(status);
            result.Stub(r => r.ExecutionTime).Return(executionTime);

            _subject.NotifyScenarioFinished(result);
            Assert.That(_buffer.ToString(), Is.EqualTo(expectedText));
        }

        [Test]
        [TestCase(ResultStatus.Failed)]
        [TestCase(ResultStatus.Ignored)]
        [TestCase(ResultStatus.Passed)]
        [TestCase(ResultStatus.NotRun)]
        public void NotifyScenarioFinished_should_print_scenario_result_with_provided_details(ResultStatus status)
        {
            var executionTime = new TimeSpan(0, 0, 27);
            string details = @"expected: A
got: B";
            string expectedText = string.Format("  SCENARIO RESULT: {0} after {1}{2}    expected: A{2}    got: B{2}", status, executionTime.FormatPretty(), Environment.NewLine);

            var result = MockRepository.GenerateMock<IScenarioResult>();
            result.Stub(r => r.Status).Return(status);
            result.Stub(r => r.ExecutionTime).Return(executionTime);
            result.Stub(r => r.StatusDetails).Return(details);

            _subject.NotifyScenarioFinished(result);
            Assert.That(_buffer.ToString(), Is.EqualTo(expectedText));
        }

        [Test]
        public void NotifyStepStart_should_print_step_details()
        {
            string stepName = "scenario name";
            var stepNumber = 12;
            var totalStepCount = 19;
            string expectedText = string.Format("  STEP {0}/{1}: {2}{3}", stepNumber, totalStepCount, stepName, Environment.NewLine);

            _subject.NotifyStepStart(stepName, stepNumber, totalStepCount);
            Assert.That(_buffer.ToString(), Is.EqualTo(expectedText));
        }

        [Test]
        public void NotifyStepFinished_should_print_step_details()
        {
            string stepName = "scenario name";
            var stepNumber = 12;
            var totalStepCount = 19;

            var resultStatus = ResultStatus.Failed;
            var executionTime = new TimeSpan(0, 0, 0, 0, 127);
            string expectedText = string.Format("  STEP {0}/{1}: {2} after {3}{4}", stepNumber, totalStepCount, resultStatus, executionTime.FormatPretty(), Environment.NewLine);

            _subject.NotifyStepFinished(new StepResult(stepNumber, new StepName(stepName), resultStatus).SetExecutionTime(executionTime), totalStepCount);
            Assert.That(_buffer.ToString(), Is.EqualTo(expectedText));
        }
    }
}