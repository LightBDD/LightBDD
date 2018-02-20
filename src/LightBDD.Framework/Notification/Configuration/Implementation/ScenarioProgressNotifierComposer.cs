using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightBDD.Core.Notification;

namespace LightBDD.Framework.Notification.Configuration.Implementation
{
    [DebuggerStepThrough]
    internal class ScenarioProgressNotifierComposer
    {
        private readonly List<IScenarioProgressNotifierProvider> _providers;

        public ScenarioProgressNotifierComposer() : this(new List<IScenarioProgressNotifierProvider>())
        {
        }
        private ScenarioProgressNotifierComposer(List<IScenarioProgressNotifierProvider> providers)
        {
            _providers = providers;
        }

        public ScenarioProgressNotifierComposer Clone()
        {
            return new ScenarioProgressNotifierComposer(new List<IScenarioProgressNotifierProvider>(_providers));
        }

        public IScenarioProgressNotifier Compose(object fixture)
        {
            return DelegatingScenarioProgressNotifier.Compose(_providers.Select(p => p.Provide(fixture)));
        }

        public void Append(IScenarioProgressNotifierProvider provider)
        {
            _providers.Add(provider);
        }

        public void Set(IScenarioProgressNotifierProvider provider)
        {
            Clear();
            Append(provider);
        }

        public void Clear()
        {
            _providers.Clear();
        }
    }
}