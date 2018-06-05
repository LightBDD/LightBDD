using System;
using System.Collections.Generic;
using LightBDD.Core.Execution.Dependencies;
using LightBDD.Framework.Scenarios;
using LightBDD.Framework.Scenarios.Contextual;
using LightBDD.Framework.UnitTests.Scenarios.Helpers;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Contextual
{
    [TestFixture]
    public class Contextual_runner_hierarchical_execution_context_disposal_tests
    {
        private ICompositeStepBuilder _builder;
        private Mock<IDependencyContainer> _container;

        [SetUp]
        public void SetUp()
        {
            _builder = new TestableCompositeStepBuilder();
            _container = new Mock<IDependencyContainer>();
        }

        [Test]
        public void WithContext_accepting_instance_should_not_takeOwnership_by_default()
        {
            var step = _builder
                 .WithContext(new object())
                 .Build();
            AssertRegistration(step, false);
        }

        [Test]
        public void WithContext_accepting_instance_should_honor_takeOwnership_override()
        {
            var step = _builder
                .WithContext(new object(), true)
                .Build();
            AssertRegistration(step, true);
        }

        [Test]
        public void WithContext_accepting_instance_factory_should_takeOwnership_by_default()
        {
            var step = _builder
                .WithContext(() => new object())
                .Build();
            AssertRegistration(step, true);
        }

        [Test]
        public void WithContext_accepting_instance_factory_should_honor_takeOwnership_override()
        {
            var step = _builder
                .WithContext(() => new object(), false)
                .Build();
            AssertRegistration(step, false);
        }

        [Test]
        public void Generic_WithContext_should_takeOwnership_by_default()
        {
            var step = _builder
                .WithContext<List<string>>()
                .Build(); AssertRegistration(step, true);
        }

        private void AssertRegistration(CompositeStep step, bool shouldTakeOwnership)
        {
            step.SubStepsContext.ContextResolver(_container.Object).GetAwaiter().GetResult();
            _container.Verify(x => x.RegisterInstance(It.IsAny<object>(), shouldTakeOwnership));
        }
    }
}
