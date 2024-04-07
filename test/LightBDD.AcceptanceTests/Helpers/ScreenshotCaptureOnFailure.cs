using System;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;

namespace LightBDD.AcceptanceTests.Helpers
{
    internal class ScreenshotCaptureOnFailure : IStepDecorator
    {
        public async Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
        {
            try
            {
                await stepInvocation();
            }
            catch (Exception)
            {
                if (step.Context is IChromeDriverContext context)
                    await TakeScreenshot(step, context);
                throw;
            }

        }

        private static async Task TakeScreenshot(IStep step, IChromeDriverContext context)
        {
            var screenShotPath = $"{Guid.NewGuid()}.png";
            context.Driver.GetScreenshot().SaveAsFile(screenShotPath);
            await step.AttachFile(mgr => mgr.CreateFromFile("screenshot", screenShotPath));
        }
    }
}
