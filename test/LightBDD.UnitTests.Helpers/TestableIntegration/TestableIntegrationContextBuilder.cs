using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Formatting;
using LightBDD.Framework.Formatting.Configuration;
using LightBDD.Framework.Notification;
using LightBDD.Framework.Notification.Configuration;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public class TestableIntegrationContextBuilder
    {
        private readonly LightBddConfiguration _configuration = new LightBddConfiguration();
        private Func<INameFormatter, IMetadataProvider> _metadataProvider;
        private Func<Exception, ExecutionStatus> _exceptionToStatusMapper;

        public TestableIntegrationContextBuilder WithNameFormatter(INameFormatter formatter)
        {
            _configuration.NameFormatterConfiguration().UpdateFormatter(formatter);
            return this;
        }

        public TestableIntegrationContextBuilder WithMetadataProvider(Func<INameFormatter, IMetadataProvider> provider)
        {
            _metadataProvider = provider;
            return this;
        }

        public TestableIntegrationContextBuilder WithExceptionToStatusMapper(Func<Exception, ExecutionStatus> mapper)
        {
            _exceptionToStatusMapper = mapper;
            return this;
        }

        public TestableIntegrationContextBuilder WithFeatureProgressNotifier(IFeatureProgressNotifier notifier)
        {
            _configuration.FeatureProgressNotifierConfiguration().UpdateNotifier(notifier);
            return this;
        }

        public TestableIntegrationContextBuilder WithScenarioProgressNotifierProvider(Func<object, IScenarioProgressNotifier> provider)
        {
            _configuration.ScenarioProgressNotifierConfiguration().UpdateNotifierProvider(provider);
            return this;
        }
        public TestableIntegrationContextBuilder WithConfiguration(Action<LightBddConfiguration> configurer)
        {
            configurer(_configuration);
            return this;
        }

        public static TestableIntegrationContextBuilder Default()
        {
            return new TestableIntegrationContextBuilder()
                .WithNameFormatter(new DefaultNameFormatter())
                .WithMetadataProvider(nameFormatter => new TestMetadataProvider(nameFormatter))
                .WithExceptionToStatusMapper(ex => ex is CustomIgnoreException ? ExecutionStatus.Ignored : ExecutionStatus.Failed)
                .WithFeatureProgressNotifier(NoProgressNotifier.Default)
                .WithScenarioProgressNotifierProvider(feature => NoProgressNotifier.Default)
                .WithConfiguration(cfg => cfg.ExecutionExtensionsConfiguration().EnableStepExtension<StepCommentHelper>());
        }

        public IntegrationContext Build()
        {
            return new DefaultIntegrationContext(_configuration, _metadataProvider(_configuration.NameFormatterConfiguration().Formatter), _exceptionToStatusMapper);
        }
    }
}