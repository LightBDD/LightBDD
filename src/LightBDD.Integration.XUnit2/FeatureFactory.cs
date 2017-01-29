using System;
using LightBDD.Core.Extensibility;
using LightBDD.Integration.XUnit2;

namespace LightBDD
{
    public static class FeatureFactory
    {
        public static IFeatureBddRunner GetRunnerFor(Type featureType)
        {
            return XUnit2FeatureCoordinator.GetInstance().RunnerFactory.GetRunnerFor(featureType);
        }
    }
}