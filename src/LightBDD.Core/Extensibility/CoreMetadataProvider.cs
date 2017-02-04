using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility.Implementation;
using LightBDD.Core.Formatting;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Internals;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Metadata provider offering core implementation for providing feature, scenario and step metadata.
    /// </summary>
    [DebuggerStepThrough]
    public abstract class CoreMetadataProvider : IMetadataProvider
    {
        private readonly StepNameParser _stepNameParser;
        private readonly StepTypeProcessor _stepTypeProcessor;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nameFormatter"><see cref="INameFormatter"/> object used to format names.</param>
        /// <param name="stepTypeConfiguration"><see cref="StepTypeConfiguration"/> object used in providing step metadata.</param>
        /// <param name="cultureInfoProvider"><see cref="ICultureInfoProvider"/> object used in providing step parameter formatters.</param>
        protected CoreMetadataProvider(INameFormatter nameFormatter, StepTypeConfiguration stepTypeConfiguration, ICultureInfoProvider cultureInfoProvider)
        {
            if (nameFormatter == null)
                throw new ArgumentNullException(nameof(nameFormatter));
            if (cultureInfoProvider == null)
                throw new ArgumentNullException(nameof(cultureInfoProvider));
            if (stepTypeConfiguration == null)
                throw new ArgumentNullException(nameof(stepTypeConfiguration));

            NameFormatter = nameFormatter;
            CultureInfoProvider = cultureInfoProvider;
            _stepNameParser = new StepNameParser(nameFormatter);
            _stepTypeProcessor = new StepTypeProcessor(nameFormatter, stepTypeConfiguration);
        }

        /// <summary>
        /// Returns currently used <see cref="ICultureInfoProvider"/> instance.
        /// </summary>
        protected ICultureInfoProvider CultureInfoProvider { get; }
        /// <summary>
        /// Returns currently used <see cref="INameFormatter"/> instance.
        /// </summary>
        protected INameFormatter NameFormatter { get; }

        /// <summary>
        /// Provides <see cref="IFeatureInfo"/> object containing information about feature represented by <paramref name="featureType"/>.
        /// 
        /// The <see cref="IFeatureInfo.Name"/> is determined from the <paramref name="featureType"/> name.
        /// The <see cref="IFeatureInfo.Labels"/> are determined from attributes implementing <see cref="ILabelAttribute"/>, applied on <paramref name="featureType"/>.
        /// The <see cref="IFeatureInfo.Description"/> is determined from <see cref="FeatureDescriptionAttribute"/> in first instance, then by <see cref="GetImplementationSpecificFeatureDescription"/>() method. The value may be <c>null</c>.
        /// </summary>
        /// <param name="featureType">Feature type.</param>
        /// <returns><see cref="IFeatureInfo"/> object.</returns>
        public IFeatureInfo GetFeatureInfo(Type featureType)
        {
            return new FeatureInfo(GetFeatureName(featureType), GetFeatureLabels(featureType), GetFeatureDescription(featureType));
        }

        /// <summary>
        /// Provides currently executed scenario method.
        /// </summary>
        /// <returns><see cref="MethodBase"/> describing currently executed scenario method.</returns>
        /// <exception cref="InvalidOperationException">Thrown when called outside of scenario method.</exception>
        public abstract MethodBase CaptureCurrentScenarioMethod();

        /// <summary>
        /// Provides <see cref="INameInfo"/> object containing information about scenario name represented by <paramref name="scenarioMethod"/>.
        /// The name is based on provided method name.
        /// The current implementation ignores scenario parameters, always constructing object with empty <see cref="INameInfo.Parameters"/> collection.
        /// </summary>
        /// <param name="scenarioMethod">Scenario method.</param>
        /// <returns><see cref="INameInfo"/> object.</returns>
        public INameInfo GetScenarioName(MethodBase scenarioMethod)
        {
            return new NameInfo(NameFormatter.FormatName(scenarioMethod.Name), Arrays<INameParameterInfo>.Empty());
        }

        /// <summary>
        /// Provides scenario labels for scenario represented by <paramref name="scenarioMethod"/> which are determined from attributes implementing <see cref="LabelAttribute"/>, applied on the method.
        /// </summary>
        /// <param name="scenarioMethod">Scenario method.</param>
        /// <returns>Scenario labels.</returns>
        public string[] GetScenarioLabels(MethodBase scenarioMethod)
        {
            return ExtractAttributePropertyValues<ILabelAttribute>(scenarioMethod, a => a.Label).OrderBy(l => l).ToArray();
        }

        /// <summary>
        /// Provides scenario categories for scenario represented by <paramref name="scenarioMethod"/>.
        /// The categories are determined from attributes implementing <see cref="IScenarioCategoryAttribute"/>, applied on <paramref name="scenarioMethod"/> and type declaring the method,
        /// as well as from <see cref="GetImplementationSpecificScenarioCategories"/>() executed on <paramref name="scenarioMethod"/> and type declaring the method.
        /// 
        /// The categories specified on base classes will also be retrieved.
        /// </summary>
        /// <param name="scenarioMethod">Scenario method.</param>
        /// <returns>Scenario categories.</returns>
        public string[] GetScenarioCategories(MethodBase scenarioMethod)
        {
            return ExtractAttributePropertyValues<IScenarioCategoryAttribute>(scenarioMethod, a => a.Category)
                .Concat(ExtractAttributePropertyValues<IScenarioCategoryAttribute>(scenarioMethod.DeclaringType.GetTypeInfo(), a => a.Category))
                .Concat(GetImplementationSpecificScenarioCategories(scenarioMethod))
                .Concat(GetImplementationSpecificScenarioCategories(scenarioMethod.DeclaringType.GetTypeInfo()))
                .Distinct()
                .OrderBy(c => c)
                .ToArray();
        }

        /// <summary>
        /// Provides <see cref="IStepNameInfo"/> object containing information about step name represented by <paramref name="stepDescriptor"/>.
        /// The <paramref name="previousStepTypeName"/> represents the step type name of previous step.
        /// <para>
        /// The <see cref="IStepNameInfo.StepTypeName"/> is determined from value <see cref="StepDescriptor.PredefinedStepType"/> or parsed from <see cref="StepDescriptor.RawName"/> if former is <c>null</c>.
        /// When determined step type is the same as <paramref name="previousStepTypeName"/>, it is being replaced with <see cref="StepTypeConfiguration.RepeatedStepReplacement"/>.
        /// </para>
        /// See also: <seealso cref="StepTypeConfiguration"/>, <seealso cref="LightBddConfiguration"/>.
        /// </summary>
        /// <param name="stepDescriptor">Step descriptor.</param>
        /// <param name="previousStepTypeName">Step type name of previous step, or <c>null</c> if current step is first one.</param>
        /// <returns><see cref="IStepNameInfo"/> object.</returns>
        public IStepNameInfo GetStepName(StepDescriptor stepDescriptor, string previousStepTypeName)
        {
            var formattedStepName = _stepNameParser.GetStepNameFormat(stepDescriptor.RawName, stepDescriptor.Parameters);
            return new StepNameInfo(
                _stepTypeProcessor.GetStepTypeName(stepDescriptor.PredefinedStepType, ref formattedStepName, previousStepTypeName),
                formattedStepName,
                stepDescriptor.Parameters.Select(p => NameParameterInfo.Unknown).ToArray());
        }

        /// <summary>
        /// Provides step parameter formatter function for provided <paramref name="parameterInfo"/>.
        /// If <see cref="ParameterFormatterAttribute"/> is applied on <paramref name="parameterInfo"/>, it will be used to retrieve formatter function, otherwise the default one will be provided.
        /// The returned formatter function uses <see cref="CultureInfoProvider"/> to format parameters.
        /// </summary>
        /// <param name="parameterInfo"><see cref="ParameterInfo"/> object describing step or scenario method parameter.</param>
        /// <returns>Formatter function.</returns>
        /// <exception cref="InvalidOperationException">Throws when more than one <see cref="ParameterFormatterAttribute"/> is applied on <paramref name="parameterInfo"/>.</exception>
        public Func<object, string> GetStepParameterFormatter(ParameterInfo parameterInfo)
        {
            Func<object, string> defaultFormatter = value => string.Format(CultureInfoProvider.GetCultureInfo(), "{0}", value);
            var formatters = parameterInfo.GetCustomAttributes(typeof(ParameterFormatterAttribute), true)
               .OfType<ParameterFormatterAttribute>().ToArray();

            if (formatters.Length > 1)
                throw new InvalidOperationException($"Parameter can contain only one attribute ParameterFormatterAttribute. Parameter: {parameterInfo.Name}, Detected attributes: {string.Join(", ", formatters.Select(f => f.GetType().Name).OrderBy(n => n))}");

            return formatters.Length == 1
                ? value => formatters[0].Format(CultureInfoProvider.GetCultureInfo(), value)
                : defaultFormatter;
        }
        /// <summary>
        /// Returns implementation specific scenario categories or empty collection if no categories are provided.
        /// </summary>
        /// <param name="member">Scenario method or feature test class to analyze.</param>
        /// <returns>Scenario categories or empty collection.</returns>
        protected abstract IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member);

        /// <summary>
        /// Returns implementation specific feature description or null if such is not provided.
        /// </summary>
        /// <param name="featureType">Feature type.</param>
        /// <returns>Feature description or null.</returns>
        protected abstract string GetImplementationSpecificFeatureDescription(Type featureType);

        /// <summary>
        /// Provides value of attribute of type <typeparamref name="TAttribute"/> applied on <paramref name="member"/> or default if attribute is not applied.
        /// The attribute is searched in <paramref name="member"/> and it's ancestors.
        /// </summary>
        /// <param name="member">Member to analyze for specified attribute.</param>
        /// <param name="valueExtractor">Attribute value extraction method.</param>
        /// <typeparam name="TAttribute">Type of attribute to extract.</typeparam>
        /// <returns>Attribute value or default.</returns>
        /// <exception cref="InvalidOperationException">Throws when attribute is applied more than once.</exception>
        protected static string ExtractAttributePropertyValue<TAttribute>(MemberInfo member, Func<TAttribute, string> valueExtractor)
        {
            return ExtractAttributePropertyValues(member, valueExtractor).SingleOrDefault();
        }

        /// <summary>
        /// Provides values of all attributes of type <typeparamref name="TAttribute"/> applied on <paramref name="member"/> or empty collection if none are applied.
        /// The attribute is searched in <paramref name="member"/> and it's ancestors.
        /// </summary>
        /// <param name="member">Member to analyze for specified attribute.</param>
        /// <param name="valueExtractor">Attribute value extraction method.</param>
        /// <typeparam name="TAttribute">Type of attribute to extract.</typeparam>
        /// <returns>Values of all attributes or empty collection.</returns>
        protected static IEnumerable<string> ExtractAttributePropertyValues<TAttribute>(MemberInfo member, Func<TAttribute, string> valueExtractor)
        {
            return ExtractAttributes<TAttribute>(member).Select(valueExtractor);
        }

        /// <summary>
        /// Provides all attributes of type <typeparamref name="TAttribute"/> applied on <paramref name="member"/> or empty collection if none are applied.
        /// The attribute is searched in <paramref name="member"/> and it's ancestors.
        /// </summary>
        /// <param name="member">Member to analyze for specified attribute.</param>
        /// <typeparam name="TAttribute">Type of attribute to extract.</typeparam>
        /// <returns>All attributes or empty collection.</returns>
        protected static IEnumerable<TAttribute> ExtractAttributes<TAttribute>(MemberInfo member)
        {
            return member.GetCustomAttributes(true).OfType<TAttribute>();
        }

        /// <summary>
        /// Provides labels from  attributes implementing <see cref="ILabelAttribute"/>, applied on <paramref name="featureType"/>, or empty array if none are present.
        /// </summary>
        /// <param name="featureType">Feature type.</param>
        /// <returns>Array of labels or empty array if none are present.</returns>
        protected string[] GetFeatureLabels(Type featureType)
        {
            return ExtractAttributePropertyValues<ILabelAttribute>(featureType.GetTypeInfo(), a => a.Label).OrderBy(l => l).ToArray();
        }

        /// <summary>
        /// Provides feature name which is determined from name of <paramref name="featureType"/>.
        /// </summary>
        /// <param name="featureType">Feature type.</param>
        /// <returns>Feature name.</returns>
        protected INameInfo GetFeatureName(Type featureType)
        {
            return new NameInfo(NameFormatter.FormatName(featureType.Name), Arrays<INameParameterInfo>.Empty());
        }

        /// <summary>
        /// Provides feature description which is determined from attribute implementing <see cref="IFeatureDescriptionAttribute"/> in first instance, then by <see cref="GetImplementationSpecificFeatureDescription"/>() method. 
        /// Returns description or <c>null</c> if none is present.
        /// </summary>
        /// <param name="featureType">Feature type.</param>
        /// <returns>Feature description or <c>null</c>.</returns>
        protected string GetFeatureDescription(Type featureType)
        {
            return ExtractAttributePropertyValue<IFeatureDescriptionAttribute>(featureType.GetTypeInfo(), a => a.Description)
                   ?? GetImplementationSpecificFeatureDescription(featureType);
        }
    }
}