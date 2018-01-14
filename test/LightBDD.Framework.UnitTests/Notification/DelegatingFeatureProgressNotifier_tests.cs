using System;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;
using LightBDD.UnitTests.Helpers;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Notification
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
            var featureInfo = new TestResults.TestFeatureInfo();
            _subject.NotifyFeatureStart(featureInfo);
            foreach (var notifier in _notifiers)
                Mock.Get(notifier).Verify(n => n.NotifyFeatureStart(featureInfo));
        }

        [Test]
        public void It_should_delegate_NotifyFeatureFinished()
        {
            var feature = new TestResults.TestFeatureResult();
            _subject.NotifyFeatureFinished(feature);
            foreach (var notifier in _notifiers)
                Mock.Get(notifier).Verify(n => n.NotifyFeatureFinished(feature));
        }

        [Test]
        public void Constructor_should_not_accept_null_collection_nor_null_items()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new DelegatingFeatureProgressNotifier(null));
            Assert.That(ex.ParamName, Is.EqualTo("notifiers"));

            ex = Assert.Throws<ArgumentNullException>(() => new DelegatingFeatureProgressNotifier(Mock.Of<IFeatureProgressNotifier>(), null));
            Assert.That(ex.ParamName, Is.EqualTo("notifiers[1]"));
        }

        [Test]
        public void Compose_should_flatten_notifiers_and_return_NoProgressNotifier_if_no_specific_notifiers_are_present()
        {
            var result = DelegatingFeatureProgressNotifier.Compose(new IFeatureProgressNotifier[]
            {
                new DelegatingFeatureProgressNotifier(
                    NoProgressNotifier.Default,
                    NoProgressNotifier.Default,
                    new DelegatingFeatureProgressNotifier(),
                    new DelegatingFeatureProgressNotifier(NoProgressNotifier.Default)),
                NoProgressNotifier.Default,
                new DelegatingFeatureProgressNotifier()
            });

            Assert.That(result, Is.TypeOf<NoProgressNotifier>());
        }

        [Test]
        public void Compose_should_flatten_notifiers_and_return_specific_one_if_only_one_is_present()
        {
            var specific = Mock.Of<IFeatureProgressNotifier>();
            var result = DelegatingFeatureProgressNotifier.Compose(new IFeatureProgressNotifier[]
            {
                new DelegatingFeatureProgressNotifier(
                    NoProgressNotifier.Default,
                    NoProgressNotifier.Default,
                    new DelegatingFeatureProgressNotifier(),
                    new DelegatingFeatureProgressNotifier(NoProgressNotifier.Default, specific)),
                NoProgressNotifier.Default,
                new DelegatingFeatureProgressNotifier()
            });

            Assert.That(result, Is.SameAs(specific));
        }

        [Test]
        public void Compose_should_flatten_notifiers()
        {
            var specific1 = Mock.Of<IFeatureProgressNotifier>();
            var specific2 = Mock.Of<IFeatureProgressNotifier>();
            var result = DelegatingFeatureProgressNotifier.Compose(new[]
            {
                new DelegatingFeatureProgressNotifier(
                    NoProgressNotifier.Default,
                    NoProgressNotifier.Default,
                    new DelegatingFeatureProgressNotifier(),
                    new DelegatingFeatureProgressNotifier(NoProgressNotifier.Default, specific1)),
                NoProgressNotifier.Default,
                new DelegatingFeatureProgressNotifier(),
                specific2
            });

            Assert.That(result, Is.TypeOf<DelegatingFeatureProgressNotifier>());
            Assert.That(
                ((DelegatingFeatureProgressNotifier)result).Notifiers,
                Is.EqualTo(new[] { specific1, specific2 }));
        }
    }
}