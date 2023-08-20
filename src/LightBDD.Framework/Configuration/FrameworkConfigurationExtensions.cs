﻿using System;
using System.IO;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Reporting;
using LightBDD.Framework.Formatting;
using LightBDD.Framework.Formatting.Values;
using LightBDD.Framework.Reporting;
using LightBDD.Framework.Reporting.Formatters;
using Microsoft.Extensions.DependencyInjection;

namespace LightBDD.Framework.Configuration;

/// <summary>
/// Extensions allowing to apply and configure framework default configuration.
/// </summary>
public static class FrameworkConfigurationExtensions
{
    /// <summary>
    /// Applies framework default configuration.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    /// <returns><paramref name="configuration"/>.</returns>
    public static LightBddConfiguration WithFrameworkDefaults(this LightBddConfiguration configuration)
    {
        configuration.ForObjectTrees();

        configuration.ForMetadata()
            .RegisterEngineAssembly(typeof(FrameworkConfigurationExtensions).Assembly);

        configuration
            .ForValueFormatting()
            .RegisterFrameworkDefaultGeneralFormatters();

        configuration.Services.ConfigureReportGenerators()
            .AddFrameworkDefaultReportGenerators();

        configuration.Services
            .ConfigureNameFormatter(c => c.Use(DefaultNameFormatter.Instance))
            .ConfigureDefaultFileAttachmentManager();

        return configuration;
    }

    /// <summary>
    /// Applies framework default general formatters.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    /// <returns><paramref name="configuration"/>.</returns>
    public static ValueFormattingConfiguration RegisterFrameworkDefaultGeneralFormatters(this ValueFormattingConfiguration configuration)
    {
        return configuration
            .RegisterGeneral(new DictionaryFormatter())
            .RegisterGeneral(new CollectionFormatter());
    }

    /// <summary>
    /// Applies default report generators to generate <c>~\\Reports\\FeaturesReport.html</c>(Win) <c>~/Reports/FeaturesReport.html</c>(Unix) reports.
    /// </summary>
    public static ServiceCollectionRegistrator<IReportGenerator> AddFrameworkDefaultReportGenerators(this ServiceCollectionRegistrator<IReportGenerator> configuration)
    {
        return configuration.AddFileReport<HtmlReportFormatter>($"~{Path.DirectorySeparatorChar}Reports{Path.DirectorySeparatorChar}FeaturesReport.html");
    }

    /// <summary>
    /// Applies default file attachment manager configuration to generate attachments in <c>~\\Reports\</c>(Win) <c>~/Reports/</c>(Unix) directory.
    /// </summary>
    public static IServiceCollection ConfigureDefaultFileAttachmentManager(this IServiceCollection collection)
    {
        return collection.ConfigureFileAttachmentsManager(x => x.Use(_ => new FileAttachmentsManager($"~{Path.DirectorySeparatorChar}Reports")));
    }

    /// <summary>
    /// Adds <see cref="FileReportGenerator"/> instance configured to format report with <typeparamref name="TFormatter"/> and write it to <paramref name="outputPath"/>.
    /// </summary>
    /// <typeparam name="TFormatter">Type of report formatter.</typeparam>
    /// <param name="configuration">Configuration.</param>
    /// <param name="outputPath">Output path for the report.</param>
    /// <returns>Configuration.</returns>
    public static ServiceCollectionRegistrator<IReportGenerator> AddFileReport<TFormatter>(this ServiceCollectionRegistrator<IReportGenerator> configuration, string outputPath) where TFormatter : IReportFormatter, new()
    {
        return AddFileReport<TFormatter>(configuration, outputPath, null);
    }

    /// <summary>
    /// Adds <see cref="FileReportGenerator"/> instance configured to format report with <typeparamref name="TFormatter"/> and write it to <paramref name="outputPath"/>.
    /// </summary>
    /// <typeparam name="TFormatter">Type of report formatter.</typeparam>
    /// <param name="configuration">Configuration.</param>
    /// <param name="outputPath">Output path for the report.</param>
    /// <param name="onConfigure">Action to configure the <typeparamref name="TFormatter"/> instance.</param>
    /// <returns>Configuration.</returns>
    public static ServiceCollectionRegistrator<IReportGenerator> AddFileReport<TFormatter>(this ServiceCollectionRegistrator<IReportGenerator> configuration, string outputPath, Action<TFormatter> onConfigure) where TFormatter : IReportFormatter, new()
    {
        var formatter = new TFormatter();
        onConfigure?.Invoke(formatter);
        return configuration.Add(_ => new FileReportGenerator(formatter, outputPath));
    }

    /// <summary>
    /// Provides configuration for object trees.
    /// </summary>
    public static ObjectTreeConfiguration ForObjectTrees(this LightBddConfiguration configuration)
    {
        return configuration.Get<ObjectTreeConfiguration>();
    }
}