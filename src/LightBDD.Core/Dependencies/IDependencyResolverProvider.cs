namespace LightBDD.Core.Dependencies;

/// <summary>
/// Interface providing <see cref="IDependencyResolver"/> instance that is valid for current execution scope, i.e. scenario scope (if called from task executing scenario) or root container scope. <br/>
/// The interface is designed to allow obtaining it from dependencies registered as singleton, which operations require access to scoped instances.
/// </summary>
public interface IDependencyResolverProvider
{
    /// <summary>
    /// Returns instance of <see cref="IDependencyResolver"/> that is valid for current execution scope, i.e. scenario scope (if called from task executing scenario) or root container scope. 
    /// </summary>
    /// <returns></returns>
    IDependencyResolver GetCurrent();
}