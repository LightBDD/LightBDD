using LightBDD.Core.Configuration;
using LightBDD.Framework.Parameters.ObjectTrees;

namespace LightBDD.Framework.Configuration;

/// <summary>
/// Configuration class allowing to customize object tree building behavior.
/// </summary>
public class ObjectTreeConfiguration : FeatureConfiguration
{
    /// <summary>
    /// Configures new instance of <seealso cref="ObjectTreeBuilder"/> with provided options.
    /// </summary>
    /// <param name="options">Builder options</param>
    /// <returns>Self</returns>
    public ObjectTreeConfiguration ConfigureBuilder(ObjectTreeBuilderOptions options)
    {
        ThrowIfSealed();
        Builder = new ObjectTreeBuilder(options);
        return this;
    }

    /// <summary>
    /// Returns configured builder.
    /// </summary>
    public ObjectTreeBuilder Builder { get; private set; } = ObjectTreeBuilder.Default;
}