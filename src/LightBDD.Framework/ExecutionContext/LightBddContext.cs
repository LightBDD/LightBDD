using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Coordination;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.ExecutionContext
{
    public class LightBddContext
    {
        public static LightBddConfiguration Configuration => FeatureCoordinatorWrapper.GetInstance()?.Configuration ?? new LightBddConfiguration();


        private class FeatureCoordinatorWrapper : FeatureCoordinator
        {
            public FeatureCoordinatorWrapper(FeatureRunnerRepository runnerRepository, IFeatureAggregator featureAggregator, LightBddConfiguration configuration)
                : base(runnerRepository, featureAggregator, configuration)
            {
            }

            public static FeatureCoordinator GetInstance()
            {
                return Instance;
            }
        }
    }
}
