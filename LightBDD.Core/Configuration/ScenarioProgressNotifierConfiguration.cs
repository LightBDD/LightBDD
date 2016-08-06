using System;
using System.Reflection;
using LightBDD.Core.Notification;

namespace LightBDD.Configuration
{
    public class ScenarioProgressNotifierConfiguration : IFeatureConfiguration
    {
        public Func<object, IScenarioProgressNotifier> NotifierProvider { get; private set; } = fixture => NoProgressNotifier.Default;

        public ScenarioProgressNotifierConfiguration UpdateNotifierProvider(Func<IScenarioProgressNotifier> scenarioProgressNotifier)
        {
            if (scenarioProgressNotifier == null)
                throw new ArgumentNullException(nameof(scenarioProgressNotifier));
            NotifierProvider = fixture => scenarioProgressNotifier();
            return this;
        }

        public ScenarioProgressNotifierConfiguration UpdateNotifierProvider<TFixture>(Func<TFixture, IScenarioProgressNotifier> scenarioProgressNotifier)
        {
            if (scenarioProgressNotifier == null)
                throw new ArgumentNullException(nameof(scenarioProgressNotifier));
            NotifierProvider = fixture => CreateScenarioProgressNotifier(scenarioProgressNotifier, fixture);
            return this;
        }

        private static IScenarioProgressNotifier CreateScenarioProgressNotifier<TFixture>(Func<TFixture, IScenarioProgressNotifier> scenarioProgressNotifier, object fixture)
        {
            if (fixture == null)
                throw new ArgumentNullException(nameof(fixture));

            if (!(fixture is TFixture))
                throw new InvalidOperationException($"Unable to create {nameof(IScenarioProgressNotifier)}. Expected fixture of type '{typeof(TFixture)}' while got '{fixture.GetType()}'.");

            return scenarioProgressNotifier((TFixture)fixture);
        }
    }
}