using System.Linq;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace LightBDD.Core.UnitTests.Notification
{
    [TestFixture]
    public class DelegatingProgressNotifierTests
    {
        private DelegatingProgressNotifier _subject;
        private Mock<IProgressNotifier>[] _notifiers;
        private readonly IFixture _autoFixture = new Fixture().Customize(new AutoMoqCustomization());

        [SetUp]
        public void SetUp()
        {
            _notifiers = new[] { new Mock<IProgressNotifier>(), new Mock<IProgressNotifier>() };
            _subject = new DelegatingProgressNotifier(_notifiers.Select(n => n.Object).ToArray());
        }

        [Test]
        public void It_should_delegate_NotifyFeatureStart()
        {
            var featureInfo = _autoFixture.Create<IFeatureInfo>();
            _subject.NotifyFeatureStart(featureInfo);
            foreach (var notifier in _notifiers)
                notifier.Verify(n => n.NotifyFeatureStart(featureInfo));
        }

        [Test]
        public void It_should_delegate_NotifyFeatureFinished()
        {
            var feature = _autoFixture.Create<IFeatureResult>();
            _subject.NotifyFeatureFinished(feature);
            foreach (var notifier in _notifiers)
                notifier.Verify(n => n.NotifyFeatureFinished(feature));
        }

        [Test]
        public void It_should_delegate_NotifyStepStart()
        {
            var stepInfo = _autoFixture.Create<IStepInfo>();
            _subject.NotifyStepStart(stepInfo);
            foreach (var notifier in _notifiers)
                notifier.Verify(n => n.NotifyStepStart(stepInfo));
        }

        [Test]
        public void It_should_delegate_NotifyStepFinished()
        {
            var step = _autoFixture.Create<IStepResult>();
            _subject.NotifyStepFinished(step);
            foreach (var notifier in _notifiers)
                notifier.Verify(n => n.NotifyStepFinished(step));
        }

        [Test]
        public void It_should_delegate_NotifyStepComment()
        {
            var stepInfo = _autoFixture.Create<IStepInfo>();
            var comment = "comment";
            _subject.NotifyStepComment(stepInfo, comment);
            foreach (var notifier in _notifiers)
                notifier.Verify(n => n.NotifyStepComment(stepInfo, comment));
        }

        [Test]
        public void It_should_delegate_NotifyScenarioStart()
        {
            var scenarioInfo = _autoFixture.Create<IScenarioInfo>();
            _subject.NotifyScenarioStart(scenarioInfo);
            foreach (var notifier in _notifiers)
                notifier.Verify(n => n.NotifyScenarioStart(scenarioInfo));
        }

        [Test]
        public void It_should_delegate_NotifyScenarioFinished_should_print_notification_and_update_stats()
        {
            var scenario = _autoFixture.Create<IScenarioResult>();
            _subject.NotifyScenarioFinished(scenario);
            foreach (var notifier in _notifiers)
                notifier.Verify(n => n.NotifyScenarioFinished(scenario));
        }

    }
}