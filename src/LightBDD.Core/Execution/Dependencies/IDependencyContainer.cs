using System;

namespace LightBDD.Core.Execution.Dependencies
{
    public interface IDependencyContainer : IDependencyResolver, IDisposable
    {
        IDependencyContainer BeginScope(Action<IContainerConfigurer> configuration = null);
    }

    public interface IContainerConfigurer
    {
        void RegisterInstance(object instance, RegistrationOptions options);
    }

    public class RegistrationOptions
    {
        public static readonly RegistrationOptions Default = new RegistrationOptions { TakeOwnership = true };
        public bool TakeOwnership { get; set; }
    }
}
