#nullable enable
using LightBDD.Core.Extensibility.Implementation;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Scenario factory allowing to create scenario builders for given FeatureFixtures
    /// </summary>
    public class RunnableScenarioFactory
    {
        private readonly EngineContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Integration context</param>
        public RunnableScenarioFactory(EngineContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates <see cref="IRunnableScenarioBuilder"/> instance to execute scenario for feature described by <paramref name="featureInfo"/>
        /// </summary>
        /// <param name="featureInfo">Feature info</param>
        /// <returns>Scenario builder</returns>
        public IRunnableScenarioBuilder CreateFor(IFeatureInfo featureInfo)
        {
            return new RunnableScenarioBuilder(_context, featureInfo);
        }
    }
}
