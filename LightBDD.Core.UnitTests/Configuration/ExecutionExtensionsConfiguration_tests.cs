using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Configuration;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Metadata;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Configuration
{
    [TestFixture]
    public class ExecutionExtensionsConfiguration_tests
    {
        #region Extensions
        class ScenarioExtension1 : IScenarioExecutionExtension
        {
            public Task ExecuteAsync(IScenarioInfo scenario, Func<Task> scenarioInvocation)
            {
                throw new NotImplementedException();
            }
        }
        class ScenarioExtension2 : IScenarioExecutionExtension
        {
            public Task ExecuteAsync(IScenarioInfo scenario, Func<Task> scenarioInvocation)
            {
                throw new NotImplementedException();
            }
        }
        class StepExtension1 : IStepExecutionExtension
        {
            public Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
            {
                throw new NotImplementedException();
            }
        }
        class StepExtension2 : IStepExecutionExtension
        {
            public Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        [Test]
        public void EnableScenarioExtension_should_register_given_extension_only_once()
        {
            var cfg = new ExecutionExtensionsConfiguration()
                .EnableScenarioExtension<ScenarioExtension1>()
                .EnableScenarioExtension<ScenarioExtension1>()
                .EnableScenarioExtension<ScenarioExtension2>()
                .EnableScenarioExtension<ScenarioExtension2>();

            CollectionAssert.AreEquivalent(
                new[] { typeof(ScenarioExtension1), typeof(ScenarioExtension2) },
                cfg.ScenarioExecutionExtensions.Select(e => e.GetType()).ToArray());
        }

        [Test]
        public void EnableStepExtension_should_register_given_extension_only_once()
        {
            var cfg = new ExecutionExtensionsConfiguration()
                .EnableStepExtension<StepExtension1>()
                .EnableStepExtension<StepExtension1>()
                .EnableStepExtension<StepExtension2>()
                .EnableStepExtension<StepExtension2>();

            CollectionAssert.AreEquivalent(
                new[] { typeof(StepExtension1), typeof(StepExtension2) },
                cfg.StepExecutionExtensions.Select(e => e.GetType()).ToArray());
        }
    }
}
