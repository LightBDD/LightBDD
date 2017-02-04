using System;
using LightBDD.Core.Extensibility;
using LightBDD.XUnit2.Implementation;

namespace LightBDD.XUnit2
{
    /// <summary>
    /// Class allowing to instantiate <see cref="IFeatureBddRunner"/> that is being configured to work with XUnit framework.
    /// </summary>
    public static class FeatureRunnerProvider
    {
        /// <summary>
        /// Returns <see cref="IFeatureBddRunner"/> for given <paramref name="featureType"/>.
        /// </summary>
        /// <param name="featureType">Feature type.</param>
        /// <returns><see cref="IFeatureBddRunner"/> object.</returns>
        public static IFeatureBddRunner GetRunnerFor(Type featureType)
        {
            return XUnit2FeatureCoordinator.GetInstance().RunnerRepository.GetRunnerFor(featureType);
        }
    }
}