using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using Moq;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Extensibility
{
    [TestFixture]
    public class FeatureRunnerRepository_tests
    {
        [Test]
        public void Only_one_repository_should_be_created_per_type()
        {
            var taskCount = 20;
            var mockNotifier = new Mock<IProgressNotifier>();
            mockNotifier.Setup(x => x.Notify(It.IsAny<FeatureStarting>())).Callback(() => Thread.Sleep(100));
            var testableIntegrationContextBuilder = TestableIntegrationContextBuilder.Default().WithProgressNotifier(mockNotifier.Object);

            var runners = new ConcurrentQueue<IFeatureRunner>();
            var repository = new TestableFeatureRunnerRepository(testableIntegrationContextBuilder);

            using (var barrier = new Barrier(taskCount))
            {
                void GetRunner()
                {
                    barrier.SignalAndWait();
                    runners.Enqueue(repository.GetRunnerFor(GetType()));
                }

                var threads = Enumerable.Range(0, taskCount)
                    .Select(_ => new Thread(GetRunner))
                    .ToArray();

                foreach (var thread in threads)
                    thread.Start();

                foreach (var thread in threads)
                    thread.Join();
            }

            Assert.That(runners.Distinct().Count(), Is.EqualTo(1));
            mockNotifier.Verify(x => x.Notify(It.IsAny<FeatureStarting>()), Times.Once);
        }
    }
}
