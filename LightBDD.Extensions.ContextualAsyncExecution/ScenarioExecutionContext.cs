using System;
using System.Collections.Concurrent;
using System.Threading;
using LightBDD.Configuration;

namespace LightBDD.Extensions.ContextualAsyncExecution
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
                throw new InvalidOperationException($"Current task is not executing any scenarios. Please ensure that {nameof(ScenarioExecutionContext)} is installed in {nameof(LightBddConfiguration)} and execution context is accessed from task running scenario.");
            }
            set { CurrentContext.Value = value; }
        }
    }
}