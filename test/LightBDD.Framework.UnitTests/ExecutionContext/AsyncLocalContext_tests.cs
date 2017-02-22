using System;
using System.Threading.Tasks;
using LightBDD.Framework.ExecutionContext;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.ExecutionContext
{
    [TestFixture]
    public class AsyncLocalContext_tests
    {
        [Test]
        public void It_should_set_and_get_value()
        {
            var ctx = new AsyncLocalContext<object>();
            var expected = new object();
            ctx.Value = expected;
            Assert.That(ctx.Value, Is.SameAs(expected));
        }

        [Test]
        public void It_should_allow_to_override_value()
        {
            var ctx = new AsyncLocalContext<object>();
            var expected = new object();
            var expected2 = new object();
            ctx.Value = expected;
            Assert.That(ctx.Value, Is.SameAs(expected));

            ctx.Value = expected2;
            Assert.That(ctx.Value, Is.SameAs(expected2));

            ctx.Value = null;
            Assert.That(ctx.Value, Is.Null);
        }

        [Test]
        public void It_should_return_default_value_if_not_specified_yet()
        {
            Assert.That(new AsyncLocalContext<object>().Value, Is.Null);
            Assert.That(new AsyncLocalContext<int>().Value, Is.EqualTo(0));
        }

        [Test]
        public async Task Async_tasks_should_share_the_context()
        {
            var ctx = new AsyncLocalContext<Guid>();
            var task1 = Outer(ctx, Guid.NewGuid());
            var task2 = Outer(ctx, Guid.NewGuid());
            var task3 = Outer(ctx, Guid.NewGuid());

            await Task.WhenAll(task1, task2, task3);
        }

        [Test]
        public async Task Async_tasks_should_not_interfere_each_other()
        {
            var ctx = new AsyncLocalContext<object>();

            Func<object, Task> func = async expected =>
            {
                ctx.Value = expected;
                await Task.Delay(500);
                Assert.That(ctx.Value, Is.EqualTo(expected));
            };

            await Task.WhenAll(func(new object()), func(new object()), func(null));
        }

        private async Task Outer(AsyncLocalContext<Guid> ctx, Guid expected)
        {
            ctx.Value = expected;
            await Task.Yield();
            Assert.That(ctx.Value, Is.EqualTo(expected), "Outer pre");
            await Task.Delay(1000);
            await Task.WhenAll(Inner(ctx, expected), Inner(ctx, expected), Inner(ctx, expected));
            Assert.That(ctx.Value, Is.EqualTo(expected), "Outer post");
        }

        private async Task Inner(AsyncLocalContext<Guid> ctx, Guid expected)
        {
            Assert.That(ctx.Value, Is.EqualTo(expected), "Inner pre");
            await Task.Delay(1000);
            Assert.That(ctx.Value, Is.EqualTo(expected), "Inner post");
        }
    }
}
