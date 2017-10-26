using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using LightBDD.Core.Configuration;
using LightBDD.Framework.ExecutionContext.Configuration;

namespace LightBDD.Framework.ExecutionContext
{
    /// <summary>
    /// Scenario execution context class allowing to store and retrieve scenario properties that would be available for all steps and tasks executed within the scenario.
    /// 
    /// <para>This feature has to be enabled in <see cref="LightBddConfiguration"/> via <see cref="ScenarioExecutionContextConfigurationExtensions.EnableScenarioExecutionContext"/>() prior to usage.</para>
    /// </summary>
    [DebuggerStepThrough]
    public sealed class ScenarioExecutionContext
#if !NETSTANDARD1_3
        : MarshalByRefObject
#endif
    {
        private static readonly AsyncLocalContext<ScenarioExecutionContext> CurrentContext = new AsyncLocalContext<ScenarioExecutionContext>();
        private readonly ConcurrentDictionary<Type, IContextProperty> _properties = new ConcurrentDictionary<Type, IContextProperty>();

        /// <summary>
        /// Provides property value of <typeparamref name="TProperty"/> type that is stored in scenario context.
        /// If such property does not exists yet, a new instance will be registered in context and returned.
        /// </summary>
        /// <typeparam name="TProperty">Property type to retrieve.</typeparam>
        /// <returns>Property object.</returns>
        public TProperty Get<TProperty>() where TProperty : IContextProperty, new()
        {
            return (TProperty)_properties.GetOrAdd(typeof(TProperty), t => new TProperty());
        }

        /// <summary>
        /// Returns current scenario execution context.
        /// <exception cref="InvalidOperationException">Throws when <see cref="Current"/> property is accessed from outside of scenario method or if <see cref="ScenarioExecutionContext"/> feature is not enabled.</exception>
        /// </summary>
        public static ScenarioExecutionContext Current
        {
            get
            {
                var ctx = CurrentContext.Value;
                if (ctx != null)
                    return ctx;
                throw new InvalidOperationException($"Current task is not executing any scenarios or {nameof(ScenarioExecutionContext)} is not enabled in {nameof(LightBddConfiguration)}. Ensure that configuration.{nameof(ConfigurationExtensions.ExecutionExtensionsConfiguration)}().{nameof(ScenarioExecutionContextConfigurationExtensions.EnableScenarioExecutionContext)}() is called during LightBDD initialization and operation accessing {nameof(ScenarioExecutionContext)} is called from task running scenario.");
            }
            set => CurrentContext.Value = value;
        }
    }
}