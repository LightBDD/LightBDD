namespace LightBDD
{
    public class FeatureFixture
    {
        protected IBddRunner Runner { get; }

        protected FeatureFixture()
        {
            Runner = FeatureFactory.GetRunnerFor(GetType()).GetRunner(this);
        }
    }
}