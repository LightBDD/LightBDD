using System;

namespace LightBDD.Framework.Messaging
{
    /// <summary>
    /// Interface describing message source.
    /// </summary>
    public interface IMessageSource
    {
        /// <summary>
        /// Event triggered when new message appeared on source.
        /// </summary>
        public event Action<object> OnMessage;
    }
}