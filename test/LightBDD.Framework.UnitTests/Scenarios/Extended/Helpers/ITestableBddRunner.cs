using LightBDD.Core.Extensibility;

namespace LightBDD.Scenarios.Extended.UnitTests.Helpers
{
    public interface ITestableBddRunner<T> : IBddRunner<T>, ICoreBddRunner { }
}