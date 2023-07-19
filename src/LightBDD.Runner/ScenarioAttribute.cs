using System;
using LightBDD.Core.Extensibility;
using Xunit;

namespace LightBDD.XUnit2
{
    /// <summary>
    /// Scenario attribute that should be used for XUnit framework tests, as a replacement for [Fact] and [Theory] attributes.
    /// The ScenarioAttribute represents scenario test method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ScenarioAttribute : FactAttribute, IScenarioAttribute
    {
    }
}