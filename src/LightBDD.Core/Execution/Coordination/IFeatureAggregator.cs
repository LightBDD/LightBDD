using System;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Coordination
{
    /// <summary>
    /// Feature aggregator interface for collecting feature results.
    /// </summary>
    //TODO: remove IFeatureAggregator interface in LightBDD 4.x as it is not configurable
    public interface IFeatureAggregator : IDisposable
    {
        /// <summary>
        /// Aggregates given feature result.
        /// </summary>
        /// <param name="featureResult">Feature result to aggregate.</param>
        void Aggregate(IFeatureResult featureResult);
    }
}