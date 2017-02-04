using LightBDD.Core.Configuration;

namespace LightBDD.Core.Metadata
{
    /// <summary>
    /// Interface describing step type name metadata.
    /// </summary>
    public interface IStepTypeNameInfo
    {
        /// <summary>
        /// Returns <see cref="Name"/> value.
        /// </summary>
        string ToString();
        /// <summary>
        /// Returns normalized step type name.
        /// If consecutive steps are of the same type (like: GIVEN, GIVEN, GIVEN, WHEN, WHEN), the name of all except first one will be normalized with value of <see cref="StepTypeConfiguration.DefaultRepeatedStepReplacement"/> (producing steps like: GIVEN, AND, AND, WHEN, AND).
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Returns original step type name (before normalization).
        /// </summary>
        string OriginalName { get; }
    }
}