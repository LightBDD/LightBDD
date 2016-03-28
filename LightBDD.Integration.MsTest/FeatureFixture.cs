using LightBDD.Core.Notification;
using LightBDD.Integration.MsTest;

namespace LightBDD
{
    public class FeatureFixture
    {
        protected IBddRunner Runner { get; }

        protected FeatureFixture()
        {
            Runner = MsTestBddRunnerFactory.Instance.GetRunnerFor(GetType(), CreateProgressNotifier)
                .AsBddRunner();
        }

        protected virtual IProgressNotifier CreateProgressNotifier()
        {
            return new MsTestProgressNotifier();
        }
    }
}