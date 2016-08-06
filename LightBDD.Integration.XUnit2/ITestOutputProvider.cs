using Xunit.Abstractions;

namespace LightBDD
{
    public interface ITestOutputProvider
    {
        ITestOutputHelper TestOutput { get; }
    }
}