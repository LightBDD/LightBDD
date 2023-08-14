using System;
using System.Collections.Concurrent;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Dependencies.Implementation;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Implementation;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Core.Formatting.Implementation;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Notification;
using LightBDD.Core.Reporting;
using Microsoft.Extensions.DependencyInjection;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// LightBDD feature configuration class allowing to configure and/or obtain LightBDD features configuration.
    /// </summary>
    public class LightBddConfiguration
    {
        private readonly ConcurrentDictionary<Type, FeatureConfiguration> _configuration = new();
        private readonly ServiceCollection _collection = new();

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
            _collection.AddSingleton<IValueFormattingService, ValueFormattingService>(p => p.GetRequiredService<ValueFormattingService>());
        }

        private void RegisterCoreFeatures()
        {
            this.RegisterCultureInfoProvider(c => c.Use<DefaultCultureInfoProvider>())
                .RegisterNameFormatter(c => c.Use(NoNameFormatter.Instance))
                .RegisterExceptionFormatter(c => c.Use<DefaultExceptionFormatter>())
                .RegisterFileAttachmentsManager(c => c.Use(NoFileAttachmentsManager.Instance))
                .RegisterFixtureFactory(c => c.Use(ActivatorFixtureFactory.Instance));
        }

        private void RegisterDefaultConfigurations()
        {
            this.ConfigureMetadata();
            this.ConfigureStepTypes();
            this.ConfigureValueFormatting();
        }

        /// <summary>
        /// Returns current feature configuration of requested type.
        /// If there was no configuration specified for given feature, the default configuration would be instantiated and returned for further customizations.
        /// </summary>
        /// <typeparam name="TConfiguration">Feature configuration type.</typeparam>
        /// <returns>Feature configuration instance.</returns>
        public TConfiguration ConfigureFeature<TConfiguration>() where TConfiguration : FeatureConfiguration, new()
        {
            return (TConfiguration)_configuration.GetOrAdd(typeof(TConfiguration), _ =>
            {
                ThrowIfSealed();
                var cfg = new TConfiguration();
                _collection.AddSingleton(cfg);
                return cfg;
            });
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
            onConfigure?.Invoke(_collection);
            return this;
        }

        public IServiceCollection Services => _collection;

        /// <summary>
        /// Seals configuration making it immutable.
        /// It calls <see cref="FeatureConfiguration.Seal"/>() method on all configuration items that implements the <see cref="FeatureConfiguration"/> interface.
        /// Since this call, the <see cref="ConfigureFeature{TConfiguration}"/>() method will return only sealed configuration (current, and future default one).
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
        /// Sealed configuration disables ability to register new dependencies via <see cref="ConfigureDependencies"/> and configuring new features via <see cref="ConfigureFeature{TConfiguration}"/>.<br/>
        /// Every call to this method creates new instance of <see cref="IDependencyContainer"/>, which needs to be independently discarded
        /// </summary>
        /// <returns></returns>
        public IDependencyContainer BuildContainer()
        {
            Seal();
            return new DependencyContainer(_collection.BuildServiceProvider(true));
        }

        private void ThrowIfSealed()
        {
            if (IsSealed)
                throw new InvalidOperationException("Feature configuration is sealed. Please update configuration only during LightBDD initialization.");
        }
    }
}
