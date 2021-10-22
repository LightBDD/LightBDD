using System;
using LightBDD.Framework.Messaging;

namespace LightBDD.Framework.UnitTests.Messaging.Helpers
{
    class MessageSource : IMessageSource
    {
        public void Handle(object message) => OnMessage?.Invoke(message);
        public event Action<object> OnMessage;
        public Delegate[] GetInvocationList() => OnMessage?.GetInvocationList() ?? Array.Empty<Delegate>();
    }
}