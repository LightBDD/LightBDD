using System;

namespace LightBDD.Core.Metadata
{
    public interface IParameterInfo
    {
        string RawName { get; }
        bool IsContextDependent { get; }
        Func<object, object> ValueEvaluator { get; }
        Func<object, string> ValueFormatter { get; }
    }
}