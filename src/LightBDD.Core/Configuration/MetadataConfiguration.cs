using System.Collections.Generic;
using System.Reflection;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// LightBDD metadata configuration
    /// </summary>
    [InjectableConfiguration]
    public class MetadataConfiguration : FeatureConfiguration
    {
        private readonly Stack<Assembly> _engineAssemblies = new(new[] { typeof(MetadataConfiguration).Assembly });

        /// <summary>
        /// Returns list of LightBDD Engine assemblies, used to report in the execution reports
        /// </summary>
        public IReadOnlyCollection<Assembly> EngineAssemblies => _engineAssemblies;

        /// <summary>
        /// Registers engine assembly
        /// </summary>
        /// <returns>Self.</returns>
        public MetadataConfiguration RegisterEngineAssembly(Assembly assembly)
        {
            ThrowIfSealed();
            if (!_engineAssemblies.Contains(assembly))
                _engineAssemblies.Push(assembly);
            return this;
        }
    }
}
