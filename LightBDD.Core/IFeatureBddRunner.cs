using System;
using LightBDD.Core.Execution.Results;

namespace LightBDD
{
    public interface IFeatureBddRunner : IDisposable
    {
        IBddRunner GetRunner(object fixture);
        IFeatureResult GetFeatureResult();
    }
}