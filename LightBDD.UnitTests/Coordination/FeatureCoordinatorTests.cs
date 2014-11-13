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
        private IFeatureAggregator _aggregator;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _aggregator = MockRepository.GenerateMock<IFeatureAggregator>();
            FeatureCoordinator.Instance.Aggregator = _aggregator;
        }

        #endregion

        [Test]
        public void Should_add_feature()
        {
            var feature = MockRepository.GenerateMock<IFeatureResult>();
            FeatureCoordinator.Instance.AddFeature(feature);
            _aggregator.AssertWasCalled(a => a.AddFeature(feature));
        }

        [Test]
        public void Should_finish()
        {
            FeatureCoordinator.Instance.Finished();
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
            FeatureCoordinator.Instance.OnBeforeFinish += action;
            try
            {
                FeatureCoordinator.Instance.Finished();
                Assert.True(called);
            }
            finally
            {
                FeatureCoordinator.Instance.OnBeforeFinish -= action;
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
            FeatureCoordinator.Instance.OnAfterFinish += action;
            try
            {
                FeatureCoordinator.Instance.Finished();
                Assert.True(called);
            }
            finally
            {
                FeatureCoordinator.Instance.OnAfterFinish -= action;
            }
        }
    }
}