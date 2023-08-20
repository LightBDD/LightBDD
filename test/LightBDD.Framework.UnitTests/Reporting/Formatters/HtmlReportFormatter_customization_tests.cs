using System;
using System.IO;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using LightBDD.Core.Configuration;
using LightBDD.Core.Reporting;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Reporting;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.UnitTests.Helpers;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Reporting.Formatters;

public class HtmlReportFormatter_customization_tests
{
    [Test]
    public void It_should_allow_CSS_customization()
    {
        var formatter = ConfigureFormatter(f => f
            .WithCustomCss("_CUSTOM_CSS_"));

        var doc = GetHtml(formatter);

        var style = doc.DocumentNode.SelectNodes("html/head/style").Last();
        Assert.That(style.InnerText, Is.EqualTo("_CUSTOM_CSS_"));
    }

    [Test]
    public void It_should_allow_Favicon_customization()
    {
        var favIconBytes = new byte[] { 1, 2, 3 };
        var expectedBase64 = $"data:data/custom-favicon;base64,{Convert.ToBase64String(favIconBytes)}";

        var formatter = ConfigureFormatter(f => f
            .WithCustomFavicon("data/custom-favicon", favIconBytes));

        var doc = GetHtml(formatter);

        var link = doc.DocumentNode.SelectNodes("html/head/link").First();
        Assert.That(link.GetAttributeValue("type", null), Is.EqualTo("data/custom-favicon"));
        Assert.That(link.GetAttributeValue("href", null), Is.EqualTo(expectedBase64));
    }

    [Test]
    public void It_should_allow_logo_customization()
    {
        var logoBytes = new byte[] { 4, 5, 6 };
        var expectedCssIcon = $"--logo-ico: url('data:data/custom-logo;base64,{Convert.ToBase64String(logoBytes)}');";

        var formatter = ConfigureFormatter(f => f
            .WithCustomLogo("data/custom-logo", logoBytes));

        var doc = GetHtml(formatter);

        var found = doc.DocumentNode.SelectNodes("html/head/style").Any(n => n.InnerText.Contains(expectedCssIcon));
        Assert.That(found, Is.True);
    }

    /// <summary>
    /// Configures formatter using official configuration method
    /// </summary>
    //TODO: review
    private IReportFormatter ConfigureFormatter(Action<HtmlReportFormatter> onConfigure)
    {
        var cfg = new LightBddConfiguration();
        cfg.Services.ConfigureReportGenerators()
            .Clear()
            .AddFileReport("foo.html", onConfigure);

        var g = (FileReportGenerator)cfg.Services.Single(x => x.ServiceType == typeof(IReportGenerator))
            .ImplementationFactory(Mock.Of<IServiceProvider>());

        return g.Formatter;
    }
    private HtmlDocument GetHtml(IReportFormatter fmt)
    {
        using var memory = new MemoryStream();
        fmt.Format(memory, new TestResults.TestTestRunResult());
        var html = Encoding.UTF8.GetString(memory.ToArray());
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        return doc;
    }
}