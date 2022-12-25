using System.Collections.Generic;

namespace LightBDD.Core.Results.Parameters.Trees;

public interface ITreeParameterNodeResult : IValueResult
{
    public string Path { get; }
    public string Node { get; }
    public IReadOnlyList<ITreeParameterNodeResult> Children { get; }
}