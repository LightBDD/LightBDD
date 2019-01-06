using System;
using System.Diagnostics;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework
{
    /// <summary>
    /// Label attribute that can be applied on feature test class or scenario method.
    /// May be used to link feature/scenario with external tools by storing ticket number.
    /// Multiple labels per item are supported.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class LabelAttribute : Attribute, ILabelAttribute
    {
        /// <summary>
        /// Specified label.
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Constructor allowing to associate label text.
        /// </summary>
        /// <param name="label">Label.</param>
        public LabelAttribute(string label)
        {
            Label = label;
        }
    }
}