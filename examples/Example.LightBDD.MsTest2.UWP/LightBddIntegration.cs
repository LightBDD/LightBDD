using System;
using System.Threading.Tasks;
using Windows.Storage;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Reporting.Configuration;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.MsTest2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Example.LightBDD.MsTest2.UWP
{
    [TestClass]
    public class LightBddIntegration
    {
        private static readonly string ReportPath = @"Reports\FeaturesReport.html";
        private static readonly StorageFolder CurrentTemporaryFolder = ApplicationData.Current.TemporaryFolder;

        [AssemblyInitialize]
        public static void Setup(TestContext testContext) { LightBddScope.Initialize(OnConfigure); }

        [AssemblyCleanup]
        public static async Task Cleanup()
        {
            LightBddScope.Cleanup();
            await Windows.System.Launcher.LaunchFileAsync(await CurrentTemporaryFolder.GetFileAsync(ReportPath));
        }

        private static void OnConfigure(LightBddConfiguration configuration)
        {
            configuration.ReportWritersConfiguration()
                .Clear()
                .AddFileWriter<HtmlReportFormatter>($@"{CurrentTemporaryFolder.Path}\{ReportPath}");
        }
    }
}