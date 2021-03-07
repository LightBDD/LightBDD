using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace LightBDD.Reporting.Progressive.UI.Services
{
    public class InteropService
    {
        private static InteropService _instance;
        private readonly IJSRuntime _runtime;

        public InteropService(IJSRuntime runtime)
        {
            _runtime = runtime;
        }

        public async Task Initialize()
        {
            _instance = this;
            await _runtime.InvokeVoidAsync("comms.onInitialized");
        }

        [JSInvokable]
        public static void InteropService_Import(string type, int version, string data)
        {
            throw new InvalidOperationException($"{type}:{version}:{data}");
        }
    }
}
