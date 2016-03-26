using LightBDD.Integration.MsTest;

namespace LightBDD
{
    public class FeatureFixture
    {
        protected IBddRunner Runner { get; }

        protected FeatureFixture()
        {
            Runner = MsTestBddRunnerFactory.Instance.GetRunnerFor(GetType()).AsBddRunner();
        }
    }
}