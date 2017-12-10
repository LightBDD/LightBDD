using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;

namespace LightBDD.Core.Extensibility.Implementation
{
    internal class IntegrationContextWrapper : IntegrationContext
    {
        private readonly IIntegrationContext _integrationContext;

        public IntegrationContextWrapper(IIntegrationContext integrationContext)
        {
            _integrationContext = integrationContext;
        }

        public override IMetadataProvider MetadataProvider => _integrationContext.MetadataProvider;
        public override INameFormatter NameFormatter => _integrationContext.NameFormatter;
        public override Func<Exception, ExecutionStatus> ExceptionToStatusMapper => _integrationContext.ExceptionToStatusMapper;
        public override ILightBddProgressNotifier LightBddProgressNotifier => throw new NotImplementedException(); //TODO: fix
        public override IFeatureProgressNotifier FeatureProgressNotifier => _integrationContext.FeatureProgressNotifier;
        public override Func<object, IScenarioProgressNotifier> ScenarioProgressNotifierProvider => _integrationContext.ScenarioProgressNotifierProvider;
        public override IExecutionExtensions ExecutionExtensions => _integrationContext.ExecutionExtensions;
        public override LightBddConfiguration Configuration { get; } = new LightBddConfiguration();
    }
}