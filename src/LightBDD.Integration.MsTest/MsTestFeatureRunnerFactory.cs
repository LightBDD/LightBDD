using System;
using LightBDD.Core.Extensibility;
using LightBDD.Integration.MsTest;

namespace LightBDD
{
    /// <summary>
    /// Class allowing to instantiate <see cref="IFeatureBddRunner"/> that is being configured to work with MsTest framework.
    /// </summary>
    public static class MsTestFeatureRunnerFactory
    {
        /// <summary>
        /// Returns <see cref="IFeatureBddRunner"/> for given <paramref name="featureType"/>.
        /// </summary>
        /// <param name="featureType">Feature type.</param>
        /// <returns><see cref="IFeatureBddRunner"/> object.</returns>
        public static IFeatureBddRunner GetRunnerFor(Type featureType)
        {
            return MsTestFeatureCoordinator.GetInstance().RunnerFactory.GetRunnerFor(featureType);
        }
    }
}