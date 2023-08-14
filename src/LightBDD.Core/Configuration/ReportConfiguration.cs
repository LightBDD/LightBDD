#nullable enable
using System;
using System.Collections.Generic;
using LightBDD.Core.Notification;
using LightBDD.Core.Reporting;
using Microsoft.Extensions.DependencyInjection;

namespace LightBDD.Core.Configuration;

/// <summary>
/// Configuration class allowing to customize scenario progress notification behavior.
/// </summary>
//TODO: consider generalizing collection registrations
public class ReportConfiguration : FeatureConfiguration
{
    private readonly List<ServiceDescriptor> _generators = new();

    /// <summary>
    /// Returns <see cref="IReportGenerator"/> registrations.<br/>
    /// By default it is configured with no notifiers.
    /// </summary>
    //TODO: review how these descriptors are registered. Consider hiding it. Consider also reworking global service collection to provide only one way to setup and clear specific registrations.
    public IReadOnlyList<ServiceDescriptor> Generators => _generators;

    /// <summary>
    /// Registers <see cref="IReportGenerator"/> described by <paramref name="configurer"/> to notifier collection, making all of them used during notification.
    /// </summary>
    /// <param name="configurer"><see cref="IReportGenerator"/> configurer</param>
    /// <returns>Self</returns>
    /// <exception cref="InvalidOperationException">Throws when configuration is sealed.</exception>
    public ReportConfiguration Add(Action<FeatureConfigurer<IReportGenerator>> configurer)
    {
        ThrowIfSealed();
        var cfg = new FeatureConfigurer<IReportGenerator>();
        configurer.Invoke(cfg);
        _generators.Add(cfg.GetDescriptor());
        return this;
    }

    /// <summary>
    /// Clears <see cref="Generators"/>.
    /// </summary>
    /// <returns>Self.</returns>
    public ReportConfiguration Clear()
    {
        ThrowIfSealed();
        _generators.Clear();
        return this;
    }
}