using System;
using LightBDD.Framework.Messaging;

namespace LightBDD.Framework.UnitTests.Messaging.Helpers
{
    class MessageSource : IMessageSource
    {
        public event Action<object> OnMessage;
        public Delegate[] GetInvocationList() => OnMessage?.GetInvocationList() ?? Array.Empty<Delegate>();

        public T Publish<T>(T message)
        {
            OnMessage?.Invoke(message);
            return message;
        }
    }
}