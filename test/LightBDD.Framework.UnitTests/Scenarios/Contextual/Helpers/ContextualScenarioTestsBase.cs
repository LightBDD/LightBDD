using System;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Contextual.Helpers
{
    public class ContextualScenarioTestsBase
    {
        protected Mock<ITestableBddRunner> Runner;
        protected Mock<IScenarioRunner> MockScenarioRunner;
        protected Func<object> CapturedContextProvider;
        protected Func<IDependencyResolver, object> CapturedContextResolver;

        public interface ITestableBddRunner : IBddRunner, IFeatureFixtureRunner { }

        protected class MyContext { }

        protected void VerifyAllExpectations()
        {
            Runner.Verify();
            MockScenarioRunner.Verify();
        }

        protected void ExpectContext()
        {
            MockScenarioRunner
                .Setup(s => s.WithContext(It.IsAny<Func<object>>(), It.IsAny<bool>()))
                .Returns((Func<object> obj, bool takeOwnership) =>
                {
                    CapturedContextProvider = obj;
                    return MockScenarioRunner.Object;
                })
                .Verifiable();
        }

        protected void ExpectResolvedContext()
        {
            MockScenarioRunner
                .Setup(s => s.WithContext(It.IsAny<Func<IDependencyResolver, object>>(), null))
                .Returns((Func<IDependencyResolver, object> contextResolver, Action<ContainerConfigurator> _) =>
                {
                    CapturedContextResolver = contextResolver;
                    return MockScenarioRunner.Object;
                })
                .Verifiable();
        }

        protected void ExpectNewScenario()
        {
            Runner
                .Setup(r => r.NewScenario())
                .Returns(MockScenarioRunner.Object)
                .Verifiable();
        }

        [SetUp]
        public void SetUp()
        {
            Runner = new Mock<ITestableBddRunner>(MockBehavior.Strict);
            MockScenarioRunner = new Mock<IScenarioRunner>(MockBehavior.Strict);
            CapturedContextProvider = null;
        }
    }
}