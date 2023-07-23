using System;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Exception describing intention to ignore currently executed stage (step or scenario) and all parent stages, resulting with overall scenario being ignored.
    /// All execution stages affected by this exception will have <see cref="ExecutionStatus.Ignored"/>.
    /// </summary>
    public class IgnoreException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public IgnoreException() { }

        /// <summary>
        /// Constructor accepting message and optional inner exception.
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="inner">Inner exception</param>
        public IgnoreException(string message, Exception inner = null)
            : base(message, inner)
        {
        }
    }
}