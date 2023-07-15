using System;

namespace LightBDD.Runner.Implementation
{
    internal class IgnoreException : Exception
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
        public IgnoreException(string message, Exception? inner = null)
            : base(message, inner)
        {
        }
    }
}