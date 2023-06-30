namespace LightBDD.Core.Results.Parameters.Trees;

/// <summary>
/// Interface representing tree parameter details.
/// </summary>
public interface ITreeParameterDetails : IParameterDetails
{
    /// <summary>
    /// Returns tree root node.
    /// </summary>
    ITreeParameterNodeResult Root { get; }
}