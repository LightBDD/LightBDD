using System;

namespace LightBDD.Core.Metadata
{
    /// <summary>
    /// Interface describing scenario or step method parameter information.
    /// </summary>
    public interface IParameterInfo
    {
        /// <summary>
        /// Parameter identifier (will change between test runs).
        /// </summary>
        Guid RuntimeId { get; }
        /// <summary>
        /// Scenario or step info.
        /// </summary>
        IMetadataInfo Owner { get; }
        /// <summary>
        /// Parameter name.
        /// </summary>
        string Name { get; }
    }
}
