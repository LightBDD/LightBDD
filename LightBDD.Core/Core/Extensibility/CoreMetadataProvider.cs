using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Formatting;
using LightBDD.Core.Helpers;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;

namespace LightBDD.Core.Extensibility
{
    public abstract class CoreMetadataProvider : IMetadataProvider
    {
        private readonly INameFormatter _nameFormatter;
        private readonly StepNameParser _stepNameParser;

        protected CoreMetadataProvider(INameFormatter nameFormatter)
        {
            _nameFormatter = nameFormatter;
            _stepNameParser = new StepNameParser(nameFormatter);
        }

        protected INameFormatter NameFormatter
        {
            get { return _nameFormatter; }
        }

        public IFeatureInfo GetFeatureInfo(Type featureType)
        {
            return new FeatureInfo(GetFeatureName(featureType), GetFeatureLabels(featureType), GetFeatureDescription(featureType));
        }

        public abstract MethodBase CaptureCurrentScenarioMethod();

        public virtual INameInfo GetScenarioName(MethodBase scenarioMethod)
        {
            return new NameInfo(_nameFormatter.FormatName(scenarioMethod.Name), Arrays<INameParameterInfo>.Empty());
        }

        public virtual string[] GetScenarioLabels(MethodBase scenarioMethod)
        {
            return ExtractAttributePropertyValues<LabelAttribute>(scenarioMethod, a => a.Label).OrderBy(l => l).ToArray();
        }

        public string[] GetScenarioCategories(MethodBase scenarioMethod)
        {
            return ExtractAttributePropertyValues<ScenarioCategoryAttribute>(scenarioMethod, a => a.Name)
                .Concat(ExtractAttributePropertyValues<ScenarioCategoryAttribute>(scenarioMethod.DeclaringType.GetTypeInfo(), a => a.Name))
                .Concat(GetImplementationSpecificScenarioCategories(scenarioMethod))
                .Concat(GetImplementationSpecificScenarioCategories(scenarioMethod.DeclaringType.GetTypeInfo()))
                .Distinct()
                .OrderBy(c => c)
                .ToArray();
        }

        public IStepNameInfo GetStepName(StepDescriptor stepDescriptor)
        {
            return new StepNameInfo(
                stepDescriptor.PredefinedStepType,
                _stepNameParser.GetStepNameFormat(stepDescriptor),
                stepDescriptor.Parameters.Select(p => NameParameterInfo.Unknown).ToArray());
        }

        public Func<object, string> GetStepParameterFormatter(ParameterInfo parameter)
        {
            return value => string.Format("{0}", value);
        }

        protected abstract IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member);

        protected virtual string GetFeatureDescription(Type featureType)
        {
            return ExtractAttributePropertyValue<FeatureDescriptionAttribute>(featureType.GetTypeInfo(), a => a.Description)
                   ?? GetImplementationSpecificFeatureDescription(featureType);
        }

        protected abstract string GetImplementationSpecificFeatureDescription(Type featureType);

        /// <summary>
        /// Retrieves specified attribute property value.
        /// </summary>
        protected static string ExtractAttributePropertyValue<TAttribute>(MemberInfo member, Func<TAttribute, string> valueExtractor) where TAttribute : Attribute
        {
            return ExtractAttributePropertyValues(member, valueExtractor).SingleOrDefault();
        }

        /// <summary>
        /// Retrieves specified attribute property value for all attribute instances applied on given member.
        /// </summary>
        protected static IEnumerable<string> ExtractAttributePropertyValues<TAttribute>(MemberInfo member, Func<TAttribute, string> valueExtractor) where TAttribute : Attribute
        {
            return ExtractAttributes<TAttribute>(member).Select(valueExtractor);
        }
        /// <summary>
        /// Retrieves specified attributes applied on given member.
        /// </summary>
        protected static IEnumerable<TAttribute> ExtractAttributes<TAttribute>(MemberInfo member) where TAttribute : Attribute
        {
            return member.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>();
        }

        protected virtual string[] GetFeatureLabels(Type featureType)
        {
            return ExtractAttributePropertyValues<LabelAttribute>(featureType.GetTypeInfo(), a => a.Label).OrderBy(l => l).ToArray();
        }

        protected virtual INameInfo GetFeatureName(Type featureType)
        {
            return new NameInfo(_nameFormatter.FormatName(featureType.Name), Arrays<INameParameterInfo>.Empty());
        }
    }
}