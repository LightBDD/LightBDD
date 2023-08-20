using System;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Execution.Implementation.GlobalSetUp;
using Microsoft.Extensions.DependencyInjection;

namespace LightBDD.Core.Configuration;

public class GlobalSetupRegistrator
{
    private readonly IServiceCollection _collection;

    internal GlobalSetupRegistrator(IServiceCollection collection) => _collection = collection;

    /// <summary>
    /// Registers <typeparamref name="TDependency"/> type to be used for global set up before any tests are run and tear down after all tests execution.<br/>
    /// The <seealso cref="IGlobalResourceSetUp.SetUpAsync"/> method will be executed once, before any tests are run. If multiple set up functions are registered, they will be executed in the registration order.<br/>
    /// The <seealso cref="IGlobalResourceSetUp.TearDownAsync"/> method will be executed once after all tests are run, but only if <seealso cref="IGlobalResourceSetUp.SetUpAsync"/> has been successfully run. The tear down methods are executed in reverse registration order, i.e. last registered one will be executed first.<br/>
    /// The <typeparamref name="TDependency" /> instance is resolved from DI container. Please note that it is resolved independently for set up and tear down methods, thus needs to be registered as singleton or scoped if the same instance is expected by both methods.
    /// </summary>
    /// <typeparam name="TDependency">Dependency type, that is registered in the DI container.</typeparam>
    /// <returns>Self.</returns>
    public GlobalSetupRegistrator RegisterGlobalSetUp<TDependency>() where TDependency : IGlobalResourceSetUp
        => RegisterResource<TDependency>();

    /// <summary>
    /// Registers global set up and optional global tear down methods.<br/>
    /// The <paramref name="setUp"/> delegate will be executed once, before any tests are run. If multiple set up methods are registered, they will be executed in the registration order.<br/>
    /// If <paramref name="tearDown"/> delegate is specified, it will be executed once after all tests are run, but only if <paramref name="setUp"/> has been successfully run. The tear down methods are executed in reverse registration order.
    /// </summary>
    /// <param name="activityName">Name of the set up activity</param>
    /// <param name="setUp">Set up method</param>
    /// <param name="tearDown">Tear down method</param>
    /// <returns>Self.</returns>
    public GlobalSetupRegistrator RegisterGlobalSetUp(string activityName, Func<Task> setUp, Func<Task> tearDown = null)
        => RegisterActivity(activityName, setUp, tearDown);

    /// <summary>
    /// Registers global set up and optional global tear down methods.<br/>
    /// The <paramref name="setUp"/> delegate will be executed once, before any tests are run. If multiple set up methods are registered, they will be executed in the registration order.<br/>
    /// If <paramref name="tearDown"/> delegate is specified, it will be executed once after all tests are run, but only if <paramref name="setUp"/> has been successfully run. The tear down methods are executed in reverse registration order.
    /// </summary>
    /// <param name="activityName">Name of the set up activity</param>
    /// <param name="setUp">Set up method</param>
    /// <param name="tearDown">Tear down method</param>
    /// <returns>Self.</returns>
    public GlobalSetupRegistrator RegisterGlobalSetUp(string activityName, Action setUp, Action tearDown = null)
    {
        if (setUp == null)
            throw new ArgumentNullException(nameof(setUp));

        return RegisterActivity(activityName,
            () =>
            {
                setUp();
                return Task.CompletedTask;
            },
            () =>
            {
                tearDown?.Invoke();
                return Task.CompletedTask;
            });
    }

    /// <summary>
    /// Registers global tear down method.<br/>
    /// The <paramref name="tearDown"/> delegate will be executed once after all tests are run. The tear down methods are executed in reverse registration order, i.e. last registered one will be executed as first.
    /// </summary>
    /// <param name="activityName">Name of the tear down activity</param>
    /// <param name="tearDown">Tear down method</param>
    /// <returns>Self.</returns>
    public GlobalSetupRegistrator RegisterGlobalTearDown(string activityName, Func<Task> tearDown)
    {
        if (tearDown == null)
            throw new ArgumentNullException(nameof(tearDown));
        return RegisterActivity(activityName, null, tearDown);
    }

    /// <summary>
    /// Registers global tear down method.<br/>
    /// The <paramref name="tearDown"/> delegate will be executed once after all tests are run. The tear down methods are executed in reverse registration order, i.e. last registered one will be executed as first.
    /// </summary>
    /// <param name="activityName">Name of the tear down activity</param>
    /// <param name="tearDown">Tear down method</param>
    /// <returns>Self.</returns>
    public GlobalSetupRegistrator RegisterGlobalTearDown(string activityName, Action tearDown)
    {
        if (tearDown == null)
            throw new ArgumentNullException(nameof(tearDown));

        return RegisterActivity(activityName,
            null,
            () =>
            {
                tearDown.Invoke();
                return Task.CompletedTask;
            });
    }

    private GlobalSetupRegistrator RegisterResource<TDependency>() where TDependency : IGlobalResourceSetUp
    {
        _collection.AddSingleton<IGlobalSetUp, GlobalResourceSetUp<TDependency>>();
        return this;
    }

    private GlobalSetupRegistrator RegisterActivity(string name, Func<Task> setUp, Func<Task> tearDown)
    {
        _collection.AddSingleton<IGlobalSetUp>(new GlobalActivitySetUp(name, setUp, tearDown));
        return this;
    }
}