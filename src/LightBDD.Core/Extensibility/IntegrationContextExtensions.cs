using System;
using LightBDD.Core.Configuration;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Extensions for <see cref="IIntegrationContext"/>.
    /// </summary>
    public static class IntegrationContextExtensions
    {
        /// <summary>
        /// Retrieves <see cref="LightBddConfiguration"/> from <see cref="IIntegrationContext"/>.
        /// </summary>
        /// <param name="context"><see cref="IIntegrationContext"/> instance.</param>
        /// <returns><see cref="LightBddConfiguration"/> instance.</returns>
        public static LightBddConfiguration GetConfiguration(this IIntegrationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            var configurationContext = context as IConfigurationContext;
            if (configurationContext == null)
                throw new ArgumentException($"Unable to retrieve configuration from context of {context.GetType()} type. Provided context does not implement {typeof(IConfigurationContext)} interface.");
            return configurationContext.Configuration;
        }
    }
}