using System;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Parameters.ObjectTrees;

namespace LightBDD.Framework.Configuration;

/// <summary>
/// Configuration class allowing to customize object tree building behavior.
/// </summary>
public class ObjectTreeConfiguration : FeatureConfiguration
{
    private ObjectTreeBuilderOptions _options = new();

    /// <summary>
    /// Constructor
    /// </summary>
    public ObjectTreeConfiguration()
    {
        Builder = new ObjectTreeBuilder(_options);
    }

    /// <summary>
    /// Configures options for <seealso cref="ObjectTreeBuilder"/>.
    /// </summary>
    /// <param name="configure">Configuration method</param>
    public ObjectTreeConfiguration ConfigureOptions(Action<ObjectTreeBuilderOptions> configure)
    {
        ThrowIfSealed();
        configure?.Invoke(_options);
        return this;
    }

    /// <summary>
    /// Resets options to default value.
    /// </summary>
    public ObjectTreeConfiguration ResetOptions()
    {
        ThrowIfSealed();
        _options = new();
        return this;
    }

    /// <summary>
    /// Returns configured builder.
    /// </summary>
    public ObjectTreeBuilder Builder { get; }
}