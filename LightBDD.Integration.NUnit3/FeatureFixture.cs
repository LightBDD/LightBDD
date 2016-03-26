using LightBDD.Core.Extensibility;
using LightBDD.Integration.NUnit3;
using NUnit.Framework;

namespace LightBDD
{
    public class FeatureFixture
    {
        private readonly ICoreBddRunner _runner;
        protected IBddRunner Runner => _runner.AsBddRunner();

        protected FeatureFixture()
        {
            _runner = NUnit3BddRunnerFactory.Instance.GetRunnerFor(GetType());
        }

        [OneTimeTearDown]
        public void FeatureFixtureTearDown()
        {
            _runner.Dispose();
        }
    }
}