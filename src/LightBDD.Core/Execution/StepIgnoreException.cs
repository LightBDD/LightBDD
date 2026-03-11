using System;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Step ignore exception used to mark step ignored while allowing the scenario to continue.
    /// </summary>
    public class StepIgnoreException : Exception
    {
        /// <summary>
        /// Constructor allowing to specify ignore reason.
        /// </summary>
        /// <param name="reason">Ignore reason.</param>
        public StepIgnoreException(string reason) : base(reason) { }
    }
}