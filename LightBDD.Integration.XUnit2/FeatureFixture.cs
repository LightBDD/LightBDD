using LightBDD.Core.Notification;
using LightBDD.Integration.XUnit2;
using Xunit.Abstractions;

namespace LightBDD
{
    public class FeatureFixture
    {
        protected ITestOutputHelper Output { get; }
        protected IBddRunner Runner { get; }

        protected FeatureFixture(ITestOutputHelper output)
        {
            Output = output;
            Runner = XUnitBddRunnerFactory.Instance.GetRunnerFor(GetType(), CreateProgressNotifier).AsBddRunner();
        }

        protected virtual IProgressNotifier CreateProgressNotifier() { return new XUnitProgressNotifier(Output); }
    }
}