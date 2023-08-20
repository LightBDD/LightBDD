using System;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Formatting;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Core.Notification;
using LightBDD.Core.Reporting;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// Extension class allowing to configure core parts of LightBDD.
    /// </summary>
    public static class ConfigurationExtensions
    {
        public static LightBddConfiguration RegisterCultureInfoProvider(this LightBddConfiguration cfg, Action<SingletonDescriptor<ICultureInfoProvider>> onRegister) => cfg.RegisterFeature(onRegister);
        public static LightBddConfiguration RegisterNameFormatter(this LightBddConfiguration cfg, Action<SingletonDescriptor<INameFormatter>> onRegister) => cfg.RegisterFeature(onRegister);
        public static LightBddConfiguration RegisterExceptionFormatter(this LightBddConfiguration cfg, Action<SingletonDescriptor<IExceptionFormatter>> onRegister) => cfg.RegisterFeature(onRegister);
        public static LightBddConfiguration RegisterFixtureFactory(this LightBddConfiguration cfg, Action<SingletonDescriptor<IFixtureFactory>> onRegister) => cfg.RegisterFeature(onRegister);
        public static LightBddConfiguration RegisterFileAttachmentsManager(this LightBddConfiguration cfg, Action<SingletonDescriptor<IFileAttachmentsManager>> onRegister) => cfg.RegisterFeature(onRegister);
        public static ServiceCollectionRegistrator<IScenarioDecorator> RegisterScenarioDecorators(this LightBddConfiguration cfg) => new(cfg.Services);
        public static ServiceCollectionRegistrator<IStepDecorator> RegisterStepDecorators(this LightBddConfiguration cfg) => new(cfg.Services);
        public static ServiceCollectionRegistrator<IReportGenerator> RegisterReportGenerators(this LightBddConfiguration cfg) => new(cfg.Services);
        public static ProgressNotifierRegistrator RegisterProgressNotifiers(this LightBddConfiguration cfg) => new(cfg);
        public static GlobalSetupRegistrator RegisterGlobalSetUp(this LightBddConfiguration cfg) => new(cfg);

        public static LightBddConfiguration RegisterFeature<T>(this LightBddConfiguration cfg, Action<SingletonDescriptor<T>> onRegister) where T : class
        {
            var reg = new SingletonDescriptor<T>();
            onRegister.Invoke(reg);

            cfg.Services.Replace(reg.GetDescriptor());
            return cfg;
        }

        /// <summary>
        /// Returns step type configuration.
        /// </summary>
        /// <param name="cfg">Configuration object.</param>
        /// <returns>Step type configuration.</returns>
        public static StepTypeConfiguration ConfigureStepTypes(this LightBddConfiguration cfg) 
            => cfg.ConfigureFeature<StepTypeConfiguration>();

        /// <summary>
        /// Retrieves <see cref="ConfigureValueFormatting"/> from <paramref name="configuration"/> for further customizations.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        public static ValueFormattingConfiguration ConfigureValueFormatting(this LightBddConfiguration configuration) 
            => configuration.ConfigureFeature<ValueFormattingConfiguration>();

        /// <summary>
        /// Retrieves <see cref="ConfigureMetadata"/> from <paramref name="configuration"/> for further customizations.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Configuration object.</returns>
        public static MetadataConfiguration ConfigureMetadata(this LightBddConfiguration configuration) 
            => configuration.ConfigureFeature<MetadataConfiguration>();

        public static ExecutionPipelineConfiguration ConfigureExecutionPipeline(this LightBddConfiguration configuration)
            => configuration.ConfigureFeature<ExecutionPipelineConfiguration>();
    }
}