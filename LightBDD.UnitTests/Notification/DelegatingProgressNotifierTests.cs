using LightBDD.Notification;
using LightBDD.Results;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests.Notification
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
        public void Notifiers_should_return_configured_ones()
        {
            Assert.That(_subject.Notifiers, Is.SameAs(_notifiers));
        }

        [Test]
        public void It_should_delegate_NotifyFeatureStart()
        {
            _subject.NotifyFeatureStart("a", "b", "c");
            foreach (var notifier in _notifiers)
                notifier.AssertWasCalled(n => n.NotifyFeatureStart("a", "b", "c"));
        }

        [Test]
        public void It_should_delegate_NotifyStepStart()
        {
            _subject.NotifyStepStart("a", 1, 2);
            foreach (var notifier in _notifiers)
                notifier.AssertWasCalled(n => n.NotifyStepStart("a", 1, 2));
        }

        [Test]
        public void It_should_delegate_NotifyStepFinished()
        {
            var result = MockRepository.GenerateMock<IStepResult>();
            _subject.NotifyStepFinished(result, 1);
            foreach (var notifier in _notifiers)
                notifier.AssertWasCalled(n => n.NotifyStepFinished(result, 1));
        }

        [Test]
        public void It_should_delegate_NotifyStepComment()
        {
            _subject.NotifyStepComment(1, 1, "a");
            foreach (var notifier in _notifiers)
                notifier.AssertWasCalled(n => n.NotifyStepComment(1, 1, "a"));
        }

        [Test]
        public void It_should_delegate_NotifyScenarioStart()
        {
            _subject.NotifyScenarioStart("name", "l");
            foreach (var notifier in _notifiers)
                notifier.AssertWasCalled(n => n.NotifyScenarioStart("name", "l"));
        }

        [Test]
        public void It_should_delegate_NotifyScenarioFinished_should_print_notification_and_update_stats()
        {
            var result = CreateScenarioResult(ResultStatus.Failed, "name");
            _subject.NotifyScenarioFinished(result);
            foreach (var notifier in _notifiers)
                notifier.AssertWasCalled(n => n.NotifyScenarioFinished(result));
        }

        private static IScenarioResult CreateScenarioResult(ResultStatus status, string name)
        {
            var result = MockRepository.GenerateMock<IScenarioResult>();
            result.Stub(r => r.Name).Return(name);
            result.Stub(r => r.Status).Return(status);
            return result;
        }
    }
}