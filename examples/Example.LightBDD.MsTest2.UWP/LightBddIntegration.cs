using System;
using System.Threading.Tasks;
using Windows.Storage;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.MsTest2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

namespace Example.LightBDD.MsTest2.UWP
{
    /// <summary>
    /// This is a way to enable LightBDD - MsTest integration.
    /// It is required to do it in all assemblies with LightBDD scenarios.
    /// The method with [AssemblyInitialize] attribute has to call LightBddScope.Initialize() to initialize LightBDD.
    /// The method with [AssemblyCleanup] attribute has to call LightBddScope.Cleanup() to finalize LightBDD execution for that assembly and generate reports.
    /// </summary>
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

        /// <summary>
        /// This method allows to customize LightBDD behavior.
        /// The code below configures LightBDD to replace all default reports with HTML report placed in the temporary folder.
        /// More information on what can be customized can be found on wiki: https://github.com/LightBDD/LightBDD/wiki/LightBDD-Configuration#configurable-lightbdd-features 
        /// </summary>
        private static void OnConfigure(LightBddConfiguration configuration)
        {
            configuration.ReportWritersConfiguration()
                .Clear()
                .AddFileWriter<HtmlReportFormatter>($@"{CurrentTemporaryFolder.Path}\{ReportPath}");
        }
    }
}