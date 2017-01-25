using System;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Coordination
{
    public interface IFeatureAggregator : IDisposable
    {
        void Aggregate(IFeatureResult featureResult);
    }
}