using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Implementation.GlobalSetUp;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Execution;
using LightBDD.Core.Notification;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Reporting;

namespace LightBDD.Core.Extensibility;

/// <summary>
/// LightBDD Engine Context
/// </summary>
public class EngineContext
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="configuration">Configuration</param>
    public EngineContext(LightBddConfiguration configuration)
    {
        Configuration = configuration;
        MetadataProvider = new CoreMetadataProvider(configuration);
        ExceptionProcessor = new ExceptionProcessor(configuration);
        DependencyContainer = Configuration.DependencyContainerConfiguration().Build();
    }
    /// <summary>
    /// Returns instance of <see cref="LightBddConfiguration"/>.
    /// </summary>
    public LightBddConfiguration Configuration { get; }

    /// <summary>
    /// Returns instance of <see cref="ValueFormattingService"/>.
    /// </summary>
    public ValueFormattingService ValueFormattingService => MetadataProvider.ValueFormattingService;

    internal readonly IExecutionTimer ExecutionTimer = DefaultExecutionTimer.StartNew();
    internal readonly CoreMetadataProvider MetadataProvider;
    internal readonly ExceptionProcessor ExceptionProcessor;
    internal readonly IDependencyContainer DependencyContainer;
    internal GlobalSetUpRegistry GlobalSetUp => Configuration.Get<ExecutionExtensionsConfiguration>().GlobalSetUpRegistry;
    internal IProgressNotifier ProgressNotifier => Configuration.Get<ProgressNotifierConfiguration>().Notifier;
    internal IExecutionExtensions ExecutionExtensions => Configuration.ExecutionExtensionsConfiguration();
    internal IFileAttachmentsManager FileAttachmentsManager => Configuration.ReportConfiguration().GetFileAttachmentsManager();
    internal IFixtureFactory FixtureFactory => Configuration.ExecutionExtensionsConfiguration().FixtureFactory;
}