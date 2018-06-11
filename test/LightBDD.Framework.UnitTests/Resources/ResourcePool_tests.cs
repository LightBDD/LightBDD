using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Framework.Resources;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Resources
{
    [TestFixture]
    public class ResourcePool_tests
    {
        [Test]
        public async Task Pool_should_dynamically_create_new_instances_when_required_and_dispose_upon_completion()
        {
            var holder = new InstanceHolder();
            var taskCount = 100;

            using (var pool = new ResourcePool<IDisposable>(holder.CreateInstance))
                await RunTasks(pool, taskCount);

            Assert.That(holder.Instances.Count, Is.EqualTo(taskCount));

            foreach (var instance in holder.Instances)
                instance.Verify(x => x.Dispose());
        }

        [Test]
        public async Task Pool_should_dynamically_create_new_instances_up_to_the_specified_limit()
        {
            var holder = new InstanceHolder();
            var expectedLimit = 5;

            using (var pool = new ResourcePool<IDisposable>(holder.CreateInstance, expectedLimit))
                await RunTasks(pool, expectedLimit * 3);

            Assert.That(holder.Instances.Count, Is.EqualTo(expectedLimit));

            foreach (var instance in holder.Instances)
                instance.Verify(x => x.Dispose());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task Pool_should_accept_preset_instances_and_allow_controlling_their_disposal(bool shouldDispose)
        {
            var holder = new InstanceHolder();
            var instances = Enumerable.Range(0, 5).Select(x => holder.CreateInstance()).ToArray();
            using (var pool = new ResourcePool<IDisposable>(instances, shouldDispose))
                await RunTasks(pool, instances.Length * 3);

            foreach (var instance in holder.Instances)
                instance.Verify(x => x.Dispose(), Times.Exactly(shouldDispose ? 1 : 0));
        }

        [Test]
        public void Given_resource_can_be_used_only_by_one_provider_at_a_time()
        {
            var holder = new InstanceHolder();
            var usage = new ConcurrentDictionary<object, object>();

            ResourcePool<IDisposable> pool;
            async Task RunTask()
            {
                using (var provider = new ResourceHandle<IDisposable>(pool))
                {
                    var instance = await provider.ObtainAsync();
                    usage.AddOrUpdate(instance, instance, (_, __) => throw new InvalidOperationException("Instance already in use!"));
                    await Task.Delay(250);
                    usage.TryRemove(instance, out _);
                }
            }

            using (pool = new ResourcePool<IDisposable>(holder.CreateInstance, 5))
                Assert.DoesNotThrowAsync(() => RunTasks(15, RunTask));
        }

        [Test]
        public void Pool_should_throw_for_invalid_arguments()
        {
            var argumentNullException = Assert.Throws<ArgumentNullException>(() => new ResourcePool<string>((Func<string>)null));
            Assert.That(argumentNullException.ParamName, Is.EqualTo("resourceFactory"));

            var argumentException = Assert.Throws<ArgumentException>(() => new ResourcePool<string>(() => "aa", 0));
            Assert.That(argumentException.Message, Does.StartWith("Value has to be greater than 0"));

            argumentException = Assert.Throws<ArgumentException>(() => new ResourcePool<string>(() => "aa", -1));
            Assert.That(argumentException.Message, Does.StartWith("Value has to be greater than 0"));

            argumentException = Assert.Throws<ArgumentException>(() => new ResourcePool<string>(null, true));
            Assert.That(argumentException.Message, Does.StartWith("At least one resource has to be provided"));

            argumentException = Assert.Throws<ArgumentException>(() => new ResourcePool<string>(new string[0]));
            Assert.That(argumentException.Message, Does.StartWith("At least one resource has to be provided"));
        }

        [Test]
        public async Task ResourceHandle_should_return_the_same_instance()
        {
            using (var pool = new ResourcePool<IDisposable>(new InstanceHolder().CreateInstance))
            using (var handle = new ResourceHandle<IDisposable>(pool))
                Assert.That(await handle.ObtainAsync(), Is.SameAs(await handle.ObtainAsync()));
        }

        [Test]
        public void ResourceHandle_should_return_instance_to_pool_upon_disposal_even_if_not_fully_obtained()
        {
            using (var pool = new ResourcePool<IDisposable>(new InstanceHolder().CreateInstance, 1))
            {
                using (var handle = pool.CreateHandle())
                { var _ = handle.ObtainAsync(); }

                using (var handle2 = pool.CreateHandle())
                    Assert.DoesNotThrowAsync(() => handle2.ObtainAsync(new CancellationTokenSource(1000).Token));
            }
        }

        [Test]
        public async Task ResourceHandle_should_cancel_obtain_upon_disposal()
        {

            using (var pool = new ResourcePool<IDisposable>(new InstanceHolder().CreateInstance, 1))
            {
                using (var handle = pool.CreateHandle())
                {
                    await handle.ObtainAsync();

                    Task task;
                    using (var handle2 = pool.CreateHandle())
                        task = handle2.ObtainAsync();
                    Assert.ThrowsAsync<OperationCanceledException>(() => task);
                }
            }
        }

        [Test]
        public async Task ResourceHandle_should_cancel_obtain_upon_disposal_allowing_other_handles_to_retrieve_resource()
        {
            using (var pool = new ResourcePool<IDisposable>(new InstanceHolder().CreateInstance, 1))
            {
                using (var handle = pool.CreateHandle())
                {
                    await handle.ObtainAsync();

                    using (var handle2 = pool.CreateHandle())
                    { var _ = handle2.ObtainAsync(); }

                    handle.Dispose();

                    using (var handle3 = pool.CreateHandle())
                        Assert.DoesNotThrowAsync(() => handle3.ObtainAsync(new CancellationTokenSource(1000).Token));
                }
            }
        }

        [Test]
        public async Task ResourceHandle_should_always_return_the_same_resource_even_if_task_did_not_finish()
        {
            using (var pool = new ResourcePool<IDisposable>(new InstanceHolder().CreateInstance, 1))
            {
                using (var handle = pool.CreateHandle())
                {
                    await handle.ObtainAsync();

                    using (var handle2 = pool.CreateHandle())
                    {
                        var t1 = handle2.ObtainAsync();
                        var t2 = handle2.ObtainAsync();
                        handle.Dispose();
                        Assert.That(await t1, Is.SameAs(await t2));
                    }
                }
            }
        }

        private Task RunTasks(int count, Func<Task> task)
        {
            return Task.WhenAll(Enumerable.Range(0, count).Select(x => task()));
        }

        private Task RunTasks(ResourcePool<IDisposable> pool, int count)
        {
            async Task RunTask()
            {
                using (var provider = new ResourceHandle<IDisposable>(pool))
                {
                    await Task.Yield();
                    await provider.ObtainAsync();
                    await Task.Delay(250);
                }
            }

            return RunTasks(count, RunTask);
        }

        class InstanceHolder
        {
            public ConcurrentQueue<Mock<IDisposable>> Instances { get; } = new ConcurrentQueue<Mock<IDisposable>>();

            public IDisposable CreateInstance()
            {
                var instance = new Mock<IDisposable>();
                Instances.Enqueue(instance);
                return instance.Object;
            }
        }
    }
}
