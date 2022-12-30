using System.Collections.Generic;

namespace LightBDD.Core.Results.Parameters.Trees;

/// <summary>
/// Interface describing validation result of tree parameter node
/// </summary>
public interface ITreeParameterNodeResult : IValueResult
{
    /// <summary>
    /// Node path in object tree
    /// </summary>
    public string Path { get; }
    /// <summary>
    /// Node name
    /// </summary>
    public string Node { get; }
    /// <summary>
    /// Collection of node children
    /// </summary>
    public IReadOnlyList<ITreeParameterNodeResult> Children { get; }
}