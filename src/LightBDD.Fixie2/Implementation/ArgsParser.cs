using System.Collections.Generic;
using Fixie;

namespace LightBDD.Fixie2.Implementation;

internal static class ArgsParser
{
    public static IEnumerable<string> ParseCategories(TestEnvironment environment)
    {
        var args = environment.CustomArguments;
        var i = 1;
        while (i < args.Count)
        {
            if (args[i - 1] == "--category" && !args[i].StartsWith("--"))
            {
                yield return args[i];
                i += 2;
            }
            else
            {
                i++;
            }
        }
    }
}