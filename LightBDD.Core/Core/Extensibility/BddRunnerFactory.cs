using System;
using System.Collections.Concurrent;
using LightBDD.Core.Execution.Implementation;

namespace LightBDD.Core.Extensibility
{
    public abstract class BddRunnerFactory
    {
        private readonly ConcurrentDictionary<Type, ICoreBddRunner> _runners = new ConcurrentDictionary<Type, ICoreBddRunner>();
        private readonly IIntegrationContext _integrationContext;

        protected BddRunnerFactory(IIntegrationContext integrationContext)
        {
            _integrationContext = integrationContext;
        }

        public ICoreBddRunner GetRunnerFor(Type featureType)
        {
            return _runners.GetOrAdd(featureType, type => new CoreBddRunner(type, _integrationContext));
        }
    }
}