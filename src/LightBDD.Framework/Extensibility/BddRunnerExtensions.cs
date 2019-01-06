using LightBDD.Core.Extensibility;
using LightBDD.Framework.Implementation;
using System;

namespace LightBDD.Framework.Extensibility
{
    /// <summary>
    /// <see cref="IBddRunner{TContext}"/> extensions.
    /// </summary>
    public static class BddRunnerExtensions
    {
        /// <summary>
        /// Method allowing to retrieve the <see cref="IBddRunner"/> instance from <see cref="IFeatureRunner"/>.
        /// The <see cref="IBddRunner"/> is a main runner interface that should be used for executing LightBDD scenarios, while the interfaces coming from LightBDD.Core namespace should be used only in integration projects.
        /// </summary>
        /// <param name="featureRunner">Instance of <see cref="IFeatureRunner"/>.</param>
        /// <param name="fixture">Feature fixture instance.</param>
        /// <returns></returns>
        public static IBddRunner GetBddRunner(this IFeatureRunner featureRunner, object fixture)
        {
            if (featureRunner == null)
                throw new ArgumentNullException(nameof(featureRunner));
            //TODO: review before 3.0
            return new BddRunner(featureRunner.ForFixture(fixture));
        }
    }
}