using System;
using System.Collections.Generic;
using LightBDD.Core.Extensibility;

namespace LightBDD.Core.UnitTests.Helpers;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestScenarioInlineCase : Attribute, IScenarioCaseSourceAttribute
{
    private readonly object[] _arguments;

    public TestScenarioInlineCase(params object[] arguments)
    {
        _arguments = arguments ?? new object[] { null };
    }

    public IEnumerable<object[]> GetCases()
    {
        yield return _arguments;
    }

    public bool IsResolvableAtDiscovery => true;
}