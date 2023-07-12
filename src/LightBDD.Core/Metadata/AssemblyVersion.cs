using System;
using System.Reflection;

namespace LightBDD.Core.Metadata;

/// <summary>
/// Assembly version class.
/// </summary>
public class AssemblyVersion
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AssemblyVersion(Assembly assembly)
    {
        var name = assembly.GetName();
        Name = name.Name;
        Version = name.Version;
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