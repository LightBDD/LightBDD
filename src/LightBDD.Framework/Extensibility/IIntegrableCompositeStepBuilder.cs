using System;
using System.Threading.Tasks;
using LightBDD.Core.Execution.Dependencies;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Extensibility
{
    /// <summary>
    /// Interface allowing to build composite step from list of sub-steps described by <see cref="StepDescriptor"/> instances and step context.
    /// The interface is dedicated for projects extending LightBDD with user friendly API for defining composite steps - it should not be used directly by regular LightBDD users. 
    /// </summary>
    public interface IIntegrableCompositeStepBuilder : IIntegrableStepGroupBuilder
    {
        /// <summary>
        /// Uses <paramref name="contextProvider"/> to instantiate context that will be shared with all sub-steps.
        /// Please note that context can be specified only once and only when there is no steps added yet.
        /// </summary>
        /// <param name="contextProvider">Context provider function.</param>
        /// <returns>Self.</returns>
        /// <exception cref="InvalidOperationException">Thrown if context is already specified or if some steps has been already added.</exception>
        [Obsolete("Use other " + nameof(WithStepContext) + "() method instead", true)]
        IIntegrableCompositeStepBuilder WithStepContext(Func<object> contextProvider);

        /// <summary>
        /// Uses <paramref name="contextProvider"/> to instantiate context that will be shared with all sub-steps.
        /// Please note that context can be specified only once and only when there is no steps added yet.
        /// </summary>
        /// <param name="contextProvider">Context provider function.</param>
        /// <param name="takeOwnership">Specifies if scenario runner should take ownership of the context instance. If set to true and context instance implements <see cref="IDisposable"/>, it will be disposed after scenario finish.</param>
        /// <returns>Self.</returns>
        /// <exception cref="InvalidOperationException">Thrown if context is already specified or if some steps has been already added.</exception>
        [Obsolete("Use other " + nameof(WithStepContext) + "() method instead")]
        IIntegrableCompositeStepBuilder WithStepContext(Func<object> contextProvider, bool takeOwnership);

        /// <summary>
        /// Uses <paramref name="contextProvider"/> to instantiate context that will be shared with all sub-steps.
        /// Please note that context can be specified only once and only when there is no steps added yet.
        /// </summary>
        /// <param name="contextProvider">Context provider function.</param>
        /// <returns>Self.</returns>
        /// <exception cref="InvalidOperationException">Thrown if context is already specified or if some steps has been already added.</exception>
        IIntegrableCompositeStepBuilder WithStepContext(Func<IDependencyResolver, Task<object>> contextProvider); //TODO: test

        /// <summary>
        /// Builds <see cref="CompositeStep"/> based on specified steps and step context provider.
        /// </summary>
        /// <returns><see cref="CompositeStep"/> instance.</returns>
        CompositeStep Build();
    }
}