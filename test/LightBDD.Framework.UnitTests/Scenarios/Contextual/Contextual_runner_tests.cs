using System;
using LightBDD.Core.Dependencies;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios.Contextual;
using LightBDD.Framework.UnitTests.Scenarios.Contextual.Helpers;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Contextual
{
    [TestFixture]
    public class Contextual_runner_tests : ContextualScenarioTestsBase
    {
        [Test]
        public void It_should_allow_to_apply_context_instance()
        {
            ExpectNewScenario();
            ExpectContext();

            var context = new object();
            Runner.Object.WithContext(context).Integrate().NewScenario();

            VerifyAllExpectations();
            Assert.That(CapturedContextProvider.Invoke(), Is.SameAs(context));
        }

        [Test]
        public void It_should_allow_to_apply_context_provider()
        {
            ExpectNewScenario();
            ExpectContext();

            Runner.Object.WithContext(() => TimeSpan.FromSeconds(5)).Integrate().NewScenario();

            VerifyAllExpectations();
            Assert.That(CapturedContextProvider.Invoke(), Is.EqualTo(TimeSpan.FromSeconds(5)));
        }

        [Test]
        public void It_should_allow_to_apply_context_resolved_with_DI_container()
        {
            ExpectNewScenario();
            ExpectResolvedContext();

            Runner.Object.WithContext<MyContext>().Integrate().NewScenario();

            VerifyAllExpectations();

            var resolver = new Mock<IDependencyResolver>();

            CapturedContextResolver.Invoke(resolver.Object);
            resolver.Verify(x => x.Resolve(typeof(MyContext)));
        }
    }
}
