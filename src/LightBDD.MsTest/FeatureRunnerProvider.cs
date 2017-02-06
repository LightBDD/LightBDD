using System;
using LightBDD.Core.Extensibility;
using LightBDD.MsTest.Implementation;

namespace LightBDD.MsTest
{
    /// <summary>
    /// Class allowing to instantiate <see cref="IFeatureRunner"/> that is being configured to work with MsTest framework.
    /// </summary>
    public static class FeatureRunnerProvider
    {
        /// <summary>
        /// Returns <see cref="IFeatureRunner"/> for given <paramref name="featureType"/>.
        /// </summary>
        /// <param name="featureType">Feature type.</param>
        /// <returns><see cref="IFeatureRunner"/> object.</returns>
        public static IFeatureRunner GetRunnerFor(Type featureType)
        {
            return MsTestFeatureCoordinator.GetInstance().RunnerRepository.GetRunnerFor(featureType);
        }
    }
}