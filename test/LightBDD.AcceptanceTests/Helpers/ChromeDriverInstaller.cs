using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
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
        private readonly HttpClient _httpClient = new()
        {
            BaseAddress = new Uri("https://chromedriver.storage.googleapis.com/")
        };

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
                var chromeDriverVersion = await GetChromeDriverVersion(chromeVersion, cancellationToken);
                var (zipName, driverName) = GetDriverLocation();
                var targetPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    driverName);

                await using (var zipFileStream = await _httpClient.GetStreamAsync($"{chromeDriverVersion}/{zipName}", cancellationToken))
                using (var zipArchive = new ZipArchive(zipFileStream, ZipArchiveMode.Read))
                await using (var chromeDriverWriter = new FileStream(targetPath, FileMode.Create))
                {
                    await using var chromeDriverStream = zipArchive.GetEntry(driverName).Open();
                    await chromeDriverStream.CopyToAsync(chromeDriverWriter, cancellationToken);
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

        private async Task<string> GetChromeDriverVersion(string chromeVersion, CancellationToken cancellationToken)
        {
            var chromeDriverVersionResponse = await _httpClient.GetAsync($"LATEST_RELEASE_{chromeVersion}", cancellationToken);
            if (chromeDriverVersionResponse.IsSuccessStatusCode)
                return await chromeDriverVersionResponse.Content.ReadAsStringAsync();

            if (chromeDriverVersionResponse.StatusCode == HttpStatusCode.NotFound)
                throw new Exception($"ChromeDriver version not found for Chrome version {chromeVersion}");
            throw new Exception($"ChromeDriver version request failed with status code: {chromeDriverVersionResponse.StatusCode}, reason phrase: {chromeDriverVersionResponse.ReasonPhrase}");
        }

        public string GetChromeVersion()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new PlatformNotSupportedException("Your operating system is not supported.");

            var chromePath = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\chrome.exe", null, null);
            if (chromePath == null)
                throw new Exception("Google Chrome not found in registry");

            var chromeVersion = FileVersionInfo.GetVersionInfo(chromePath).FileVersion;
            return chromeVersion.Substring(0, chromeVersion.LastIndexOf('.'));
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}