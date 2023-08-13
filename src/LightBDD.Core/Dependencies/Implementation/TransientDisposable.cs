#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightBDD.Core.Dependencies.Implementation;

internal class TransientDisposable : IAsyncDisposable
{
    private object? _disposable;

    public T WithInstance<T>(T disposable)
    {
        _disposable = disposable;
        return disposable;
    }

    public async ValueTask DisposeAsync()
    {
        var instance = Interlocked.Exchange(ref _disposable, null);

        try
        {
            if (instance is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
            else if (instance is IDisposable disposable)
                disposable.Dispose();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to dispose transient dependency '{instance!.GetType().Name}': {ex.Message}", ex);
        }
    }
}