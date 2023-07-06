using System;
using System.Linq;
using System.Reflection;

namespace LightBDD.Core.Metadata;

/// <summary>
/// Test Suite information
/// </summary>
public class TestSuite
{
    /// <summary>
    /// Instance representing not provided test suite
    /// </summary>
    public static readonly TestSuite NotProvided = Create(string.Empty, new Version(), string.Empty);

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; private set; }
    /// <summary>
    /// Version
    /// </summary>
    public Version Version { get; private set; }

    private TestSuite() { }

    /// <summary>
    /// Creates test suite based on provided parameters
    /// </summary>
    public static TestSuite Create(string name, Version version, string description)
    {
        return new TestSuite
        {
            Name = name,
            Description = description,
            Version = version
        };
    }

    /// <summary>
    /// Creates test suite based on provided test assembly.
    /// </summary>
    public static TestSuite Create(Assembly assembly)
    {
        var name = assembly.GetName();
        var suiteName = name.Name ?? name.FullName;

        var description = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
            .OfType<AssemblyDescriptionAttribute>()
            .FirstOrDefault()?.Description;

        if (description == null || string.Equals(description, suiteName, StringComparison.OrdinalIgnoreCase))
            description = string.Empty;

        return Create(suiteName, name.Version, description);
    }
}