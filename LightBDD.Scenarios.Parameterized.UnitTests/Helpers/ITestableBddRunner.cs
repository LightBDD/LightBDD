using LightBDD.Core.Extensibility;

namespace LightBDD.Scenarios.Parameterized.UnitTests.Helpers
{
    public interface ITestableBddRunner<T> : IBddRunner<T>, ICoreBddRunner { }
}