using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace LightBDD.Reporting.Progressive.UI.Services
{
    public class InteropService
    {
        private static InteropService _instance;
        private readonly IJSRuntime _runtime;
        private readonly JsonlImporter _jsonlImporter;
        private static InteropService Instance => _instance ?? throw new InvalidOperationException("Interop not initialized");

        public InteropService(IJSRuntime runtime, JsonlImporter jsonlImporter)
        {
            _runtime = runtime;
            _jsonlImporter = jsonlImporter;
        }

        public async Task Initialize()
        {
            _instance = this;
            await _runtime.InvokeVoidAsync("comms.onInitialized");
        }

        [JSInvokable]
        public static async Task InteropService_Import(string type, int version, string[] data)
        {
            await Instance.Import(type, version, data);
        }

        private async Task Import(string type, int version, string[] data)
        {
            if (type == "jsonl" && version == 1)
                await _jsonlImporter.Import(data);
            else
                throw new InvalidOperationException($"Unsupported import model: {type}:{version}");
        }
    }
}
