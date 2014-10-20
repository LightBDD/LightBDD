using LightBDD.Notification;

namespace LightBDD
{
    /// <summary>
    /// Base class for feature tests.
    /// It creates new BDDRunner instance on FixtureSetUp() and collects runner results and passes them to FeatureCoordinator on FixtureTearDown().
    /// </summary>
    public abstract class FeatureFixture
    {
        /// <summary>
        /// BDD Runner that should be used to run feature tests.
        /// </summary>
        protected BDDRunner Runner { get; private set; }

        /// <summary>
        /// Creates new BDDRunner instance.
        /// </summary>
        protected FeatureFixture()
        {
            Runner = BDDRunnerFactory.GetRunnerFor(GetType(), CreateProgressNotifier);
        }

        /// <summary>
        /// Creates progress notifier used later by BDDRunner.
        /// Default implementation returns ConsoleProgressNotifier.
        /// </summary>
        /// <returns>progress notifier.</returns>
        protected virtual IProgressNotifier CreateProgressNotifier() { return new ConsoleProgressNotifier(); }
    }
}
