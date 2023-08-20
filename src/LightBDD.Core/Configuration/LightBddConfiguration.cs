using System;
using System.Collections.Concurrent;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Dependencies.Implementation;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution.Implementation;
using LightBDD.Core.Extensibility.Implementation;
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
        private readonly ServiceCollection _collection = new();

        /// <summary>
        /// Default constructor that configures core features
        /// </summary>
        public LightBddConfiguration()
        {
            RegisterCoreDependencies();
            RegisterCoreFeatures();
            RegisterDefaultConfigurations();
        }

        private void RegisterCoreDependencies()
        {
            _collection.AddTransient<TransientDisposable>();
            _collection.AddSingleton<CoreMetadataProvider>();
            _collection.AddSingleton<ExceptionProcessor>();
            _collection.AddSingleton<ProgressNotificationDispatcher>();
            _collection.AddSingleton<ValueFormattingService>();
            _collection.AddSingleton<GlobalDecoratorsProvider>();
            _collection.AddSingleton<IValueFormattingService, ValueFormattingService>(p => p.GetRequiredService<ValueFormattingService>());
        }

        private void RegisterCoreFeatures()
        {
            Services
                .ConfigureCultureInfoProvider(c => c.Use<DefaultCultureInfoProvider>())
                .ConfigureNameFormatter(c => c.Use(NoNameFormatter.Instance))
                .ConfigureExceptionFormatter(c => c.Use<DefaultExceptionFormatter>())
                .ConfigureFileAttachmentsManager(c => c.Use(NoFileAttachmentsManager.Instance))
                .ConfigureFixtureFactory(c => c.Use<DefaultFixtureFactory>());
        }

        private void RegisterDefaultConfigurations()
        {
            this.ForMetadata();
            this.ForStepTypes();
            this.ForValueFormatting();
            this.ForExecutionPipeline();
        }

        /// <summary>
        /// Returns feature configuration of requested type.
        /// If there was no configuration specified for given feature, the default configuration would be instantiated and returned for further customizations.
        /// </summary>
        /// <typeparam name="TFeatureConfiguration">Feature configuration type.</typeparam>
        /// <returns>Feature configuration instance.</returns>
        public TFeatureConfiguration Get<TFeatureConfiguration>() where TFeatureConfiguration : FeatureConfiguration, new()
        {
            return (TFeatureConfiguration)_configuration.GetOrAdd(typeof(TFeatureConfiguration), _ =>
            {
                var cfg = new TFeatureConfiguration();
                if (IsSealed)
                    cfg.Seal();
                return cfg;
            });
        }

        /// <summary>
        /// Returns service collection that can be used to register further dependencies.<br/>
        /// When configuration is sealed, the service collection is read-only.
        /// </summary>
        public IServiceCollection Services => _collection;

        /// <summary>
        /// Seals configuration making it immutable.
        /// It calls <see cref="FeatureConfiguration.Seal"/>() method on all configuration items that implements the <see cref="FeatureConfiguration"/> interface.
        /// Since this call, the <see cref="Get{TFeatureConfiguration}"/>() method will return only sealed configuration (current, and future default one).
        /// </summary>
        /// <returns>Self.</returns>
        public LightBddConfiguration Seal()
        {
            if (IsSealed)
                return this;
            _collection.MakeReadOnly();
            IsSealed = true;
            foreach (var value in _configuration.Values)
                value.Seal();
            return this;
        }

        /// <summary>
        /// Returns true if configuration is sealed.
        /// </summary>
        public bool IsSealed { get; private set; }

        /// <summary>
        /// Seals the configuration and builds <see cref="IDependencyContainer"/> based on this configuration.<br/>
        /// Sealed configuration disables ability to register new dependencies via <see cref="Services"/> and modifying feature configurations obtained via <see cref="Get{TFeatureConfiguration}"/>.<br/>
        /// Every call to this method creates new instance of <see cref="IDependencyContainer"/>, which needs to be independently discarded
        /// </summary>
        /// <returns>Built container</returns>
        public IDependencyContainer BuildContainer()
        {
            Seal();

            var collection = new ServiceCollection { _collection };
            foreach (var cfg in _configuration.Values)
                collection.Add(ServiceDescriptor.Singleton(cfg.GetType(), cfg));

            return new DependencyContainer(collection);
        }
    }
}
