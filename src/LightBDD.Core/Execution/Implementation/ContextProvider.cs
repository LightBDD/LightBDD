using System;
using System.Diagnostics;

namespace LightBDD.Core.Execution.Implementation
{
    [DebuggerStepThrough]
    internal class ContextProvider : IContextProvider
    {
        private readonly Func<object> _contextProvider;
        private readonly bool _takeOwnership;
        private bool _disposed;
        private object _context;
        public static readonly IContextProvider NoContext = new NoContextProvider();

        public ContextProvider(Func<object> contextProvider, bool takeOwnership)
        {
            _contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider), "Context provider has to be specified.");
            _takeOwnership = takeOwnership;
        }

        public object GetContext()
        {
            if (_disposed)
                throw new ObjectDisposedException("Context is already disposed");

            return _context ?? (_context = _contextProvider.Invoke());
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;

            var disposable = _context as IDisposable;
            _context = null;

            if (_takeOwnership)
                disposable?.Dispose();
        }

        private class NoContextProvider : IContextProvider
        {
            public void Dispose() { }
            public object GetContext() => null;
        }
    }
}