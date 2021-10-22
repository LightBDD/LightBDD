using System;

namespace LightBDD.Framework.Messaging
{
    public interface IMessageSource
    {
        public event Action<object> OnMessage;
    }
}