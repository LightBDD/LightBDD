using System.Threading.Tasks;
using LightBDD.Core.Configuration;

namespace LightBDD.Core.Execution;

/// <summary>
/// Interface describing global resource requiring initialization and/or tear down.<br/>
/// Types implementing this interface can be registered with <seealso cref="ExecutionExtensionsConfiguration.RegisterGlobalSetUp{TDependency}"/>:
/// <code>
/// protected override void OnConfigure(LightBddConfiguration configuration)
/// {
///     configuration.DependencyContainerConfiguration()
///         .UseDefault(ConfigureDependencies);
/// 
///     configuration.ExecutionExtensionsConfiguration()
///         .RegisterGlobalSetUp&lt;MyGlobalSetUp>();
/// }
/// 
/// private void ConfigureDependencies(IDefaultContainerConfigurator cfg)
/// {
///     cfg.RegisterType&lt;MyGlobalSetUp>(InstanceScope.Single);
/// }
/// 
/// class MyGlobalSetUp : IGlobalResourceSetUp { /* ... */ }
/// </code>
/// </summary>
public interface IGlobalResourceSetUp
{
    /// <summary>
    /// Action setting up the resource state, executed before any tests run.
    /// </summary>
    Task SetUpAsync();
    /// <summary>
    /// Action tearing down the resource state, executed after all tests run, but only if <seealso cref="SetUpAsync"/> action was executed. 
    /// </summary>
    Task TearDownAsync();
}