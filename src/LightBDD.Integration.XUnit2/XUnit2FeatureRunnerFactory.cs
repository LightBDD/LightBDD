using System;
using LightBDD.Core.Extensibility;
using LightBDD.Integration.XUnit2;

namespace LightBDD
{
    /// <summary>
    /// Class allowing to instantiate <see cref="IFeatureBddRunner"/> that is being configured to work with XUnit framework.
    /// </summary>
    public static class XUnit2FeatureRunnerFactory
    {
        /// <summary>
        /// Returns <see cref="IFeatureBddRunner"/> for given <paramref name="featureType"/>.
        /// </summary>
        /// <param name="featureType">Feature type.</param>
        /// <returns><see cref="IFeatureBddRunner"/> object.</returns>
        public static IFeatureBddRunner GetRunnerFor(Type featureType)
        {
            return XUnit2FeatureCoordinator.GetInstance().RunnerFactory.GetRunnerFor(featureType);
        }
    }
}