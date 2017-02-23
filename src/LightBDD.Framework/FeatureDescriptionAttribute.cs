using System;
using System.Diagnostics;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework
{
    /// <summary>
    /// Feature description attribute that can be applied on feature test class.
    /// May be used to enrich feature class with description like "In order to... As a... I want to..."
    /// or similar, that would be used by progress notifier and would be included in summary.
    ///
    /// If given implementation supports alternative description attributes, and both are applied on class, this one would be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [DebuggerStepThrough]
    public class FeatureDescriptionAttribute : Attribute, IFeatureDescriptionAttribute
    {
        /// <summary>
        /// Feature description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Constructor allowing to associate description.
        /// </summary>
        public FeatureDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}