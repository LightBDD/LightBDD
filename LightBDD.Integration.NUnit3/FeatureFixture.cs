using LightBDD.Core.Extensibility;
using NUnit.Framework;

namespace LightBDD.Integration.NUnit3
{
    public class FeatureFixture
    {
        private readonly ICoreBddRunner _runner;
        protected IBddRunner Runner => _runner.AsBddRunner();

        public FeatureFixture()
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