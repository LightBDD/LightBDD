namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// A LightBDD feature configuration that could be sealed, making it immutable.
    /// </summary>
    public interface ISealableFeatureConfiguration : IFeatureConfiguration
    {
        /// <summary>
        /// Seals the configuration making it immutable.
        /// </summary>
        void Seal();
    }
}