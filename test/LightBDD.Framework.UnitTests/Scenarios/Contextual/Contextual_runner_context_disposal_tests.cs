using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.UnitTests.Scenarios.Helpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using LightBDD.Framework.Scenarios;

namespace LightBDD.Framework.UnitTests.Scenarios.Contextual
{
    [TestFixture]
    public class Contextual_runner_context_disposal_tests
    {
        private Mock<ICoreScenarioBuilder> _builder;
        private IBddRunner _runner;

        [SetUp]
        public void SetUp()
        {
            _builder = ScenarioMocks.CreateScenarioBuilder();
            _runner = ScenarioMocks.CreateBddRunner(_builder);
        }

        [Test]
        public void WithContext_accepting_instance_should_not_takeOwnership_by_default()
        {
            _runner.WithContext(new object());

            _builder.Verify(x => x.WithContext(It.IsAny<Func<object>>(), false));
        }

        [Test]
        public void WithContext_accepting_instance_should_honor_takeOwnership_override()
        {
            _runner.WithContext(new object(), true);

            _builder.Verify(x => x.WithContext(It.IsAny<Func<object>>(), true));
        }

        [Test]
        public void WithContext_accepting_instance_factory_should_takeOwnership_by_default()
        {
            _runner.WithContext(() => new object());

            _builder.Verify(x => x.WithContext(It.IsAny<Func<object>>(), true));
        }

        [Test]
        public void WithContext_accepting_instance_factory_should_honor_takeOwnership_override()
        {
            _runner.WithContext(() => new object(), false);

            _builder.Verify(x => x.WithContext(It.IsAny<Func<object>>(), false));
        }

        [Test]
        public void Generic_WithContext_should_use_DI_container()
        {
            _runner.WithContext<List<string>>();

            _builder.Verify(x => x.WithContext(It.IsAny<Func<IDependencyResolver, object>>(), null));
        }
    }
}
