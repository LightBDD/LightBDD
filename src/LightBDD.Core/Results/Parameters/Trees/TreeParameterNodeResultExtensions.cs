using System.Collections.Generic;

namespace LightBDD.Core.Results.Parameters.Trees;

/// <summary>
/// Set of extension methods for <seealso cref="ITreeParameterNodeResult"/> type
/// </summary>
public static class TreeParameterNodeResultExtensions
{
    /// <summary>
    /// Enumerates all nodes, including provided one and all it's descendants.
    /// </summary>
    public static IEnumerable<ITreeParameterNodeResult> EnumerateAll(this ITreeParameterNodeResult result)
    {
        yield return result;
        foreach (var child in result.Children)
        foreach (var n in child.EnumerateAll())
            yield return n;
    }
}