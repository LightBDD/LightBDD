using System;
using LightBDD.Integration.MsTest;

namespace LightBDD
{
    public static class FeatureFactory
    {
        public static IFeatureBddRunner GetRunnerFor(Type featureType)
        {
            return MsTestFeatureCoordinator.GetInstance().RunnerFactory.GetRunnerFor(featureType);
        }
    }
}