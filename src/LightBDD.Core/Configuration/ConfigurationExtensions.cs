namespace LightBDD.Core.Configuration;

/// <summary>
/// Extension class allowing to configure core parts of LightBDD.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Provides configuration for step types.
    /// </summary>
    public static StepTypeConfiguration ForStepTypes(this LightBddConfiguration configuration)
        => configuration.Get<StepTypeConfiguration>();

    /// <summary>
    /// Provides configuration for value formatting.
    /// </summary>
    public static ValueFormattingConfiguration ForValueFormatting(this LightBddConfiguration configuration)
        => configuration.Get<ValueFormattingConfiguration>();

    /// <summary>
    /// Provides configuration for LightBDD metadata.
    /// </summary>
    public static MetadataConfiguration ForMetadata(this LightBddConfiguration configuration)
        => configuration.Get<MetadataConfiguration>();

    /// <summary>
    /// Provides configuration for execution pipeline.
    /// </summary>
    public static ExecutionPipelineConfiguration ForExecutionPipeline(this LightBddConfiguration configuration)
        => configuration.Get<ExecutionPipelineConfiguration>();
}