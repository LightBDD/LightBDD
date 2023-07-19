using System;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Runner.Implementation;

internal class TestOutputHelpers
{
    private static readonly AsyncLocal<ITestOutputHelper> Helpers = new();

    public static ITestOutputHelper Current => Helpers.Value ?? throw new InvalidOperationException("No scenario is executed at this moment");

    public static void Install(MessageBus bus, ITest test)
    {
        var helper = new TestOutputHelper();
        helper.Initialize(bus, test);
        Helpers.Value = helper;
    }

    public static void Clear() => Helpers.Value = null;
}