#nullable enable
using System;
using LightBDD.Core.Dependencies.Implementation;

namespace LightBDD.Core.Dependencies;

/// <summary>
/// Type representing resolvable dependency that is capable to resolve the instance of <typeparamref name="TDependency"/> against <see cref="IDependencyResolver"/>.
/// Use <see cref="Resolvable"/> to construct.
/// </summary>
/// <typeparam name="TDependency"></typeparam>
public readonly struct Resolvable<TDependency> where TDependency : class?
{
    /// <summary>
    /// Resolver returning null instance.
    /// </summary>
    public static readonly Resolvable<TDependency?> Null = new(_ => null);
    /// <summary>
    /// Resolver using <see cref="IDependencyResolver.Resolve"/> to produce instances.
    /// </summary>
    public static readonly Resolvable<TDependency> Default = default;

    private readonly Func<IDependencyResolver, TDependency>? _resolveFn;

    /// <summary>
    /// Constructor allowing to configure dependency with <paramref name="resolveFn"/>.
    /// </summary>
    public Resolvable(Func<IDependencyResolver, TDependency> resolveFn)
    {
        _resolveFn = resolveFn ?? throw new ArgumentNullException(nameof(resolveFn));
    }

    /// <summary>
    /// Resolves dependency instance using configured resolution method and <see cref="IDependencyResolver"/> provided via <paramref name="resolver"/>.
    /// </summary>
    public TDependency Resolve(IDependencyResolver resolver)
    {
        return _resolveFn != null
            ? _resolveFn(resolver)
            : resolver.Resolve<TDependency>();
    }
}

/// <summary>
/// Type allowing creating resolvable instances using <see cref="IDependencyResolver"/> or direct instantiation.
/// </summary>
public static class Resolvable
{
    /// <summary>
    /// Resolver using <see cref="IDependencyResolver.Resolve"/> to produce instances.
    /// </summary>
    public static Resolvable<TDependency> Default<TDependency>() where TDependency : class => Resolvable<TDependency>.Default;
    /// <summary>
    /// Resolver returning null instance.
    /// </summary>
    public static Resolvable<TDependency?> Null<TDependency>() where TDependency : class => Resolvable<TDependency>.Null;

    /// <summary>
    /// Configures resolvable to return <paramref name="instance"/> and <paramref name="takeOwnership"/> which specifies if instance should be disposed by the container (if implements <see cref="IDisposable"/> or <see cref="IAsyncDisposable"/> interfaces).
    /// </summary>
    public static Resolvable<TDependency> Use<TDependency>(TDependency instance, bool takeOwnership = false) where TDependency : class? => Use(() => instance, takeOwnership);

    /// <summary>
    /// Configures resolvable with <paramref name="resolveFn"/> delegate and <paramref name="takeOwnership"/> which specifies if instance should be disposed by the container (if implements <see cref="IDisposable"/> or <see cref="IAsyncDisposable"/> interfaces).
    /// </summary>
    public static Resolvable<TDependency> Use<TDependency>(Func<TDependency> resolveFn, bool takeOwnership = true) where TDependency : class?
    {
        if (!takeOwnership)
            return new(_ => resolveFn());
        return new(r => r.Resolve<TransientDisposable>().WithInstance(resolveFn()));
    }

    /// <summary>
    /// Configures resolvable with <paramref name="resolveFn"/>.
    /// </summary>
    public static Resolvable<TDependency> Use<TDependency>(Func<IDependencyResolver, TDependency> resolveFn) where TDependency : class? => new(resolveFn);
}