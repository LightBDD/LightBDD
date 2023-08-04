#nullable enable
using System;

namespace LightBDD.Core.Extensibility;

/// <summary>
/// Fixture factory interface
/// </summary>
public interface IFixtureFactory
{
    /// <summary>
    /// Creates instance of <paramref name="fixtureType"/> type.
    /// </summary>
    /// <param name="fixtureType"></param>
    /// <returns></returns>
    object Create(Type fixtureType);
}