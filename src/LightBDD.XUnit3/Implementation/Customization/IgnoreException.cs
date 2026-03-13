using System;
using Xunit.v3;

namespace LightBDD.XUnit3.Implementation.Customization
{
    internal class IgnoreException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public IgnoreException() { }

        /// <summary>
        /// Constructor accepting message and optional inner exception.
        /// The message is prefixed with the xUnit v3 dynamic skip token so that
        /// unhandled IgnoreExceptions are reported as skipped tests by xUnit.
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="inner">Inner exception</param>
        public IgnoreException(string message, Exception inner = null)
            : base(DynamicSkipToken.Value + message, inner)
        {
        }
    }
}
