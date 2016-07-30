using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using LightBDD.Configuration;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Notification;

namespace LightBDD.Core.Extensibility
{
    public abstract class BddRunnerFactory
    {
        private readonly ConcurrentDictionary<Type, ICoreBddRunner> _runners = new ConcurrentDictionary<Type, ICoreBddRunner>();
        protected LightBddConfiguration Configuration { get; }

        protected BddRunnerFactory(LightBddConfiguration configuration)
        {
            Configuration = configuration;
        }

        public ICoreBddRunner GetRunnerFor(Type featureType, Func<IProgressNotifier> progressNotifierProvider)
        {
            if (featureType == null)
                throw new ArgumentNullException(nameof(featureType));
            if (progressNotifierProvider == null)
                throw new ArgumentNullException(nameof(progressNotifierProvider));

            return _runners.GetOrAdd(featureType, type => new CoreBddRunner(type, CreateIntegrationContext(progressNotifierProvider.Invoke())));
        }

        public IEnumerable<ICoreBddRunner> AllRunners => _runners.Values;

        protected abstract IIntegrationContext CreateIntegrationContext(IProgressNotifier progressNotifier);
    }
}