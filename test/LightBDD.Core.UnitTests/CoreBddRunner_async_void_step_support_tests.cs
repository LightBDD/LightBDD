using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_async_void_step_support_tests
    {
        private IBddRunner _runner;
        private IFeatureRunner _feature;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _feature = TestableFeatureRunnerRepository.GetRunner(GetType());
            _runner = _feature.GetBddRunner(this);
        }

        #endregion

        [Test]
        public void Runner_should_await_for_async_void_step_before_calling_next_one()
        {
            bool finished = false;
            Action step1 = async () =>
            {
                await Task.Delay(200);
                finished = true;
            };

            Action step2 = () => Assert.True(finished);

            Assert.DoesNotThrow(() => _runner.Test().TestScenario(step1, step2));
        }

        [Test]
        public void Runner_should_await_for_async_void_step_and_propagate_any_exceptions_thrown()
        {
            Action asyncVoidStep = DelayAndThrow;

            var ex = Assert.Throws<InvalidOperationException>(() => _runner.Test().TestScenario(asyncVoidStep));

            Assert.That(ex.Message, Is.EqualTo("test"));
        }

        [Test]
        public void Runner_should_await_for_inner_async_void_methods_and_propagate_their_exceptions()
        {
            Action asyncVoidStep = async () =>
            {
                await Task.Delay(100);
                DelayAndThrow();
                DelayAndThrow();
                throw new InvalidOperationException("test2");
            };

            var ex = Assert.Throws<AggregateException>(() => _runner.Test().TestScenario(asyncVoidStep));

            Assert.That(ex.InnerExceptions.Select(x => x.Message), Is.EquivalentTo(new[] { "test2", "test", "test" }));
        }

        private static async void DelayAndThrow()
        {
            await Task.Delay(200);
            throw new InvalidOperationException("test");
        }
    }
}
