using System;
using System.Threading.Tasks;

namespace LightBDD.Core.Execution.Dependencies
{
    public interface IDependencyResolver
    {
        Task<object> ResolveAsync(Type type);
        Task<object> RegisterInstance(object instance, bool takeOwnership);
    }
}