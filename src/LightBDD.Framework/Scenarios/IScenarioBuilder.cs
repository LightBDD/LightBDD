namespace LightBDD.Framework.Scenarios
{
    /// <summary>
    /// Interface allowing to compose scenario in fluent way.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IScenarioBuilder<TContext>
    {
        /// <summary>
        /// This method should not be used by LightBDD users, but code extending LightBDD capabilities.<br/>
        /// Returns core version of the builder, allowing to configure scenario.<br/>
        /// </summary>
        IIntegratedScenarioBuilder<TContext> Integrate();
    }
}