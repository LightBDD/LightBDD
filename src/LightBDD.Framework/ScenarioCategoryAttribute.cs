using System;
using System.Diagnostics;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework
{
    /// <summary>
    /// Scenario category attribute that can be applied on scenario test method.
    /// May be used to associate scenarios with specific categories.
    /// It is possible to apply multiple ScenarioCategory attributes on given scenario.
    ///
    /// If given implementation supports alternative category attributes, and both are applied on scenario method, all of them would be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    [DebuggerStepThrough]
    public class ScenarioCategoryAttribute : Attribute, IScenarioCategoryAttribute
    {
        /// <summary>
        /// Scenario category name.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Constructor accepting category name.
        /// </summary>
        public ScenarioCategoryAttribute(string name)
        {
            Category = name;
        }
    }
}