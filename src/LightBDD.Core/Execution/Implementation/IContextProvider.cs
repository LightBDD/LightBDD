using System;

namespace LightBDD.Core.Execution.Implementation
{
    internal interface IContextProvider : IDisposable
    {
        object GetContext();
    }
}