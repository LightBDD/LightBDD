using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Configuration
{
    [TestFixture]
    public class ExecutionExtensionsConfiguration_tests
    {
        #region Decorators
        class ScenarioDecorator1 : IScenarioDecorator
        {
            public Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
            {
                throw new NotImplementedException();
            }
        }
        class ScenarioDecorator2 : IScenarioDecorator
        {
            public Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
            {
                throw new NotImplementedException();
            }
        }
        class StepDecorator1 : IStepDecorator
        {
            public Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
            {
                throw new NotImplementedException();
            }
        }
        class StepDecorator2 : IStepDecorator
        {
            public Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        [Test]
        public void EnableScenarioDecorator_should_register_given_extension_only_once()
        {
            var cfg = new ExecutionExtensionsConfiguration()
                .EnableScenarioDecorator<ScenarioDecorator1>()
                .EnableScenarioDecorator<ScenarioDecorator1>()
                .EnableScenarioDecorator<ScenarioDecorator2>()
                .EnableScenarioDecorator<ScenarioDecorator2>();

            CollectionAssert.AreEquivalent(
                new[] { typeof(ScenarioDecorator1), typeof(ScenarioDecorator2) },
                cfg.ScenarioDecorators.Select(e => e.GetType()).ToArray());
        }

        [Test]
        public void EnableStepDecorator_should_register_given_extension_only_once()
        {
            var cfg = new ExecutionExtensionsConfiguration()
                .EnableStepDecorator<StepDecorator1>()
                .EnableStepDecorator<StepDecorator1>()
                .EnableStepDecorator<StepDecorator2>()
                .EnableStepDecorator<StepDecorator2>();

            CollectionAssert.AreEquivalent(
                new[] { typeof(StepDecorator1), typeof(StepDecorator2) },
                cfg.StepDecorators.Select(e => e.GetType()).ToArray());
        }

        [Test]
        public void Configuration_should_be_sealable()
        {
            var lighbddConfig = new LightBddConfiguration();
            var cfg = lighbddConfig.Get<ExecutionExtensionsConfiguration>()
                .EnableStepDecorator<StepDecorator1>()
                .EnableScenarioDecorator<ScenarioDecorator1>();
            lighbddConfig.Seal();

            Assert.Throws<InvalidOperationException>(() => cfg.EnableStepDecorator<StepDecorator2>());
            Assert.Throws<InvalidOperationException>(() => cfg.EnableScenarioDecorator<ScenarioDecorator2>());

            CollectionAssert.AreEquivalent(
                new[] { typeof(ScenarioDecorator1) },
                cfg.ScenarioDecorators.Select(e => e.GetType()).ToArray());
            CollectionAssert.AreEquivalent(
                new[] { typeof(StepDecorator1) },
                cfg.StepDecorators.Select(e => e.GetType()).ToArray());
        }
    }
}
