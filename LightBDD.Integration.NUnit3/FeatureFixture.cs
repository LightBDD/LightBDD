using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;
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
            _runner = NUnit3BddRunnerFactory.Instance.GetRunnerFor(GetType(),CreateProgressNotifier);
        }

        [OneTimeTearDown]
        public void FeatureFixtureTearDown()
        {
            _runner.Dispose();
        }

        protected virtual IProgressNotifier CreateProgressNotifier()
        {
            return new NUnit3ProgressNotifier(ParallelProgressNotifier.ProgressManager.Instance);
        }
    }
}