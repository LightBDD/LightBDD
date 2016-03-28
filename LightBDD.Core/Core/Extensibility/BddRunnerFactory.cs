using System;
using System.Collections.Concurrent;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Notification;

namespace LightBDD.Core.Extensibility
{
    public abstract class BddRunnerFactory
    {
        private readonly ConcurrentDictionary<Type, ICoreBddRunner> _runners = new ConcurrentDictionary<Type, ICoreBddRunner>();

        public ICoreBddRunner GetRunnerFor(Type featureType, Func<IProgressNotifier> progressNotifierProvider)
        {
            return _runners.GetOrAdd(featureType, type => new CoreBddRunner(type, CreateIntegrationContext(progressNotifierProvider.Invoke())));
        }

        protected abstract IIntegrationContext CreateIntegrationContext(IProgressNotifier progressNotifier);
    }
}