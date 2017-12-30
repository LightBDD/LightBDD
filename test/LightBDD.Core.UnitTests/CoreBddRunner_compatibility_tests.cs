using System;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

#pragma warning disable 618
#pragma warning disable 1998

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_compatibility_tests
    {
        #region Setup/Teardown

        private IFeatureRunner _feature;
        private IBddRunner _runner;

        [SetUp]
        public void SetUp()
        {
            _feature = TestableFeatureRunnerRepository.GetRunner(GetType());
            _runner = _feature.GetBddRunner(this);
        }

        #endregion

        [Test]
        public void RunAsynchronous_should_propagate_step_exception()
        {
            Assert.ThrowsAsync<NotImplementedException>(() => _runner
                .Integrate()
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(new[] { TestStep.Create(Async_test_throwing_exception) })
                .RunAsynchronously());
        }

        private async Task Async_test_throwing_exception()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void RunSynchronous_should_propagate_step_exception()
        {
            Assert.Throws<NotImplementedException>(() => _runner
                .Integrate()
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(new[] { TestStep.CreateSync(Test_throwing_exception) })
                .RunSynchronously());
        }

        private void Test_throwing_exception()
        {
            throw new NotImplementedException();
        }
    }
}
