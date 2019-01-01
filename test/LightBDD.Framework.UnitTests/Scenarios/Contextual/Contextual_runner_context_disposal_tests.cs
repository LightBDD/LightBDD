using System;
using System.Collections.Generic;
using LightBDD.Core.Dependencies;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios.Contextual;
using LightBDD.Framework.UnitTests.Scenarios.Contextual.Helpers;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Contextual
{
    [TestFixture]
    public class Contextual_runner_context_disposal_tests : ContextualScenarioTestsBase
    {
        [Test]
        public void WithContext_accepting_instance_should_not_takeOwnership_by_default()
        {
            ExpectNewScenario();
            ExpectContext();

            Runner.Object
                .WithContext(new object());

            MockScenarioRunner.Verify(x => x.WithContext(It.IsAny<Func<object>>(), false));
        }

        [Test]
        public void WithContext_accepting_instance_should_honor_takeOwnership_override()
        {
            ExpectNewScenario();
            ExpectContext();

            Runner.Object
                .WithContext(new object(), true);

            MockScenarioRunner.Verify(x => x.WithContext(It.IsAny<Func<object>>(), true));
        }

        [Test]
        public void WithContext_accepting_instance_factory_should_takeOwnership_by_default()
        {
            ExpectNewScenario();
            ExpectContext();

            Runner.Object
                .WithContext(() => new object());

            MockScenarioRunner.Verify(x => x.WithContext(It.IsAny<Func<object>>(), true));
        }

        [Test]
        public void WithContext_accepting_instance_factory_should_honor_takeOwnership_override()
        {
            ExpectNewScenario();
            ExpectContext();

            Runner.Object
                .WithContext(() => new object(), false);

            MockScenarioRunner.Verify(x => x.WithContext(It.IsAny<Func<object>>(), false));
        }

        [Test]
        public void Generic_WithContext_should_use_DI_container()
        {
            ExpectNewScenario();
            ExpectResolvedContext();

            Runner.Object
                .WithContext<List<string>>();

            MockScenarioRunner.Verify(x => x.WithContext(It.IsAny<Func<IDependencyResolver, object>>(), null));
        }
    }
}
