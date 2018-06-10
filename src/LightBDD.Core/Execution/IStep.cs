using LightBDD.Core.Dependencies;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Interface describing current step, providing step information details and ability to comment it.
    /// </summary>
    public interface IStep : IExecutable
    {
        /// <summary>
        /// Step information details.
        /// </summary>
        IStepInfo Info { get; }
        /// <summary>
        /// Annotates step with comment.
        /// It is possible to comment step many times.
        /// </summary>
        /// <param name="comment">Comment.</param>
        void Comment(string comment);
        /// <summary>
        /// Returns the dependency resolver used by this step.
        /// </summary>
        IDependencyResolver DependencyResolver { get; }
        /// <summary>
        /// Returns the context used by this step (or null if none were provided).
        /// </summary>
        object Context { get; }
    }
}