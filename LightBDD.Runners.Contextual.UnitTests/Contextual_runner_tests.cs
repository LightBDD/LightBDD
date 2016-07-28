using System;
using LightBDD.Core.Extensibility;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.Runners.Contextual.UnitTests
{
    [TestFixture]
    public class Contextual_runner_tests
    {
        public interface ITestableBddRunner : IBddRunner, ICoreBddRunner { }
        class MyContext { }

        private ITestableBddRunner _runner;
        private IScenarioRunner _mockScenarioRunner;
        private Func<object> _capturedContextProvider;

        [SetUp]
        public void SetUp()
        {
            _runner = MockRepository.GenerateStrictMock<ITestableBddRunner>();
            _mockScenarioRunner = MockRepository.GenerateStrictMock<IScenarioRunner>();
            _capturedContextProvider = null;
        }

        [Test]
        public void It_should_allow_to_apply_context_instance()
        {
            ExpectNewScenario();
            ExpectContext();

            var context = new object();
            _runner.WithContext(context).Integrate().NewScenario();

            VerifyAllExpectations();
            Assert.That(_capturedContextProvider.Invoke(), Is.SameAs(context));
        }

        [Test]
        public void It_should_allow_to_apply_context_provider()
        {
            ExpectNewScenario();
            ExpectContext();

            _runner.WithContext(() => TimeSpan.FromSeconds(5)).Integrate().NewScenario();

            VerifyAllExpectations();
            Assert.That(_capturedContextProvider.Invoke(), Is.EqualTo(TimeSpan.FromSeconds(5)));
        }

        [Test]
        public void It_should_allow_to_apply_context_with_parameterless_constructor()
        {
            ExpectNewScenario();
            ExpectContext();

            _runner.WithContext<MyContext>().Integrate().NewScenario();

            VerifyAllExpectations();
            Assert.That(_capturedContextProvider.Invoke(), Is.InstanceOf<MyContext>());
        }

        private void VerifyAllExpectations()
        {
            _runner.VerifyAllExpectations();
            _mockScenarioRunner.VerifyAllExpectations();
        }

        private void ExpectContext()
        {
            _mockScenarioRunner
                .Expect(s => s.WithContext(Arg<Func<object>>.Is.Anything))
                .WhenCalled(call => _capturedContextProvider = (Func<object>)call.Arguments[0])
                .Return(_mockScenarioRunner);
        }

        private void ExpectNewScenario()
        {
            _runner
                .Expect(r => r.NewScenario())
                .Return(_mockScenarioRunner);
        }
    }
}
