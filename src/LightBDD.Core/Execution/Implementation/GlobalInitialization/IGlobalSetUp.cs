using System.Threading.Tasks;
using LightBDD.Core.Dependencies;

namespace LightBDD.Core.Execution.Implementation.GlobalInitialization;

internal interface IGlobalSetUp
{
    Task SetUpAsync(IDependencyResolver resolver);
    Task CleanUpAsync(IDependencyResolver resolver);
}