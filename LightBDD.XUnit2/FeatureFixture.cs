using LightBDD.Notification;
using Xunit.Abstractions;

namespace LightBDD
{
    /// <summary>
    /// Base class for feature tests.
    /// It creates new BDDRunner instance on FixtureSetUp() and collects runner results and passes them to FeatureCoordinator on FixtureTearDown().
    /// </summary>
    public abstract class FeatureFixture
    {
        /// <summary>
        /// A test output helper that should be used insted of Console
        /// </summary>
        protected ITestOutputHelper Output { get; private set; }

        /// <summary>
        /// BDD Runner that should be used to run feature tests.
        /// </summary>
        protected BDDRunner Runner { get; private set; }

        /// <summary>
        /// Creates new BDDRunner instance.
        /// </summary>
        protected FeatureFixture(ITestOutputHelper output)
        {
            Output = output;
            Runner = BDDRunnerFactory.GetRunnerFor(GetType(), CreateProgressNotifier);
        }

        /// <summary>
        /// Creates progress notifier used later by BDDRunner.
        /// XUnit implementation returns XUnitOutputProgressNotifier.
        /// </summary>
        /// <returns>progress notifier.</returns>
        protected virtual IProgressNotifier CreateProgressNotifier() { return new DelegatingProgressNotifier(new XUnitOutputProgressNotifier(Output), SimplifiedConsoleProgressNotifier.GetInstance()); }
    }
}
