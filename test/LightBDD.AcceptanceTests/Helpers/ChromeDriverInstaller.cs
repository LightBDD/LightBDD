using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LightBDD.AcceptanceTests.Helpers
{
    /// <summary>
    /// Based on https://github.com/Swimburger/DownloadChromeDriverSample
    /// </summary>
    public class ChromeDriverInstaller : IDisposable
    {
        private readonly SemaphoreSlim _sem = new(1);
        private bool _installed;
        private readonly HttpClient _httpClient = new()
        {
            BaseAddress = new Uri("https://storage.googleapis.com/chrome-for-testing-public/")
        };

        private static readonly string KnownGoodVersionsUrl = "https://googlechromelabs.github.io/chrome-for-testing/known-good-versions-with-downloads.json";

        public static ChromeDriverInstaller Instance { get; } = new();

        private ChromeDriverInstaller() { }

        public async Task InstallOnce(CancellationToken cancellationToken)
        {
            if (_installed)
                return;
            await _sem.WaitAsync(cancellationToken);

            try
            {
                if (_installed)
                    return;

                var chromeVersion = GetChromeVersion();
                var chromeDriverUrl = await GetChromeDriverUrl(chromeVersion, cancellationToken);
                var (zipName, driverName) = GetDriverLocation();
                var targetPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    driverName);

                await using (var zipFileStream = await _httpClient.GetStreamAsync(chromeDriverUrl, cancellationToken))
                using (var zipArchive = new ZipArchive(zipFileStream, ZipArchiveMode.Read))
                {
                    var chromeDriveEntry = zipArchive.Entries.FirstOrDefault(e => e.Name.EndsWith(driverName))
                        ?? throw new Exception($"ChromeDriver not found in {zipName}");

                    await using (var chromeDriverWriter = new FileStream(targetPath, FileMode.Create))
                    {
                        await using var chromeDriverStream = chromeDriveEntry.Open();
                        await chromeDriverStream.CopyToAsync(chromeDriverWriter, cancellationToken);
                    }
                }

                _installed = true;
            }
            finally
            {
                _sem.Release();
            }
        }

        private static (string zipName, string driverName) GetDriverLocation()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return ("chromedriver_win32.zip", "chromedriver.exe");

            throw new PlatformNotSupportedException("Your operating system is not supported.");
        }

        private static string GetChromeVersion()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new PlatformNotSupportedException("Your operating system is not supported.");

            var chromePath = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\chrome.exe", null, null)
                ?? throw new Exception("Google Chrome not found in registry");

            var chromeVersion = FileVersionInfo.GetVersionInfo(chromePath).FileVersion;
            return chromeVersion.Substring(0, chromeVersion.LastIndexOf('.'));
        }


        private async Task<string> GetChromeDriverUrl(string chromeVersion, CancellationToken cancellationToken)
        {
            JObject knownGoodVersions;

            // Fetch the list of known good versions for all Chrome assets.
            await using (var jsonStream = await _httpClient.GetStreamAsync(KnownGoodVersionsUrl, cancellationToken))
            using (var sr = new StreamReader(jsonStream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                knownGoodVersions = new JsonSerializer().Deserialize<JObject>(jsonTextReader);
            }

            string url = null;
            // Look for a matching ChromeDriver version
            foreach (var version in knownGoodVersions.GetValue("versions").Children<JObject>())
            {
                if (version.Value<string>("version").StartsWith(chromeVersion))
                {
                    var downloads = version?.Value<JObject>("downloads");
                    var chromeDriver = downloads?.Value<JArray>("chromedriver");

                    if (chromeDriver != null)
                    {
                        foreach (var platform in chromeDriver)
                        {
                            if ("win64" == platform.Value<string>("platform"))
                                url = platform.Value<string>("url");
                        }
                    }
                }
            }

            if (url == null)
                throw new Exception($"ChromeDriver version not found for Chrome version {chromeVersion}");
            
            return url;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}