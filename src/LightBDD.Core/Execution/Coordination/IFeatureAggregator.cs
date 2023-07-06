using System;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Coordination
{
    /// <summary>
    /// Feature aggregator interface for collecting feature results.
    /// </summary>
    [Obsolete("This interface is no longer used by LightBDD")]
    public interface IFeatureAggregator : IDisposable
    {
        /// <summary>
        /// Aggregates given feature result.
        /// </summary>
        /// <param name="featureResult">Feature result to aggregate.</param>
        void Aggregate(IFeatureResult featureResult);
    }
}