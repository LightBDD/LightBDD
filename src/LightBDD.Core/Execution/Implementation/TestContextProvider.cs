#nullable enable
using System;
using System.Reflection;
using System.Threading;

namespace LightBDD.Core.Execution.Implementation;

//TODO: review
internal class TestContextProvider
{
    private static readonly AsyncLocal<TestContextProvider?> Provider = new();
    public MethodInfo TestMethod { get; }
    public object[] TestMethodArguments { get; }

    public static TestContextProvider Current => Provider.Value ?? throw new InvalidOperationException("No scenario is executed at this moment");

    public static void Initialize(MethodInfo testMethod, object[] arguments)
    {
        Provider.Value = new TestContextProvider(testMethod, arguments);
    }

    public static void Clear()
    {
        Provider.Value = null;
    }

    private TestContextProvider(MethodInfo testMethod, object[] arguments)
    {
        TestMethod = testMethod;
        TestMethodArguments = arguments;
    }
}