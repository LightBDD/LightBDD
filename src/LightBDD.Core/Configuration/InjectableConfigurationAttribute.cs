using System;
using LightBDD.Core.Dependencies;

namespace LightBDD.Core.Configuration;

/// <summary>
/// Applicable to <see cref="FeatureConfiguration"/>. Specifies that configuration should be resolvable from <see cref="IDependencyResolver"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class InjectableConfigurationAttribute : Attribute { }