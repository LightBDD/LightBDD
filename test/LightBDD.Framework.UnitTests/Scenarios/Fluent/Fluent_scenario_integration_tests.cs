using System.Collections.Concurrent;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios;
using LightBDD.Framework.UnitTests.Helpers;
using LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Fluent
{
    [TestFixture]
    public class Fluent_scenario_integration_tests : Steps
    {
        private readonly ConcurrentQueue<int> _numbers = new();

        [Test]
        public async Task Full_integration_test()
        {
            await TestableBddRunner.Default.RunScenario(r => r
                .AddAsyncSteps(Async_Task_Step_1)
                .AddSteps(Void_Step_2)
                .AddSteps(Async_Void_Step_3)
                .AddAsyncSteps(_ => Async_Task_Step4())
                .AddSteps(_ => Void_Step5())
                .AddSteps(_ => Async_Void_Step6())
                .AddSteps(Assert_Steps)
                .RunAsync());

            Assert_Steps();
        }

        async Task Async_Task_Step_1()
        {
            await Task.Delay(100);
            _numbers.Enqueue(1);
        }

        void Void_Step_2()
        {
            _numbers.Enqueue(2);
        }

        async void Async_Void_Step_3()
        {
            await Task.Delay(100);
            _numbers.Enqueue(3);
        }

        async Task Async_Task_Step4()
        {
            await Task.Delay(100);
            _numbers.Enqueue(4);
        }

        void Void_Step5()
        {
            _numbers.Enqueue(5);
        }

        async void Async_Void_Step6()
        {
            await Task.Delay(100);
            _numbers.Enqueue(6);
        }

        void Assert_Steps()
        {
            Assert.AreEqual(new[] { 1, 2, 3, 4, 5, 6 }, _numbers.ToArray());
        }
    }
}