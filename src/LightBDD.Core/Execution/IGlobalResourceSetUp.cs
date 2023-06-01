using System.Threading.Tasks;

namespace LightBDD.Core.Execution;

/// <summary>
/// Interface describing global resource requiring initialization and/or cleanup
/// </summary>
public interface IGlobalResourceSetUp
{
    /// <summary>
    /// Action setting up the resource state, executed before any tests run.
    /// </summary>
    Task SetUpAsync();
    /// <summary>
    /// Action cleaning up the resource state, executed after all tests run, but only if <seealso cref="SetUpAsync"/> action was executed. 
    /// </summary>
    Task CleanUpAsync();
}