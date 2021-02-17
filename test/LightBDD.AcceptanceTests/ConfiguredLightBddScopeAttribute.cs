using System;
using System.IO;
using LightBDD.AcceptanceTests;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Framework.Reporting;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.Framework.Resources;
using LightBDD.NUnit3;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;

[assembly: Parallelizable(ParallelScope.All)]
[assembly: LevelOfParallelism(4)]
[assembly: ConfiguredLightBddScope]
namespace LightBDD.AcceptanceTests
{
    public class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration.ReportWritersConfiguration()
                .Add(new ReportFileWriter(new PlainTextReportFormatter(), "~" + Path.DirectorySeparatorChar + "Reports" + Path.DirectorySeparatorChar + "FeaturesReport.txt"));

            configuration.DependencyContainerConfiguration().UseDefault(ConfigureContainer);
        }

        private void ConfigureContainer(IDefaultContainerConfigurator config)
        {
            config.RegisterType(InstanceScope.Single, _ => new ResourcePool<ChromeDriver>(CreateDriver));
        }

        private ChromeDriver CreateDriver()
        {
            var driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(0);
            return driver;
        }
    }
}