using LightBDD.Core.Notification;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.Core.UnitTests.Notification
{
    [TestFixture]
    public class DelegatingProgressNotifierTests
    {
        private DelegatingProgressNotifier _subject;
        private IProgressNotifier[] _notifiers;

        [SetUp]
        public void SetUp()
        {
            _notifiers = new[] { MockRepository.GenerateMock<IProgressNotifier>(), MockRepository.GenerateMock<IProgressNotifier>() };
            _subject = new DelegatingProgressNotifier(_notifiers);
        }

        [Test]
        public void It_should_delegate_NotifyFeatureStart()
        {
            var featureInfo = new Mocks.TestFeatureInfo();
            _subject.NotifyFeatureStart(featureInfo);
            foreach (var notifier in _notifiers)
                notifier.AssertWasCalled(n => n.NotifyFeatureStart(featureInfo));
        }

        [Test]
        public void It_should_delegate_NotifyFeatureFinished()
        {
            var feature = new Mocks.TestFeatureResult();
            _subject.NotifyFeatureFinished(feature);
            foreach (var notifier in _notifiers)
                notifier.AssertWasCalled(n => n.NotifyFeatureFinished(feature));
        }

        [Test]
        public void It_should_delegate_NotifyStepStart()
        {
            var stepInfo = new Mocks.TestStepInfo();
            _subject.NotifyStepStart(stepInfo);
            foreach (var notifier in _notifiers)
                notifier.AssertWasCalled(n => n.NotifyStepStart(stepInfo));
        }

        [Test]
        public void It_should_delegate_NotifyStepFinished()
        {
            var step = new Mocks.TestStepResult();
            _subject.NotifyStepFinished(step);
            foreach (var notifier in _notifiers)
                notifier.AssertWasCalled(n => n.NotifyStepFinished(step));
        }

        [Test]
        public void It_should_delegate_NotifyStepComment()
        {
            var stepInfo = new Mocks.TestStepInfo();
            var comment = "comment";
            _subject.NotifyStepComment(stepInfo, comment);
            foreach (var notifier in _notifiers)
                notifier.AssertWasCalled(n => n.NotifyStepComment(stepInfo, comment));
        }

        [Test]
        public void It_should_delegate_NotifyScenarioStart()
        {
            var scenarioInfo = new Mocks.TestScenarioInfo();
            _subject.NotifyScenarioStart(scenarioInfo);
            foreach (var notifier in _notifiers)
                notifier.AssertWasCalled(n => n.NotifyScenarioStart(scenarioInfo));
        }

        [Test]
        public void It_should_delegate_NotifyScenarioFinished_should_print_notification_and_update_stats()
        {
            var scenario = new Mocks.TestScenarioResult();
            _subject.NotifyScenarioFinished(scenario);
            foreach (var notifier in _notifiers)
                notifier.AssertWasCalled(n => n.NotifyScenarioFinished(scenario));
        }
    }
}