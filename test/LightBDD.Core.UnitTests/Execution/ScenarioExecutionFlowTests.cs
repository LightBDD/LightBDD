using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using NUnit.Framework;
#pragma warning disable 1998

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    public class ScenarioExecutionFlowTests
    {
        [Test]
        [TestCaseSource(nameof(GetVoidCases))]
        public async Task It_should_wrap_exceptions_from_void_tasks(NamedTaskCase taskCase)
        {
            try
            {
                await ScenarioExecutionFlow.WrapScenarioExceptions(taskCase.Invoke());
                Assert.Fail("Expected exception to be thrown");
            }
            catch (ScenarioExecutionException e)
            {
                Assert.That(e.InnerException, Is.TypeOf<NotImplementedException>());
            }
        }

        [Test]
        [TestCaseSource(nameof(GetNonVoidCases))]
        public async Task It_should_wrap_exceptions_from_non_void_tasks(NamedTaskCase<bool> taskCase)
        {
            try
            {
                await ScenarioExecutionFlow.WrapScenarioExceptions(taskCase.Invoke());
                Assert.Fail("Expected exception to be thrown");
            }
            catch (ScenarioExecutionException e)
            {
                Assert.That(e.InnerException, Is.TypeOf<NotImplementedException>());
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task It_should_return_task_result(bool expected)
        {
            async Task<bool> MyTask()
            {
                await Task.Yield();
                return expected;
            }

            Assert.That(await ScenarioExecutionFlow.WrapScenarioExceptions(MyTask()), Is.EqualTo(expected));
        }

        [Test]
        public async Task It_should_wrap_only_first_inner_exception_from_void_task()
        {
            async Task Throws<TException>() where TException : Exception, new()
            {
                throw new TException();
            }

            async Task MyTask()
            {
                await Task.WhenAll(Throws<InvalidOperationException>(), Throws<NotImplementedException>());
            }

            try
            {
                await ScenarioExecutionFlow.WrapScenarioExceptions(MyTask());
                Assert.Fail("Expected exception to be thrown");
            }
            catch (ScenarioExecutionException e)
            {
                Assert.That(e.InnerException, Is.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public async Task It_should_wrap_only_first_inner_exception_from_non_void_task()
        {
            async Task<bool> Throws<TException>() where TException : Exception, new()
            {
                throw new TException();
            }

            async Task<bool> MyTask()
            {
                await Task.WhenAll(Throws<InvalidOperationException>(), Throws<NotImplementedException>());
                return true;
            }

            try
            {
                await ScenarioExecutionFlow.WrapScenarioExceptions(MyTask());
                Assert.Fail("Expected exception to be thrown");
            }
            catch (ScenarioExecutionException e)
            {
                Assert.That(e.InnerException, Is.TypeOf<InvalidOperationException>());
            }
        }

        static IEnumerable<NamedTaskCase> GetVoidCases()
        {
            return GetTasks().Select(x => new NamedTaskCase(x));
        }

        private static IEnumerable<Func<Task<bool>>> GetTasks()
        {
            yield return Task_throwing_immediately;
            yield return Task_throwing_after_await;
            yield return Task_throwing_after_delay;
            yield return Task_returning_exception;
            yield return Task_calling_non_async_task_throwing_immediately;
            yield return Task_task_throwing_ScenarioExecutionException_directly;
        }

        static IEnumerable<NamedTaskCase<bool>> GetNonVoidCases()
        {
            return GetTasks().Select(x => new NamedTaskCase<bool>(x));
        }

        private static async Task<bool> Task_throwing_after_await()
        {
            await Task.Yield();
            throw new NotImplementedException();
        }

        private static async Task<bool> Task_throwing_after_delay()
        {
            await Task.Delay(500);
            throw new NotImplementedException();
        }

        private static Task<bool> Task_returning_exception()
        {
            var source = new TaskCompletionSource<bool>();
            source.SetException(new NotImplementedException());
            return source.Task;
        }

        private static async Task<bool> Task_throwing_immediately()
        {
            throw new NotImplementedException();
        }

        private static async Task<bool> Task_calling_non_async_task_throwing_immediately()
        {
            return await Non_async_task_throwing_immediately();
        }

        private static Task<bool> Non_async_task_throwing_immediately()
        {
            throw new NotImplementedException();
        }

        private static async Task<bool> Task_task_throwing_ScenarioExecutionException_directly()
        {
            throw new ScenarioExecutionException(new NotImplementedException());
        }

        public class NamedTaskCase
        {
            private readonly Func<Task> _tackFactory;

            public NamedTaskCase(Func<Task> tackFactory)
            {
                _tackFactory = tackFactory;
            }

            public Task Invoke() => _tackFactory.Invoke();

            public override string ToString()
            {
                return _tackFactory.GetMethodInfo().Name;
            }
        }

        public class NamedTaskCase<T>
        {
            private readonly Func<Task<T>> _tackFactory;

            public NamedTaskCase(Func<Task<T>> tackFactory)
            {
                _tackFactory = tackFactory;
            }

            public Task<T> Invoke() => _tackFactory.Invoke();

            public override string ToString()
            {
                return _tackFactory.GetMethodInfo().Name;
            }
        }
    }
}
