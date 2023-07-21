using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.AcceptanceTests;
using LightBDD.AcceptanceTests.Helpers;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Framework.Reporting;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.Framework.Resources;
using LightBDD.Runner;
using OpenQA.Selenium.Chrome;

[assembly: ConfiguredLightBddScope]
namespace LightBDD.AcceptanceTests
{
    public class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration.ReportConfiguration()
                .Add(new FileReportGenerator(new PlainTextReportFormatter(), "~" + Path.DirectorySeparatorChar + "Reports" + Path.DirectorySeparatorChar + "FeaturesReport.txt"));

            configuration.DependencyContainerConfiguration().UseDefault(ConfigureContainer);
            configuration.ExecutionExtensionsConfiguration().EnableStepDecorator<ScreenshotCaptureOnFailure>();
        }

        private void ConfigureContainer(IDefaultContainerConfigurator config)
        {
            //TODO: lift this limit when Runner gets support for execution modes (running tests limits)
            config.RegisterType(InstanceScope.Single, _ => new ResourcePool<ChromeDriver>(CreateDriver, 4));
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