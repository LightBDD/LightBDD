using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.XUnit2.Implementation.Customization
{
    internal class TaskExecutor
    {
        private static readonly Type[] FixtureTypes = new[] { typeof(IClassFixture<>), typeof(ICollectionFixture<>) };

        private static Task<RunSummary> RunOnThreadPool(CancellationToken token, Func<Task<RunSummary>> code)
        {
            if (SynchronizationContext.Current == null)
                return Task.Run(code, token);

            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            return Task.Factory.StartNew(code, token, TaskCreationOptions.DenyChildAttach | TaskCreationOptions.HideScheduler, scheduler).Unwrap();
        }

        public static async Task<RunSummary> RunAsync(CancellationToken token, Func<Task<RunSummary>>[] tasks, ITestClass testClass)
        {
            var summary = new RunSummary();
            if (tasks.Length > 1 && IsParallelizable(testClass))
            {
                var results = await Task.WhenAll(tasks.Select(task => RunOnThreadPool(token, task)));
                foreach (var result in results)
                    summary.Aggregate(result);
            }
            else
            {
                foreach (var task in tasks)
                    summary.Aggregate(await task());
            }

            return summary;
        }

        private static bool IsParallelizable(ITestClass testClass)
        {
            if (!AssemblySettings.Current.EnableInterClassParallelization)
                return false;

            if (testClass.TestCollection.CollectionDefinition != null)
                return false;

            return testClass.Class.Interfaces.All(type => !type.IsGenericType || !FixtureTypes.Contains(type.ToRuntimeType().GetGenericTypeDefinition()));
        }
    }
}