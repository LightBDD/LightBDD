using System;
using System.Collections.Concurrent;
using System.Threading;
using LightBDD.Core.Configuration;
using LightBDD.ExecutionContext.Configuration;

namespace LightBDD.ExecutionContext
{
    public sealed class ScenarioExecutionContext
    {
        private static readonly AsyncLocal<ScenarioExecutionContext> CurrentContext = new AsyncLocal<ScenarioExecutionContext>();
        private readonly ConcurrentDictionary<Type, IContextProperty> _properties = new ConcurrentDictionary<Type, IContextProperty>();

        public TProperty Get<TProperty>() where TProperty : IContextProperty, new()
        {
            return (TProperty)_properties.GetOrAdd(typeof(TProperty), t => new TProperty());
        }

        public static ScenarioExecutionContext Current
        {
            get
            {
                var ctx = CurrentContext.Value;
                if (ctx != null)
                    return ctx;
                throw new InvalidOperationException($"Current task is not executing any scenarios or {nameof(ScenarioExecutionContext)} is not enabled in {nameof(LightBddConfiguration)}. Ensure that cfg.{nameof(ConfigurationExtensions.ExecutionExtensionsConfiguration)}().{nameof(ScenarioExecutionContextConfiguration.EnableScenarioExecutionContext)}() is called during LightBDD initialization and operation accessing {nameof(ScenarioExecutionContext)} is called from task running scenario.");
            }
            set { CurrentContext.Value = value; }
        }
    }
}