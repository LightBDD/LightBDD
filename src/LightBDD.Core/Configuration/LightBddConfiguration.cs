using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Dependencies.Implementation;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Implementation;
using LightBDD.Core.Formatting;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Core.Formatting.Implementation;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Notification;
using LightBDD.Core.Reporting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// LightBDD feature configuration class allowing to configure and/or obtain LightBDD features configuration.
    /// </summary>
    public class LightBddConfiguration
    {
        private readonly ConcurrentDictionary<Type, FeatureConfiguration> _configuration = new();
        private readonly ConcurrentDictionary<Type, ServiceDescriptor> _features = new();
        private readonly ServiceCollection _runtimeDependencies = new();

        public LightBddConfiguration()
        {
            ConfigureFeature<ValueFormattingService>(x => x.Use<ValueFormattingService>());
            ConfigureFeature<IValueFormattingService>(x => x.Use(p => p.GetRequiredService<ValueFormattingService>()));
            ConfigureFeature<ICultureInfoProvider>(x => x.Use<DefaultCultureInfoProvider>());
            ConfigureFeature<INameFormatter>(x => x.Use(NoNameFormatter.Instance));
            ConfigureFeature<IExceptionFormatter>(x => x.Use<DefaultExceptionFormatter>());
            ConfigureFeature<IFileAttachmentsManager>(x => x.Use(NoFileAttachmentsManager.Instance));
            ConfigureFeature<IFixtureFactory>(x => x.Use(ActivatorFixtureFactory.Instance));

            Get<ExecutionExtensionsConfiguration>();
            Get<MetadataConfiguration>();
            Get<ProgressNotifierConfiguration>();
            Get<ReportConfiguration>();
            Get<StepTypeConfiguration>();
            Get<ValueFormattingConfiguration>();
        }

        /// <summary>
        /// Returns current feature configuration of requested type.
        /// If there was no configuration specified for given feature, the default configuration would be instantiated and returned for further customizations.
        /// </summary>
        /// <typeparam name="TConfiguration">Feature configuration type.</typeparam>
        /// <returns>Feature configuration instance.</returns>
        public TConfiguration Get<TConfiguration>() where TConfiguration : FeatureConfiguration, new()
        {
            return (TConfiguration)_configuration.GetOrAdd(typeof(TConfiguration), _ => SealIfNeeded(new TConfiguration()));
        }

        public LightBddConfiguration ConfigureFeature<TFeature>(Action<FeatureConfigurer<TFeature>> onConfigure) where TFeature : class
        {
            var cfg = new FeatureConfigurer<TFeature>();
            onConfigure?.Invoke(cfg);
            _features[typeof(TFeature)] = cfg.GetDescriptor();
            return this;
        }

        /// <summary>
        /// Configures runtime dependencies that can be used to instantiate scenarios, decorators, report generators and other types used by LightBDD during test execution.
        /// </summary>
        /// <param name="onConfigure">On configure delegate</param>
        /// <returns>Self.</returns>
        /// <exception cref="InvalidOperationException">Thrown when configuration is sealed.</exception>
        public LightBddConfiguration ConfigureDependencies(Action<IServiceCollection> onConfigure)
        {
            ThrowIfSealed();
            onConfigure?.Invoke(_runtimeDependencies);
            return this;
        }

        private TConfiguration SealIfNeeded<TConfiguration>(TConfiguration config) where TConfiguration : FeatureConfiguration
        {
            if (IsSealed)
                config.Seal();
            return config;
        }

        /// <summary>
        /// Seals configuration making it immutable.
        /// It calls <see cref="FeatureConfiguration.Seal"/>() method on all configuration items that implements the <see cref="FeatureConfiguration"/> interface.
        /// Since this call, the <see cref="Get{TConfiguration}"/>() method will return only sealed configuration (current, and future default one).
        /// </summary>
        /// <returns>Self.</returns>
        public LightBddConfiguration Seal()
        {
            if (IsSealed)
                return this;
            IsSealed = true;
            foreach (var value in _configuration.Values)
                value.Seal();
            return this;
        }

        /// <summary>
        /// Returns true if configuration is sealed.
        /// </summary>
        public bool IsSealed { get; private set; }

        public IDependencyContainer BuildContainer()
        {
            Seal();

            var collection = new ServiceCollection();

            collection.AddTransient<TransientDisposable>();
            collection.AddSingleton<CoreMetadataProvider>();
            collection.AddSingleton<ExceptionProcessor>();
            collection.AddSingleton<ProgressNotificationDispatcher>();

            collection.Add(_features.Values);
            collection.Add(Get<ProgressNotifierConfiguration>().Notifiers);
            collection.Add(Get<ReportConfiguration>().Generators);
            foreach (var cfg in _configuration.Values.Where(v => v.GetType().GetCustomAttributes<InjectableConfigurationAttribute>().Any()))
                collection.AddSingleton(cfg.GetType(), cfg);

            collection.Add(_runtimeDependencies);

            return new DependencyContainer(collection.BuildServiceProvider(true));
        }

        private void ThrowIfSealed()
        {
            if (IsSealed)
                throw new InvalidOperationException("Feature configuration is sealed. Please update configuration only during LightBDD initialization.");
        }
    }
}
