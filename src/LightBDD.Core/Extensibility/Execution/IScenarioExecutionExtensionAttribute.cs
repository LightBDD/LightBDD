using LightBDD.Core.Configuration;

namespace LightBDD.Core.Extensibility.Execution
{
    /// <summary>
    /// Attribute interface allowing to enhance scenario execution with additional logic.
    /// The extensions would be executed in order specified by <see cref="Order"/> property, after globally registered extensions with <see cref="ExecutionExtensionsConfiguration"/>.
    /// </summary>
    public interface IScenarioExecutionExtensionAttribute : IScenarioExecutionExtension
    {
        /// <summary>
        /// Order in which extensions should be applied, where instances of lower values would be executed first.
        /// </summary>
        int Order { get; set; }
    }
}