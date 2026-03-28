using System;
using LightBDD.Core.Formatting.ExceptionFormatting;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// Exception handling configuration.
    /// </summary>
    public class ExceptionHandlingConfiguration : FeatureConfiguration
    {
        /// <summary>
        /// Returns function used for formatting exceptions into step/scenario result details.
        /// 
        /// The default implementation returns exception type name and message, followed by inner exception chain, followed by shortened call stack information (up to 4 methods).
        /// </summary>
        public Func<Exception, string> ExceptionDetailsFormatter { get; private set; } = new DefaultExceptionFormatter().Format;

        /// <summary>
        /// Returns function used for extracting exception message for non-failed step/scenario results (e.g. Ignored or Bypassed statuses).
        /// 
        /// The default implementation returns <see cref="Exception.Message"/>.
        /// This can be overridden to strip framework-specific prefixes from exception messages.
        /// </summary>
        public Func<Exception, string> ExceptionMessageExtractor { get; private set; } = e => e.Message;

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

        /// <summary>
        /// Updates exception message extractor with new function.
        /// The extractor is used for non-failed step/scenario results to obtain the status detail message from the exception.
        /// </summary>
        /// <param name="exceptionMessageExtractor">New function to use.</param>
        /// <returns>Self.</returns>
        public ExceptionHandlingConfiguration UpdateExceptionMessageExtractor(Func<Exception, string> exceptionMessageExtractor)
        {
            ThrowIfSealed();
            ExceptionMessageExtractor = exceptionMessageExtractor ?? throw new ArgumentNullException(nameof(exceptionMessageExtractor));
            return this;
        }
    }
}