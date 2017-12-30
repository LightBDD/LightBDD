using System;
using System.Diagnostics;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Core.Internals;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// Exception handling configuration.
    /// </summary>
    [DebuggerStepThrough]
    public class ExceptionHandlingConfiguration : FeatureConfiguration
    {
        /// <summary>
        /// Returns function used for formatting exceptions into step/scenario result details.
        /// 
        /// The default implementation returns exception type name and message, followed by inner exception chain, followed by shortened call stack information (up to 4 methods).
        /// </summary>
        public Func<Exception, string> ExceptionDetailsFormatter { get; private set; } = new DefaultExceptionFormatter().Format;

        /// <summary>
        /// Updates exception details formatter with new function.
        /// </summary>
        /// <param name="exceptionDetailsFormatter">New function to use.</param>
        /// <returns>Self.</returns>
        public ExceptionHandlingConfiguration UpdateExceptionDetailsFormatter(Func<Exception, string> exceptionDetailsFormatter)
        {
            ThrowIfSealed();
            ExceptionDetailsFormatter = exceptionDetailsFormatter ?? throw new ArgumentNullException(nameof(exceptionDetailsFormatter));
            return this;
        }
    }
}