using LightBDD.Core.Configuration;
using LightBDD.Framework.Parameters.ObjectTrees;

namespace LightBDD.Framework.Configuration;

/// <summary>
/// Configuration class allowing to customize object tree building behavior.
/// </summary>
public class ObjectTreeConfiguration : FeatureConfiguration
{
    /// <summary>
    /// Updates <see cref="ObjectTreeBuilderOptions"/> used to instantiate <see cref="ObjectTreeBuilder"/>.
    /// </summary>
    /// <param name="options">Builder options</param>
    /// <returns>Self</returns>
    public ObjectTreeConfiguration UpdateOptions(ObjectTreeBuilderOptions options)
    {
        ThrowIfSealed();
        Options = options;
        return this;
    }

    /// <summary>
    /// Returns configured options.
    /// </summary>
    public ObjectTreeBuilderOptions Options { get; private set; } = ObjectTreeBuilderOptions.Default;
}