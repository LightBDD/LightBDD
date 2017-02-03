using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers
{
    public interface ITestableBddRunner<T> : IBddRunner<T>, ICoreBddRunner { }
}