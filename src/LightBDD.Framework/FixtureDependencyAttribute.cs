using System;
using LightBDD.Core.Dependencies;

namespace LightBDD.Framework
{
    /// <summary>
    /// Attribute marking fixture class members to be injected from DI before scenario execution.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FixtureDependencyAttribute : Attribute, IFixtureDependencyAttribute { }
}