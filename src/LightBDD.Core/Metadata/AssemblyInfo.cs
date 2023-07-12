using System;
using System.Reflection;

namespace LightBDD.Core.Metadata;

/// <summary>
/// Assembly information class.
/// </summary>
public class AssemblyInfo
{
    /// <summary>
    /// Creates AssemblyInfo for given assembly
    /// </summary>
    public static AssemblyInfo From(Assembly assembly)
    {
        var name = assembly.GetName();
        return new AssemblyInfo(
            name.Name ?? name.FullName,
            name.Version);
    }

    private AssemblyInfo(string name, Version version)
    {
        Name = name;
        Version = version;
    }

    /// <summary>
    /// Assembly name
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Assembly version
    /// </summary>
    public Version Version { get; }

    /// <inheritdoc />
    public override string ToString() => $"{Name} v{Version}";
}