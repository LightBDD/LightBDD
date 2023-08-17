using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Implementation.GlobalSetUp;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Execution;
using LightBDD.Core.Notification;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Formatting.ExceptionFormatting;
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
        DependencyContainer = configuration.BuildContainer();
        Configuration = configuration;
        MetadataProvider = DependencyContainer.Resolve<CoreMetadataProvider>();
        ExceptionProcessor = DependencyContainer.Resolve<ExceptionProcessor>();
        ExceptionFormatter = DependencyContainer.Resolve<IExceptionFormatter>();
        ValueFormattingService = DependencyContainer.Resolve<IValueFormattingService>();
        FixtureFactory = DependencyContainer.Resolve<IFixtureFactory>();
        FileAttachmentsManager = DependencyContainer.Resolve<IFileAttachmentsManager>();
        ProgressDispatcher = DependencyContainer.Resolve<ProgressNotificationDispatcher>();
        ReportGenerator = DependencyContainer.Resolve<FeatureReportGenerator>();
        GlobalSetUp = DependencyContainer.Resolve<GlobalSetUpRegistry>();
        ExecutionScheduler = DependencyContainer.Resolve<ScenarioExecutionScheduler>();
    }

    /// <summary>
    /// Returns instance of <see cref="LightBddConfiguration"/>.
    /// </summary>
    public LightBddConfiguration Configuration { get; }

    /// <summary>
    /// Returns instance of <see cref="ValueFormattingService"/>.
    /// </summary>
    public IValueFormattingService ValueFormattingService { get; }

    /// <summary>
    /// Returns instance of <see cref="IExceptionFormatter"/>.
    /// </summary>
    public IExceptionFormatter ExceptionFormatter { get; }

    internal readonly IExecutionTimer ExecutionTimer = DefaultExecutionTimer.StartNew();
    internal readonly CoreMetadataProvider MetadataProvider;
    internal readonly ExceptionProcessor ExceptionProcessor;
    internal readonly IDependencyContainer DependencyContainer;
    internal readonly IFixtureFactory FixtureFactory;
    internal readonly IFileAttachmentsManager FileAttachmentsManager;
    internal readonly ProgressNotificationDispatcher ProgressDispatcher;
    internal readonly FeatureReportGenerator ReportGenerator;
    internal readonly GlobalSetUpRegistry GlobalSetUp;
    internal readonly ScenarioExecutionScheduler ExecutionScheduler;
}