using System;
using System.Collections.Concurrent;
using LightBDD.Coordination;
using LightBDD.Notification;

namespace LightBDD
{
    /// <summary>
    /// Allows to create BDDRunner for specific feature test class.
    /// </summary>
    public static class BDDRunnerFactory
    {
        private static readonly ConcurrentDictionary<Type, BDDRunner> _runners = new ConcurrentDictionary<Type, BDDRunner>();

        static BDDRunnerFactory()
        {
            FeatureCoordinator.Instance.OnBeforeFinish += CollectAllFeatureResults;
        }

        /// <summary>
        /// Creates BDDRunner for specified feature test class.
        /// The same runner would be always returned for the same feature test class.
        /// On runner creation a passed createProgresNotifier methods would be used.
        /// </summary>
        /// <param name="featureTestClass"></param>
        /// <param name="createProgressNotifier"></param>
        /// <returns></returns>
        public static BDDRunner GetRunnerFor(Type featureTestClass, Func<IProgressNotifier> createProgressNotifier)
        {
            return _runners.GetOrAdd(featureTestClass, type => CreateBDDRunner(createProgressNotifier, type));
        }

        private static BDDRunner CreateBDDRunner(Func<IProgressNotifier> createProgressNotifier, Type type)
        {
            var runner = new BDDRunner(type, createProgressNotifier());
            return runner;
        }

        private static void CollectAllFeatureResults()
        {
            foreach (var runner in _runners.Values)
                FeatureCoordinator.Instance.AddFeature(runner.Result);
        }
    }
}