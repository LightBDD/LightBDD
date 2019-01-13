using LightBDD.Core.Execution.Coordination;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Execution.Coordination
{
    /// <summary>
    /// Framework level feature coordinator.
    /// </summary>
    public abstract class FrameworkFeatureCoordinator : FeatureCoordinator
    {
        internal new static FeatureCoordinator TryGetInstance()
        {
            return FeatureCoordinator.TryGetInstance();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">Integration context</param>
        protected FrameworkFeatureCoordinator(IntegrationContext context)
            : base(context)
        {
        }
    }
}
