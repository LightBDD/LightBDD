using System;

namespace LightBDD.Fixie2
{
    /// <summary>
    /// Scenario attribute that has to be used for all Scenario methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ScenarioAttribute : Attribute
    {
    }
}