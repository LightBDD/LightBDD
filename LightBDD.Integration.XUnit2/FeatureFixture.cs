using LightBDD.Integration.XUnit2;

namespace LightBDD
{
    public class FeatureFixture
    {
        protected IBddRunner Runner { get; }

        protected FeatureFixture()
        {
            Runner = XUnitBddRunnerFactory.Instance.GetRunnerFor(GetType()).AsBddRunner();
        }
    }
}