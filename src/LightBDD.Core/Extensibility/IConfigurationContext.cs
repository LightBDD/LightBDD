using LightBDD.Core.Configuration;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Interface describing context offering <see cref="LightBddConfiguration"/> used to configure tests.
    /// </summary>
    public interface IConfigurationContext
    {
        /// <summary>
        /// Returns <see cref="LightBddConfiguration"/> used to configure tests.
        /// It is expected that returned object will be sealed which means that it should be used only for reading configuration, but not altering it.
        /// </summary>
        LightBddConfiguration Configuration { get; }
    }
}