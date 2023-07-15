using System;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Exception describing intention to ignore currently executed scenario.
    /// </summary>
    public class IgnoreScenarioException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public IgnoreScenarioException() { }

        /// <summary>
        /// Constructor accepting message and optional inner exception.
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="inner">Inner exception</param>
        public IgnoreScenarioException(string message, Exception inner = null)
            : base(message, inner)
        {
        }
    }
}