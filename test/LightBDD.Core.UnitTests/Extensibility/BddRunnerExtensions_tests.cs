using System;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Extensibility
{
    [TestFixture]
    public class BddRunnerExtensions_tests
    {
        [Test]
        public void Integrated_should_throw_if_runner_does_not_implement_ICoreBddRunner()
        {
            var ex = Assert.Throws<NotSupportedException>(() => new IncompleteRunner().Integrate());
            Assert.That(ex.Message, Is.EqualTo($"The type '{nameof(IncompleteRunner)}' has to implement '{nameof(IFeatureFixtureRunner)}' interface to support integration."));
        }

        [Test]
        public void Integrated_should_throw_if_runner_is_null()
        {
            IBddRunner runner = null;
            Assert.Throws<ArgumentNullException>(() => runner.Integrate());
        }

        [Test]
        public void Integrated_should_return_ICoreBddRunner_for_properly_integrated_runner()
        {
            Assert.DoesNotThrow(() => new CompleteRunner().Integrate());
        }

        class IncompleteRunner : IBddRunner { }

        class CompleteRunner : IBddRunner, IEnrichableFeatureFixtureRunner
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public IFeatureResult GetFeatureResult()
            {
                throw new NotImplementedException();
            }

            public IScenarioRunner NewScenario()
            {
                throw new NotImplementedException();
            }

            public TEnrichedRunner Enrich<TEnrichedRunner>(Func<IFeatureFixtureRunner, IIntegrationContext, TEnrichedRunner> runnerFactory)
            {
                throw new NotImplementedException();
            }

            public IBddRunner AsBddRunner()
            {
                throw new NotImplementedException();
            }
        }
    }
}
