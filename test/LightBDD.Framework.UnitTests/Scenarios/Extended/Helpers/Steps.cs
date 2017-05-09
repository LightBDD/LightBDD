using System;
using System.Threading.Tasks;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers
{
    public class Steps
    {
        protected void Step_one() { throw new Exception(nameof(Step_one)); }
        protected void Step_two() { throw new Exception(nameof(Step_two)); }
        protected void Step_not_throwing_exception() { }
        protected void Step_with_parameters(int param1, string param2)
        {
            if (param2 == param1.ToString())
                throw new InvalidOperationException(ExceptionMessageForStep_with_parameters(param1));
        }

        protected void Step_with_ref_parameters(ref int value) { }
        protected void Step_with_out_parameters(out int value)
        {
            value = 5;
        }

        protected async Task Step_with_parameters_async(int param1, string param2)
        {
            await Task.Yield();
            if (param2 == param1.ToString())
                throw new InvalidOperationException(ExceptionMessageForStep_with_parameters(param1));
        }

        protected async Task Step_one_async()
        {
            await Task.Yield();
            throw new Exception(nameof(Step_one_async));
        }

        protected async Task Step_two_async()
        {
            await Task.Yield();
            throw new Exception(nameof(Step_two_async));
        }

        protected async void Step_one_async_action()
        {
            await Task.Yield();
            throw new Exception(nameof(Step_one_async_action));
        }

        protected async void Step_two_async_action()
        {
            await Task.Yield();
            throw new Exception(nameof(Step_two_async_action));
        }

        public static string ExceptionMessageForStep_with_parameters(int matchingParam)
        {
            return $"Params match: {matchingParam}";
        }
    }
}