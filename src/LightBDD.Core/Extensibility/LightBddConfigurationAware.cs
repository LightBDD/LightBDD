using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.ExecutionContext;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// A class exposing current <see cref="LightBddConfiguration"/> instance for class deriving from it.
    /// </summary>
    //TODO: rework
    public class LightBddConfigurationAware
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected LightBddConfigurationAware() { }

        /// <summary>
        /// Returns current <see cref="LightBddConfiguration"/> configuration.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if LightBDD is not initialized yet or tests are already finished.</exception>
        protected LightBddConfiguration Configuration => LightBddExecutionContext.GetCurrent().Configuration;
    }
}
