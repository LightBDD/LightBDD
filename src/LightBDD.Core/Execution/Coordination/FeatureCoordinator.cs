using System;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Reporting;

namespace LightBDD.Core.Execution.Coordination
{
    /// <summary>
    /// Class responsible for coordination of LightBDD test run.
    /// The <see cref="Install"/> method allows to install instance that will be used in test execution cycle and begins the cycle.
    /// Calling <see cref="Dispose"/> method terminates the test execution cycle, triggering reports generation and disposal of the DI container.
    /// </summary>
    public abstract class FeatureCoordinator : IDisposable
    {
        private static readonly object Sync = new();
        private readonly FeatureReportGenerator _reportGenerator;
        private readonly TestRunCycle _testRunCycle;

        /// <summary>
        /// Feature coordinator instance.
        /// </summary>
        protected static FeatureCoordinator Instance { get; private set; }
        /// <summary>
        /// Runner factory.
        /// </summary>
        public FeatureRunnerRepository RunnerRepository { get; }
        /// <summary>
        /// Returns <c>true</c> if already disposed, otherwise <c>false</c>.
        /// </summary>
        public bool IsDisposed { get; private set; }
        /// <summary>
        /// Returns <see cref="LightBddConfiguration"/> configuration used for instantiating <see cref="FeatureCoordinator"/>.
        /// </summary>
        public LightBddConfiguration Configuration { get; }

        /// <summary>
        /// Returns <see cref="IValueFormattingService"/> configured in this coordinator.
        /// </summary>
        public IValueFormattingService ValueFormattingService { get; }

        /// <summary>
        /// Returns instance of installed <see cref="FeatureCoordinator"/>.
        /// </summary>
        /// <returns>Instance of <see cref="FeatureCoordinator"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="FeatureCoordinator"/> instance is not installed yet or already disposed.</exception>
        protected internal static FeatureCoordinator GetInstance()
        {
            var coordinator = Instance ?? throw new InvalidOperationException("LightBDD is not initialized yet.");
            if (coordinator.IsDisposed)
                throw new InvalidOperationException("LightBDD scenario test execution is already finished.");
            return coordinator;
        }

        /// <summary>
        /// Returns the installed instance of <see cref="FeatureCoordinator"/> or null if instance is not installed (or already disposed).
        /// </summary>
        /// <returns><see cref="FeatureCoordinator"/> or null.</returns>
        protected internal static FeatureCoordinator TryGetInstance()
        {
            var coordinator = Instance;
            if (coordinator == null || coordinator.IsDisposed)
                return null;
            return coordinator;
        }

        /// <summary>
        /// Installs the specified feature coordinator in thread safe manner.
        /// The installed instance will be used by LightBDD to coordinate tests execution and generate reports upon disposal.
        /// It is only possible to have one installed instance at given time, however upon disposal, the coordinator uninstalls self, allowing to install new one if needed and start another test cycle.
        /// </summary>
        /// <param name="coordinator">Coordinator instance to install.</param>
        /// <exception cref="InvalidOperationException">Exception thrown if another coordinator is already installed.</exception>
        protected static void Install(FeatureCoordinator coordinator)
        {
            lock (Sync)
            {
                if (Instance != null)
                    throw new InvalidOperationException($"FeatureCoordinator of {Instance.GetType()} type is already installed");
                Instance = coordinator;
            }
            coordinator.Initialize();
        }

        private void UninstallSelf()
        {
            if (Instance != this)
                return;
            lock (Sync)
            {
                if (Instance == this)
                    Instance = null;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">Integration context.</param>
        protected FeatureCoordinator(IntegrationContext context)
        {
            Configuration = context.Configuration;
            _reportGenerator = new FeatureReportGenerator(Configuration);
            RunnerRepository = new FeatureRunnerRepository(context);
            ValueFormattingService = context.ValueFormattingService;
            _testRunCycle = new TestRunCycle(context, RunnerRepository);
        }

        /// <summary>
        /// Disposes coordinator, triggering feature result aggregation.
        /// Each runner belonging to <see cref="RunnerRepository"/>, is disposed and its feature result is aggregated.
        /// After aggregation of all results, the feature aggregator is disposed as well.
        /// 
        /// If coordinator is installed as LightBDD main coordinator, it is uninstalled as well, allowing a new one to be installed in future.
        /// If coordinator is already disposed, method does nothing.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
            UninstallSelf();
            var results = _testRunCycle.Finish();
            Task.Run(() => _reportGenerator.GenerateReports(results)).GetAwaiter().GetResult();
            RunnerRepository.Dispose();
        }

        private void Initialize()
        {
            _testRunCycle.Start();
        }
    }
}
