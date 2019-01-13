using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Coordination;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Execution.Coordination
{
    /// <summary>
    /// Framework level feature coordinator.
    /// </summary>
    public abstract class FrameworkFeatureCoordinator : FeatureCoordinator
    {
        //TODO: review before 3.0
        internal new static FeatureCoordinator TryGetInstance()
        {
            return FeatureCoordinator.TryGetInstance();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected FrameworkFeatureCoordinator(IntegrationContext context)
            : base(context)
        {
        }
    }
}
