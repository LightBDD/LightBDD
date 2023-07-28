using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace LightBDD.AcceptanceTests.Helpers
{
    /// <summary>
    /// Based on https://github.com/Swimburger/DownloadChromeDriverSample
    /// </summary>
    public class ChromeDriverInstaller : IDisposable
    {
        private readonly SemaphoreSlim _sem = new(1);
        private bool _installed;
        private readonly HttpClient _httpClient = new();

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
                var chromeDriverDownloadUrl = await GetChromeDriverDownloadUrl(chromeVersion, cancellationToken);
                var driverName = GetDriverName();
                var targetPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), driverName);

                await using (var zipFileStream = await _httpClient.GetStreamAsync(chromeDriverDownloadUrl, cancellationToken))
                using (var zipArchive = new ZipArchive(zipFileStream, ZipArchiveMode.Read))
                await using (var chromeDriverWriter = new FileStream(targetPath, FileMode.Create))
                {
                    await using var chromeDriverStream = zipArchive.Entries.First(e => e.Name == driverName).Open();
                    await chromeDriverStream.CopyToAsync(chromeDriverWriter, cancellationToken);
                }

                _installed = true;
            }
            finally
            {
                _sem.Release();
            }
        }

        private static string GetDriverName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "chromedriver.exe";

            throw new PlatformNotSupportedException("Your operating system is not supported.");
        }

        private async Task<string> GetChromeDriverDownloadUrl(string chromeVersion, CancellationToken cancellationToken)
        {
            var versions = await _httpClient.GetFromJsonAsync<ChromeVersions>("https://googlechromelabs.github.io/chrome-for-testing/known-good-versions-with-downloads.json", cancellationToken);
            var matchPattern = $"{chromeVersion}.";
            var version = versions.Versions.Where(v => v.Version.StartsWith(matchPattern)).MaxBy(v => Version.Parse(v.Version));
            if (version != null && version.Downloads.TryGetValue("chromedriver", out var urls))
                return urls.First(u => u.Platform == "win32").Url;

            throw new Exception($"ChromeDriver version not found for Chrome version {chromeVersion}");
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

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        class ChromeVersions
        {
            public ChromeVersion[] Versions { get; set; }
        }
        class ChromeVersion
        {
            public string Version { get; set; }
            public string Revision { get; set; }
            public Dictionary<string, DownloadUrl[]> Downloads { get; set; }
        }

        class DownloadUrl
        {
            public string Platform { get; set; }
            public string Url { get; set; }
        }
    }
}