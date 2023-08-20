#nullable enable
using System.Threading;

namespace LightBDD.Core.Dependencies.Implementation;

internal class DependencyResolverProvider : IDependencyResolverProvider
{
    private readonly IDependencyResolver _root;
    private readonly AsyncLocal<IDependencyResolver> _current = new();

    public DependencyResolverProvider(IDependencyResolver container)
    {
        _root = container;
    }

    public IDependencyResolver GetCurrent() => _current.Value ?? _root;

    public void SetCurrent(IDependencyResolver current)
    {
        _current.Value = current;
    }
}