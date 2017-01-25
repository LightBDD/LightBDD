using System;
using LightBDD.Core.Results;

namespace LightBDD
{
    public interface IFeatureBddRunner : IDisposable
    {
        IBddRunner GetRunner(object fixture);
        IFeatureResult GetFeatureResult();
    }
}