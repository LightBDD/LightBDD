using System.Linq;
using System.Threading.Tasks;
using Xunit.v3;

namespace LightBDD.Runner.Implementation;

internal class LightBddTestFrameworkExecutor : XunitTestFrameworkExecutor
{
    public LightBddTestFrameworkExecutor(IXunitTestAssembly testAssembly)
        : base(testAssembly)
    {
    }

    protected override async ValueTask<ITestEngineStatus> OnTestAssemblyExecutionFinished(ITestEngineStatus status)
    {
        // Custom cleanup logic if needed
        return await base.OnTestAssemblyExecutionFinished(status);
    }
}