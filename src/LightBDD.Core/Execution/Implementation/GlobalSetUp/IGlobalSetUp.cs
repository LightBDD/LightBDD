using System.Threading.Tasks;
using LightBDD.Core.Dependencies;

namespace LightBDD.Core.Execution.Implementation.GlobalSetUp;

internal interface IGlobalSetUp
{
    Task SetUpAsync(IDependencyResolver resolver);
    Task TearDownAsync(IDependencyResolver resolver);
}