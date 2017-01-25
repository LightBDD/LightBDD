using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using LightBDD.Core.Execution.Implementation;

namespace LightBDD.Core.Extensibility
{
    public abstract class BddRunnerFactory
    {
        private readonly IIntegrationContext _integrationContext;
        private readonly ConcurrentDictionary<Type, IFeatureBddRunner> _runners = new ConcurrentDictionary<Type, IFeatureBddRunner>();

        protected BddRunnerFactory(IIntegrationContext integrationContext)
        {
            _integrationContext = integrationContext;
        }

        public IFeatureBddRunner GetRunnerFor(Type featureType)
        {
            if (featureType == null)
                throw new ArgumentNullException(nameof(featureType));

            return _runners.GetOrAdd(featureType, type => new FeatureBddRunner(type, _integrationContext));
        }

        public IEnumerable<IFeatureBddRunner> AllRunners => _runners.Values;

    }
}