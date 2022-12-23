using System;
using System.Linq;
using LightBDD.Core.Configuration;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;
using LightBDD.Framework.Notification.Implementation;
using LightBDD.Framework.Parameters.ObjectTrees;

namespace LightBDD.Framework.Configuration;

/// <summary>
/// Configuration class allowing to customize object tree building behavior.
/// </summary>
public class ObjectTreeConfiguration : FeatureConfiguration
{
    private ObjectTreeBuilderOptions _options = new();
    private readonly ObjectTreeBuilder _builder;

    public ObjectTreeConfiguration()
    {
        _builder = new ObjectTreeBuilder(_options);
    }

    public ObjectTreeConfiguration ConfigureOptions(Action<ObjectTreeBuilderOptions> configure)
    {
        ThrowIfSealed();
        configure?.Invoke(_options);
        return this;
    }

    public ObjectTreeConfiguration ResetOptions()
    {
        ThrowIfSealed();
        _options = new();
        return this;
    }

    public ObjectTreeBuilder Builder => _builder;
}