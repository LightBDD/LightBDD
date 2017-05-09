using LightBDD.Framework;

namespace LightBDD.Core.UnitTests.Helpers
{
    public class TestableSteps : StepGroups
    {
        private readonly IBddRunner _runner;

        public TestableSteps(IBddRunner runner)
        {
            _runner = runner;
        }

        protected override IBddRunner Runner => _runner;
    }
}