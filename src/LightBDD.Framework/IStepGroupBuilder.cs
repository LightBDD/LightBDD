namespace LightBDD.Framework
{
    /// <summary>
    /// Generic allowing to compose <see cref="StepGroup"/> instance.
    /// </summary>
    /// <typeparam name="TContext">Type of context that would be shared between steps.</typeparam>
    public interface IStepGroupBuilder<TContext>
    {
        /// <summary>
        /// Builds <see cref="StepGroup"/> based on specified steps and step context.
        /// </summary>
        /// <returns><see cref="StepGroup"/> instance.</returns>
        StepGroup Build();
    }

    /// <summary>
    /// Interface allowing to compose <see cref="StepGroup"/> instance.
    /// </summary>
    public interface IStepGroupBuilder : IStepGroupBuilder<NoContext>
    {
    }
}