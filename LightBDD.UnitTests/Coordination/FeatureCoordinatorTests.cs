using System;
using LightBDD.Coordination;
using LightBDD.Results;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests.Coordination
{
    [TestFixture]
    public class FeatureCoordinatorTests
    {
        private class TestableFeatureCoordinator : FeatureCoordinator
        {
            public TestableFeatureCoordinator(IFeatureAggregator aggregator)
                : base(aggregator)
            {
            }
        };

        private IFeatureAggregator _aggregator;
        private TestableFeatureCoordinator _subject;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _aggregator = MockRepository.GenerateMock<IFeatureAggregator>();
            _subject = new TestableFeatureCoordinator(_aggregator);
        }

        #endregion

        [Test]
        public void Should_default_instance_be_available()
        {
            Assert.That(FeatureCoordinator.Instance, Is.Not.Null);
        }

        [Test]
        public void Should_not_allow_to_finish_twice()
        {
            _subject.Finished();
            var ex = Assert.Throws<ObjectDisposedException>(() => _subject.Finished());
            Assert.That(ex.Message, Is.StringContaining("FeatureCoordinator work is already finished."));
        }

        [Test]
        public void Should_not_allow_to_add_features_after_finished()
        {
            _subject.Finished();
            var ex = Assert.Throws<ObjectDisposedException>(() => _subject.AddFeature(MockRepository.GenerateMock<IFeatureResult>()));
            Assert.That(ex.Message, Is.StringContaining("FeatureCoordinator work is already finished."));
        }

        [Test]
        public void Should_finish_on_finalize()
        {
            _subject = null;
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
            _aggregator.AssertWasCalled(a => a.Finished());
        }

        [Test]
        public void Should_add_feature()
        {
            var feature = MockRepository.GenerateMock<IFeatureResult>();
            _subject.AddFeature(feature);
            _aggregator.AssertWasCalled(a => a.AddFeature(feature));
        }

        [Test]
        public void Should_finish()
        {
            _subject.Finished();
            _aggregator.AssertWasCalled(a => a.Finished());
        }

        [Test]
        public void Should_call_onBeforeFinish_before_finish()
        {
            var called = false;
            Action action = () =>
            {
                _aggregator.AssertWasNotCalled(a => a.Finished());
                called = true;
            };
            _subject.OnBeforeFinish += action;
            try
            {
                _subject.Finished();
                Assert.True(called);
            }
            finally
            {
                _subject.OnBeforeFinish -= action;
            }
        }

        [Test]
        public void Should_call_onAfterFinish_after_finish()
        {
            var called = false;
            Action action = () =>
            {
                _aggregator.AssertWasCalled(a => a.Finished());
                called = true;
            };
            _subject.OnAfterFinish += action;
            try
            {
                _subject.Finished();
                Assert.True(called);
            }
            finally
            {
                _subject.OnAfterFinish -= action;
            }
        }
    }
}