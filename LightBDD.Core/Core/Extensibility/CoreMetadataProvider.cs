using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LightBDD.Configuration;
using LightBDD.Core.Extensibility.Implementation;
using LightBDD.Core.Formatting;
using LightBDD.Core.Helpers;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Formatting.Parameters;

namespace LightBDD.Core.Extensibility
{
    public abstract class CoreMetadataProvider : IMetadataProvider
    {
        private readonly INameFormatter _nameFormatter;
        private readonly ICultureInfoProvider _cultureInfoProvider;
        private readonly StepNameParser _stepNameParser;
        private readonly StepTypeProcessor _stepTypeProcessor;

        protected CoreMetadataProvider(INameFormatter nameFormatter, StepTypeConfiguration stepTypeConfiguration, ICultureInfoProvider cultureInfoProvider)
        {
            if (nameFormatter == null)
                throw new ArgumentNullException(nameof(nameFormatter));
            if (cultureInfoProvider == null)
                throw new ArgumentNullException(nameof(cultureInfoProvider));
            if (stepTypeConfiguration == null)
                throw new ArgumentNullException(nameof(stepTypeConfiguration));

            _nameFormatter = nameFormatter;
            _cultureInfoProvider = cultureInfoProvider;
            _stepNameParser = new StepNameParser(nameFormatter);
            _stepTypeProcessor = new StepTypeProcessor(nameFormatter, stepTypeConfiguration);
        }

        protected INameFormatter NameFormatter => _nameFormatter;

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

        public IStepNameInfo GetStepName(StepDescriptor stepDescriptor, string lastStepTypeName)
        {
            var formattedStepName = _stepNameParser.GetStepNameFormat(stepDescriptor.RawName, stepDescriptor.Parameters);
            return new StepNameInfo(
                _stepTypeProcessor.GetStepTypeName(stepDescriptor.PredefinedStepType, ref formattedStepName, lastStepTypeName),
                formattedStepName,
                stepDescriptor.Parameters.Select(p => NameParameterInfo.Unknown).ToArray());
        }

        public Func<object, string> GetStepParameterFormatter(ParameterInfo parameterInfo)
        {
            Func<object, string> defaultFormatter = value => string.Format(_cultureInfoProvider.GetCultureInfo(), "{0}", value);
            var formatters = parameterInfo.GetCustomAttributes(typeof(ParameterFormatterAttribute), true)
               .OfType<ParameterFormatterAttribute>().ToArray();

            if (formatters.Length > 1)
                throw new InvalidOperationException(string.Format(
                    "Parameter can contain only one attribute ParameterFormatterAttribute. Parameter: {0}, Detected attributes: {1}",
                    parameterInfo.Name,
                    string.Join(", ", formatters.Select(f => f.GetType().Name).OrderBy(n => n))));

            return formatters.Length == 1
                ? value => formatters[0].Format(_cultureInfoProvider.GetCultureInfo(), value)
                : defaultFormatter;
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