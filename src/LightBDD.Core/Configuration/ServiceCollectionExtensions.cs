using System;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Formatting;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Core.Notification;
using LightBDD.Core.Reporting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LightBDD.Core.Configuration;

/// <summary>
/// Extension class allowing to configure implementation of core LightBDD features.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures <see cref="ICultureInfoProvider"/> feature implementation with <paramref name="onConfigure"/> delegate.
    /// </summary>
    public static IServiceCollection ConfigureCultureInfoProvider(this IServiceCollection collection, Action<SingletonDescriptor<ICultureInfoProvider>> onConfigure) => collection.ConfigureFeature(onConfigure);

    /// <summary>
    /// Configures <see cref="INameFormatter"/> feature implementation with <paramref name="onConfigure"/> delegate.
    /// </summary>
    public static IServiceCollection ConfigureNameFormatter(this IServiceCollection collection, Action<SingletonDescriptor<INameFormatter>> onConfigure) => collection.ConfigureFeature(onConfigure);

    /// <summary>
    /// Configures <see cref="IExceptionFormatter"/> feature implementation with <paramref name="onConfigure"/> delegate.
    /// </summary>
    public static IServiceCollection ConfigureExceptionFormatter(this IServiceCollection collection, Action<SingletonDescriptor<IExceptionFormatter>> onConfigure) => collection.ConfigureFeature(onConfigure);

    /// <summary>
    /// Configures <see cref="IFixtureFactory"/> feature implementation with <paramref name="onConfigure"/> delegate.
    /// </summary>
    public static IServiceCollection ConfigureFixtureFactory(this IServiceCollection collection, Action<SingletonDescriptor<IFixtureFactory>> onConfigure) => collection.ConfigureFeature(onConfigure);

    /// <summary>
    /// Configures <see cref="IFileAttachmentsManager"/> feature implementation with <paramref name="onConfigure"/> delegate.
    /// </summary>
    public static IServiceCollection ConfigureFileAttachmentsManager(this IServiceCollection collection, Action<SingletonDescriptor<IFileAttachmentsManager>> onConfigure) => collection.ConfigureFeature(onConfigure);

    /// <summary>
    /// Configures <see cref="IScenarioDecorator"/> collection that will be used for all executed scenarios.
    /// </summary>
    public static ServiceCollectionRegistrator<IScenarioDecorator> ConfigureScenarioDecorators(this IServiceCollection collection) => new(collection);

    /// <summary>
    /// Configures <see cref="IStepDecorator"/> collection that will be used for all steps of executed scenarios.
    /// </summary>
    public static ServiceCollectionRegistrator<IStepDecorator> ConfigureStepDecorators(this IServiceCollection collection) => new(collection);

    /// <summary>
    /// Configures <see cref="IReportGenerator"/> collection that will be used to generate reports after test execution.
    /// </summary>
    public static ServiceCollectionRegistrator<IReportGenerator> ConfigureReportGenerators(this IServiceCollection collection) => new(collection);

    /// <summary>
    /// Configures <see cref="IProgressNotifier"/> collection that will be used to notify execution progress of entire LightBDD pipeline.
    /// </summary>
    public static ProgressNotifierRegistrator ConfigureProgressNotifiers(this IServiceCollection collection) => new(collection);

    /// <summary>
    /// Configures global setup and tear down actions.
    /// </summary>
    public static GlobalSetupRegistrator ConfigureGlobalSetUp(this IServiceCollection collection) => new(collection);

    /// <summary>
    /// Configures <typeparamref name="TFeature"/> feature implementation with dependency described by <paramref name="implementationDescriptorFn"/> function.<br/>
    /// The dependency is registered as singleton. If feature was already configured, it's former definition is replaced by this one.
    /// </summary>
    /// <typeparam name="TFeature">Feature type</typeparam>
    /// <param name="collection">Collection.</param>
    /// <param name="implementationDescriptorFn">Implementation descriptor function.</param>
    /// <returns></returns>
    public static IServiceCollection ConfigureFeature<TFeature>(this IServiceCollection collection, Action<SingletonDescriptor<TFeature>> implementationDescriptorFn) where TFeature : class
    {
        var reg = new SingletonDescriptor<TFeature>();
        implementationDescriptorFn.Invoke(reg);

        collection.Replace(reg.GetDescriptor());
        return collection;
    }
}