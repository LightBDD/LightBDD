using LightBDD.Core.Notification;
using LightBDD.UnitTests.Helpers;
using Moq;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Notification
{
    [TestFixture]
    public class DelegatingFeatureProgressNotifier_tests
    {
        private DelegatingFeatureProgressNotifier _subject;
        private IFeatureProgressNotifier[] _notifiers;

        [SetUp]
        public void SetUp()
        {
            _notifiers = new[] { Mock.Of<IFeatureProgressNotifier>(), Mock.Of<IFeatureProgressNotifier>() };
            _subject = new DelegatingFeatureProgressNotifier(_notifiers);
        }

        [Test]
        public void It_should_delegate_NotifyFeatureStart()
        {
            var featureInfo = new Results.TestFeatureInfo();
            _subject.NotifyFeatureStart(featureInfo);
            foreach (var notifier in _notifiers)
                Mock.Get(notifier).Verify(n => n.NotifyFeatureStart(featureInfo));
        }

        [Test]
        public void It_should_delegate_NotifyFeatureFinished()
        {
            var feature = new Results.TestFeatureResult();
            _subject.NotifyFeatureFinished(feature);
            foreach (var notifier in _notifiers)
                Mock.Get(notifier).Verify(n => n.NotifyFeatureFinished(feature));
        }
    }
}