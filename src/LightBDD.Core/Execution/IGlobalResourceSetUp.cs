using System.Threading.Tasks;

namespace LightBDD.Core.Execution;

/// <summary>
/// Interface describing global resource requiring initialization and/or tear down.
/// </summary>
public interface IGlobalResourceSetUp
{
    /// <summary>
    /// Action setting up the resource state, executed before any tests run.
    /// </summary>
    Task SetUpAsync();
    /// <summary>
    /// Action tearing down the resource state, executed after all tests run, but only if <seealso cref="SetUpAsync"/> action was executed. 
    /// </summary>
    Task TearDownAsync();
}