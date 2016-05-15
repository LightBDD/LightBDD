using System;
using LightBDD.Core.Execution.Results;

namespace LightBDD.Core.Coordination
{
    public interface IFeatureAggregator : IDisposable
    {
        void Aggregate(IFeatureResult featureResult);
    }
}