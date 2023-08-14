using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting.ExceptionFormatting;

namespace LightBDD.Core.UnitTests.Helpers;

internal class TestLightBddConfiguration
{
    public static void SetTestDefaults(LightBddConfiguration cfg)
    {
        //TODO: review to avoid any overrides
        cfg.RegisterExceptionFormatter(x => x.Use(new TestExceptionFormatter()));
    }
    class TestExceptionFormatter : IExceptionFormatter
    {
        public string Format(Exception e) => $"{e.GetType().Namespace}.{e.GetType().Name}: {e.Message}";
    }
}