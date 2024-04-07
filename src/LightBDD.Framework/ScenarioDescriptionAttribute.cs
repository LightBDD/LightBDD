using System;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework
{
    /// <summary>
    /// Scenario description attribute that can be applied on Scenrio test method.
    /// May be used to enrich scenrio method with description like "In order to... As a... I want to..."
    /// or similar, that would be used by progress notifier and would be included in summary.
    ///
    /// If given implementation supports alternative description attributes, and both are applied on class, this one would be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ScenarioDescriptionAttribute : Attribute, IScenarioDescriptionAttribute
    {
        /// <summary>
        /// Scenario description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Constructor allowing to associate description.
        /// </summary>
        public ScenarioDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
