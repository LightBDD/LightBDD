using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.AcceptanceTests;
using LightBDD.AcceptanceTests.Helpers;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Reporting;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.Framework.Resources;
using LightBDD.Runner;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium.Chrome;

[assembly: ConfiguredLightBddScope]
namespace LightBDD.AcceptanceTests
{
    public class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration.ReportConfiguration()
                .AddFileReport<PlainTextReportFormatter>("~" + Path.DirectorySeparatorChar + "Reports" + Path.DirectorySeparatorChar + "FeaturesReport.txt");

            configuration.ConfigureDependencies(ConfigureServices);
            configuration.ExecutionExtensionsConfiguration().EnableStepDecorator<ScreenshotCaptureOnFailure>();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            //TODO: lift this limit when Runner gets support for execution modes (running tests limits)
            services.AddSingleton(_ => new ResourcePool<ChromeDriver>(CreateDriver, 4));
        }

        private async Task<ChromeDriver> CreateDriver(CancellationToken token)
        {
            await ChromeDriverInstaller.Instance.InstallOnce(token);
            var service = ChromeDriverService.CreateDefaultService();
            try
            {
                var driver = new ChromeDriver(service);
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(0);
                return driver;
            }
            catch
            {
                service.Dispose();
                throw;
            }
        }
    }
}