using LightBDD.Framework.Extensibility;

namespace LightBDD.Framework.Scenarios
{
    /// <summary>
    /// Generic interface allowing to compose <see cref="CompositeStep"/> instance.
    /// </summary>
    /// <typeparam name="TContext">Type of context that would be shared between steps.</typeparam>
    public interface ICompositeStepBuilder<TContext>
    {
        /// <summary>
        /// Builds <see cref="CompositeStep"/> based on specified steps and step context.
        /// </summary>
        /// <returns><see cref="CompositeStep"/> instance.</returns>
        CompositeStep Build();

        /// <summary>
        /// This method should not be used by LightBDD users, but code extending LightBDD capabilities.<br/>
        /// Returns core version of the builder, allowing to configure composite step.<br/>
        /// </summary>
        IIntegrableCompositeStepBuilder Integrate();
    }

    /// <summary>
    /// Interface allowing to compose <see cref="CompositeStep"/> instance.
    /// </summary>
    public interface ICompositeStepBuilder : ICompositeStepBuilder<NoContext>
    {
    }
}