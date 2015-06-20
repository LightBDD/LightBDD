using System;

namespace LightBDD
{
    /// <summary>
    /// Exception used to mark test ingored.
    /// It works only with tests marked with [Scenario] attribute. 
    /// If thrown from tests annotated with [Fact] or [Theory] attribute, the test would fail.
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