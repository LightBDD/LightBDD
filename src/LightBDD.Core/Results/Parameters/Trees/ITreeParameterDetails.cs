using System.Collections.Generic;

namespace LightBDD.Core.Results.Parameters.Trees;

/// <summary>
/// Interface representing tree parameter details.
/// </summary>
public interface ITreeParameterDetails : IParameterDetails
{
    /// <summary>
    /// Returns list of tree nodes.
    /// </summary>
    IReadOnlyList<ITreeParameterNodeResult> Nodes { get; }
}