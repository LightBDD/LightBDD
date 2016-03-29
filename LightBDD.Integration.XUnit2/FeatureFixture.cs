using System;
using LightBDD.Core.Notification;
using LightBDD.Integration.XUnit2;
using Xunit.Abstractions;

namespace LightBDD
{
    public class FeatureFixture
    {
        protected ITestOutputHelper Output { get; }
        protected IBddRunner Runner { get; }

        protected FeatureFixture(ITestOutputHelper output, Func<IProgressNotifier> progressNotifierCreator = null)
        {
            Output = output;
            Runner = XUnit2BddRunnerFactory.Instance.GetRunnerFor(GetType(), progressNotifierCreator ?? CreateProgressNotifier).AsBddRunner();
        }

        private IProgressNotifier CreateProgressNotifier()
        {
            return new XUnit2ProgressNotifier(Output, ParallelProgressNotifier.ProgressManager.Instance);
        }
    }
}