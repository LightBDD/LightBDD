﻿using System;
using System.Collections.Generic;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Formatting;
using NUnit.Framework.Internal;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public class TestMetadataProvider : CoreMetadataProvider
    {
        protected override IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member)
        {
            return ExtractAttributePropertyValues<CustomCategoryAttribute>(member, a => a.Name);
        }

        protected override string GetImplementationSpecificFeatureDescription(Type featureType)
        {
            return ExtractAttributePropertyValue<CustomFeatureDescriptionAttribute>(featureType.GetTypeInfo(),
                a => a.Description);
        }

        protected override string GetImplementationSpecificScenarioDescription(ScenarioDescriptor scenarioDescriptor)
        {
            return ExtractAttributePropertyValue<CustomFeatureDescriptionAttribute>(scenarioDescriptor.MethodInfo,
                a => a.Description);
        }

        public override ScenarioDescriptor CaptureCurrentScenario()
        {
            return new ScenarioDescriptor(TestExecutionContext.CurrentContext.CurrentTest.Method.MethodInfo, null);
        }

        public TestMetadataProvider() : base(Configure(_=>{}))
        {
        }

        public TestMetadataProvider(LightBddConfiguration configuration):
            base(configuration){}


        public TestMetadataProvider(Action<LightBddConfiguration> onConfigure)
            :base(Configure(onConfigure))
        {
        }

        private static LightBddConfiguration Configure(Action<LightBddConfiguration> onConfigure)
        {
            var configuration = new LightBddConfiguration();
            configuration.NameFormatterConfiguration().UpdateFormatter(DefaultNameFormatter.Instance);
            onConfigure(configuration);
            return configuration;
        }
    }
}