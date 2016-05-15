using System;
using LightBDD.Core.Notification;
using LightBDD.Integration.MsTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD
{
    public class FeatureFixture
    {
        protected IBddRunner Runner { get; }

        protected FeatureFixture(Func<IProgressNotifier> progressNotifierCreator = null)
        {
            Runner = MsTestFeatureCoordinator.GetInstance().RunnerFactory.GetRunnerFor(GetType(), progressNotifierCreator ?? CreateProgressNotifier)
                .AsBddRunner();
        }

        private static IProgressNotifier CreateProgressNotifier()
        {
            return new MsTestProgressNotifier(ParallelProgressNotifier.ProgressManager.Instance);
        }
    }
}