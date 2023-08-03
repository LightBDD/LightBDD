using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Implementation.GlobalSetUp;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Execution;
using LightBDD.Core.Notification;
using System.Reflection;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Extensibility.Implementation;
using LightBDD.Core.Formatting;
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
    /// <param name="testAssembly">Test assembly</param>
    /// <param name="configuration">Configuration</param>
    public EngineContext(Assembly testAssembly, LightBddConfiguration configuration)
    {
        Configuration = configuration;
        MetadataProvider = new MetadataProvider(testAssembly, configuration);
        ExceptionProcessor = new ExceptionProcessor(configuration);
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
    internal GlobalSetUpRegistry GlobalSetUp => Configuration.Get<ExecutionExtensionsConfiguration>().GlobalSetUpRegistry;
    internal IProgressNotifier ProgressNotifier => Configuration.Get<ProgressNotifierConfiguration>().Notifier;
    internal IExecutionExtensions ExecutionExtensions => Configuration.ExecutionExtensionsConfiguration();
    internal IDependencyContainer DependencyContainer => Configuration.DependencyContainerConfiguration().DependencyContainer;
    internal IFileAttachmentsManager FileAttachmentsManager => Configuration.ReportConfiguration().GetFileAttachmentsManager();
    internal IFixtureFactory FixtureFactory => Configuration.ExecutionExtensionsConfiguration().FixtureFactory;
}