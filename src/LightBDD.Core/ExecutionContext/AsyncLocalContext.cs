using System;
using System.Threading;

namespace LightBDD.Core.ExecutionContext
{
    /// <summary>
    /// Class offering async local storage
    /// </summary>
    /// <typeparam name="T">Stored value</typeparam>
    [Obsolete("Use System.Threading.AsyncLocal<T> instead.")]
    public class AsyncLocalContext<T>
    {
        private readonly AsyncLocal<T> _context = new();
        /// <summary>
        /// Allows to get and set value to store.
        /// </summary>
        public T Value
        {
            get => _context.Value;
            set => _context.Value = value;
        }
    }
}
