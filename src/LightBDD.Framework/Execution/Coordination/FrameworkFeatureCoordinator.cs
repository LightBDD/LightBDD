using System.Diagnostics;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Coordination;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Execution.Coordination
{
    [DebuggerStepThrough]
    public abstract class FrameworkFeatureCoordinator : FeatureCoordinator
    {

        internal new static FeatureCoordinator GetInstance()
        {
            return FeatureCoordinator.GetInstance();
        }

        internal new static FeatureCoordinator TryGetInstance()
        {
            return FeatureCoordinator.TryGetInstance();
        }

        protected FrameworkFeatureCoordinator(FeatureRunnerRepository runnerRepository, IFeatureAggregator featureAggregator, LightBddConfiguration configuration) 
            : base(runnerRepository, featureAggregator, configuration)
        {
        }
    }
}
