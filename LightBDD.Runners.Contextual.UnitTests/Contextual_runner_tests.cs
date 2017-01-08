using System;
using LightBDD.Core.Extensibility;
using NUnit.Framework;
using Moq;

namespace LightBDD.Runners.Contextual.UnitTests
{
    [TestFixture]
    public class Contextual_runner_tests
    {
        public interface ITestableBddRunner : IBddRunner, ICoreBddRunner { }
        class MyContext { }

        private Mock<ITestableBddRunner> _runner;
        private Mock<IScenarioRunner> _mockScenarioRunner;
        private Func<object> _capturedContextProvider;

        [SetUp]
        public void SetUp()
        {
            _runner = new Mock<ITestableBddRunner>(MockBehavior.Strict);
            _mockScenarioRunner = new Mock<IScenarioRunner>(MockBehavior.Strict);
            _capturedContextProvider = null;
        }

        [Test]
        public void It_should_allow_to_apply_context_instance()
        {
            ExpectNewScenario();
            ExpectContext();

            var context = new object();
            _runner.Object.WithContext(context).Integrate().NewScenario();

            VerifyAllExpectations();
            Assert.That(_capturedContextProvider.Invoke(), Is.SameAs(context));
        }

        [Test]
        public void It_should_allow_to_apply_context_provider()
        {
            ExpectNewScenario();
            ExpectContext();

            _runner.Object.WithContext(() => TimeSpan.FromSeconds(5)).Integrate().NewScenario();

            VerifyAllExpectations();
            Assert.That(_capturedContextProvider.Invoke(), Is.EqualTo(TimeSpan.FromSeconds(5)));
        }

        [Test]
        public void It_should_allow_to_apply_context_with_parameterless_constructor()
        {
            ExpectNewScenario();
            ExpectContext();

            _runner.Object.WithContext<MyContext>().Integrate().NewScenario();

            VerifyAllExpectations();
            Assert.That(_capturedContextProvider.Invoke(), Is.InstanceOf<MyContext>());
        }

        private void VerifyAllExpectations()
        {
            _runner.Verify();
            _mockScenarioRunner.Verify();
        }

        private void ExpectContext()
        {
            _mockScenarioRunner
                .Setup(s => s.WithContext(It.IsAny<Func<object>>()))
                .Returns((Func<object> obj) =>
                {
                    _capturedContextProvider = obj;
                    return _mockScenarioRunner.Object;
                })
                .Verifiable();
        }

        private void ExpectNewScenario()
        {
            _runner
                .Setup(r => r.NewScenario())
                .Returns(_mockScenarioRunner.Object)
                .Verifiable();
        }
    }
}
