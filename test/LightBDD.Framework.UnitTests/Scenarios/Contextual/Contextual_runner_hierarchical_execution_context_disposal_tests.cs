using System.Collections.Generic;
using LightBDD.Framework.Scenarios;
using LightBDD.Framework.Scenarios.Contextual;
using LightBDD.Framework.UnitTests.Scenarios.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Contextual
{
    [TestFixture]
    public class Contextual_runner_hierarchical_execution_context_disposal_tests
    {
        private ICompositeStepBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new TestableCompositeStepBuilder();
        }

        [Test]
        public void WithContext_accepting_instance_should_not_takeOwnership_by_default()
        {
            var step = _builder
                 .WithContext(new object())
                 .Build();
            Assert.That(step.SubStepsContext.TakeOwnership, Is.False);
        }

        [Test]
        public void WithContext_accepting_instance_should_honor_takeOwnership_override()
        {
            var step = _builder
                .WithContext(new object(), true)
                .Build();
            Assert.That(step.SubStepsContext.TakeOwnership, Is.True);
        }

        [Test]
        public void WithContext_accepting_instance_factory_should_takeOwnership_by_default()
        {
            var step = _builder
                .WithContext(() => new object())
                .Build();
            Assert.That(step.SubStepsContext.TakeOwnership, Is.True);
        }

        [Test]
        public void WithContext_accepting_instance_factory_should_honor_takeOwnership_override()
        {
            var step = _builder
                .WithContext(() => new object(), false)
                .Build();
            Assert.That(step.SubStepsContext.TakeOwnership, Is.False);
        }

        [Test]
        public void Generic_WithContext_should_takeOwnership_by_default()
        {
            var step = _builder
                .WithContext<List<string>>()
                .Build();
            Assert.That(step.SubStepsContext.TakeOwnership, Is.True);
        }
    }
}
