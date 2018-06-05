using System;
using System.Threading.Tasks;
using LightBDD.Core.Execution.Dependencies;
using Moq;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Execution.Dependencies
{
    [TestFixture]
    public class SimpleDependencyContainer_tests
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task RegisterInstance_should_handle_disposal(bool expectDispose)
        {
            var instance = new Mock<IDisposable>();
            using (var container = new SimpleDependencyContainer())
                Assert.AreSame(instance.Object, await container.RegisterInstance(instance.Object, expectDispose));

            instance.Verify(x => x.Dispose(), Times.Exactly(expectDispose ? 1 : 0));
        }

        class Disposable : IDisposable
        {
            public void Dispose()
            {
                Disposed = true;
            }

            public bool Disposed { get; private set; }
        }

        [Test]
        public async Task ResolveAsync_should_always_return_new_instance_and_dispose_upon_completion()
        {
            Disposable d1;
            Disposable d2;
            using (var container = new SimpleDependencyContainer())
            {
                d1 = (Disposable)await container.ResolveAsync(typeof(Disposable));
                d2 = (Disposable)await container.ResolveAsync(typeof(Disposable));
                Assert.AreNotSame(d1, d2);
                Assert.False(d1.Disposed);
                Assert.False(d2.Disposed);
            }
            Assert.True(d1.Disposed);
            Assert.True(d2.Disposed);
        }

        [Test]
        public async Task BeginScope_should_make_separate_scope()
        {
            Disposable outer;
            Disposable inner;
            using (var container = new SimpleDependencyContainer())
            {
                outer = (Disposable)await container.ResolveAsync(typeof(Disposable));
                using (var container2 = container.BeginScope())
                    inner = (Disposable)await container2.ResolveAsync(typeof(Disposable));

                Assert.True(inner.Disposed);
                Assert.False(outer.Disposed);
            }
            Assert.True(outer.Disposed);
            Assert.True(inner.Disposed);
        }
    }
}
