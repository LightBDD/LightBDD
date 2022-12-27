using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Core.Results.Parameters.Trees;

public static class TreeParameterNodeResults
{
    public static IEnumerable<ITreeParameterNodeResult> EnumerateAll(this ITreeParameterNodeResult r)
    {
        yield return r;
        foreach (var child in r.Children)
        foreach (var n in child.EnumerateAll())
            yield return n;
    }
}