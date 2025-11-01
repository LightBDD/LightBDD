using System;
using System.Threading;
using Xunit;
using Xunit.v3;

namespace LightBDD.Runner.Implementation;

internal class TestOutputHelpers
{
    private static readonly AsyncLocal<ITestOutputHelper?> Helpers = new();

    public static ITestOutputHelper Current => Helpers.Value ?? throw new InvalidOperationException("No scenario is executed at this moment");
    public static ITestOutputHelper? TryGetCurrent() => Helpers.Value;

    public static void Install(IXunitTest test)
    {
        var helper = new TestOutputHelper();
        Helpers.Value = helper;
    }

    public static void Clear() => Helpers.Value = null;
}