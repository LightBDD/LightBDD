using System;
using LightBDD.Integration.NUnit3;

namespace LightBDD
{
    public static class FeatureFactory
    {
        public static IFeatureBddRunner GetRunnerFor(Type featureType)
        {
            return NUnit3FeatureCoordinator.GetInstance().RunnerFactory.GetRunnerFor(featureType);
        }
    }
}