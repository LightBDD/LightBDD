using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Extensibility.Implementation;
using LightBDD.Core.Formatting;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Metadata provider offering core implementation for providing feature, scenario and step metadata.
    /// </summary>
    public abstract class CoreMetadataProvider
    {
        private readonly NameParser _nameParser;
        private readonly StepTypeProcessor _stepTypeProcessor;
        /// <summary>
        /// Returns <see cref="Formatting.Values.ValueFormattingService"/> .
        /// </summary>
        public ValueFormattingService ValueFormattingService { get; }
        /// <summary>
        /// Returns <see cref="INameFormatter"/>.
        /// </summary>
        public INameFormatter NameFormatter { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected CoreMetadataProvider(LightBddConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            ValueFormattingService = new ValueFormattingService(configuration);
            NameFormatter = configuration.NameFormatterConfiguration().GetFormatter();
            _nameParser = new NameParser(NameFormatter);
            _stepTypeProcessor = new StepTypeProcessor(NameFormatter, configuration.StepTypeConfiguration());
        }

        /// <summary>
        /// Provides <see cref="IFeatureInfo"/> object containing information about feature represented by <paramref name="featureType"/>.
        /// 
        /// The <see cref="IFeatureInfo.Name"/> is determined from the <paramref name="featureType"/> name.
        /// The <see cref="IFeatureInfo.Labels"/> are determined from attributes implementing <see cref="ILabelAttribute"/>, applied on <paramref name="featureType"/>.
        /// The <see cref="IFeatureInfo.Description"/> is determined from attribute implementing <see cref="IFeatureDescriptionAttribute"/> in first instance, then by <see cref="GetImplementationSpecificFeatureDescription"/>() method. The value may be <c>null</c>.
        /// </summary>
        /// <param name="featureType">Feature type.</param>
        /// <returns><see cref="IFeatureInfo"/> object.</returns>
        public IFeatureInfo GetFeatureInfo(Type featureType)
        {
            return new FeatureInfo(GetFeatureName(featureType), GetFeatureLabels(featureType), GetFeatureDescription(featureType));
        }

        /// <summary>
        /// Provides scenario labels for scenario represented by <paramref name="scenarioMethod"/> which are determined from attributes implementing <see cref="ILabelAttribute"/>, applied on the method.
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
        /// <para>
        /// For descriptors with <see cref="StepDescriptor.IsNameFormattingRequired"/> set, the step name is formatted with <see cref="NameParser"/>, otherwise the name is used as is from <see cref="StepDescriptor.RawName"/>.
        /// </para>
        /// See also: <seealso cref="StepTypeConfiguration"/>, <seealso cref="LightBddConfiguration"/>.
        /// </summary>
        /// <param name="stepDescriptor">Step descriptor.</param>
        /// <param name="previousStepTypeName">Step type name of previous step, or <c>null</c> if current step is first one.</param>
        /// <returns><see cref="IStepNameInfo"/> object.</returns>
        public IStepNameInfo GetStepName(StepDescriptor stepDescriptor, string previousStepTypeName)
        {
            var formattedStepName = stepDescriptor.IsNameFormattingRequired
                ? _nameParser.GetNameFormat(stepDescriptor.MethodInfo, stepDescriptor.RawName, stepDescriptor.Parameters)
                : stepDescriptor.RawName;
            return new StepNameInfo(
                _stepTypeProcessor.GetStepTypeName(stepDescriptor.PredefinedStepType, ref formattedStepName, previousStepTypeName),
                formattedStepName,
                stepDescriptor.Parameters.Select(p => NameParameterInfo.Unknown).ToArray());
        }

        /// <summary>
        /// Returns <see cref="IValueFormattingService"/> instance for provided <paramref name="parameterInfo"/>.
        /// The returned formatting service is aware of any <see cref="ParameterFormatterAttribute"/> instance(s) are applied on <paramref name="parameterInfo"/> and would use them to format value before any other configured formatters.
        /// If many instances of <see cref="ParameterFormatterAttribute"/> are present, they would be applied in <see cref="IOrderedAttribute.Order"/> order.
        /// </summary>
        /// <param name="parameterInfo"><see cref="ParameterInfo"/> object describing step or scenario method parameter.</param>
        /// <returns><see cref="IValueFormattingService"/> instance.</returns>
        public IValueFormattingService GetValueFormattingServiceFor(ParameterInfo parameterInfo)
        {
            var declaredFormatters = parameterInfo.GetCustomAttributes(typeof(ParameterFormatterAttribute), true)
               .OfType<ParameterFormatterAttribute>()
               .OrderBy(x => x.Order)
               .Cast<IConditionalValueFormatter>()
               .ToArray();

            return ValueFormattingService.WithFormattersOverride(declaredFormatters);
        }

        /// <summary>
        /// Returns a collection of <see cref="IStepDecorator"/> decorators that are applied on step described by <paramref name="stepDescriptor"/> parameter.
        /// The <see cref="IStepDecorator"/> are inferred from declaring type and method attributes that implements <see cref="IStepDecoratorAttribute"/> type.
        /// The returned collection would be sorted ascending based on <see cref="IOrderedAttribute.Order"/> from declaring type and then based on <see cref="IOrderedAttribute.Order"/> from method.
        /// </summary>
        /// <param name="stepDescriptor">Step descriptor.</param>
        /// <returns>Collection of decorators or empty collection if none are present.</returns>
        public IEnumerable<IStepDecorator> GetStepDecorators(StepDescriptor stepDescriptor)
        {
            if (stepDescriptor.MethodInfo == null)
                return Enumerable.Empty<IStepDecorator>();

            return ConcatAndOrderAttributes(
                ExtractAttributes<IStepDecoratorAttribute>(stepDescriptor.MethodInfo.DeclaringType),
                ExtractAttributes<IStepDecoratorAttribute>(stepDescriptor.MethodInfo));
        }

        /// <summary>
        /// Returns a collection of <see cref="IScenarioDecorator"/> decorators that are applied on scenario described by <paramref name="scenarioDescriptor"/> parameter.
        /// The <see cref="IScenarioDecorator"/> are inferred from declaring type and method attributes that implements <see cref="IScenarioDecoratorAttribute"/> type.
        /// The returned collection would be sorted ascending based on <see cref="IOrderedAttribute.Order"/> from declaring type and then based on <see cref="IOrderedAttribute.Order"/> from method.
        /// </summary>
        /// <param name="scenarioDescriptor">Scenario descriptor.</param>
        /// <returns>Collection of decorators or empty collection if none are present.</returns>
        public IEnumerable<IScenarioDecorator> GetScenarioDecorators(ScenarioDescriptor scenarioDescriptor)
        {
            return ConcatAndOrderAttributes(
                ExtractAttributes<IScenarioDecoratorAttribute>(scenarioDescriptor.MethodInfo.DeclaringType),
                ExtractAttributes<IScenarioDecoratorAttribute>(scenarioDescriptor.MethodInfo));
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
        /// Returns implementation specific scenario description or null if such is not provided.
        /// </summary>
        /// <param name="scenarioDescriptor">Scenario descriptor.</param>
        /// <returns>Scenario description or null.</returns>
        protected virtual string GetImplementationSpecificScenarioDescription(ScenarioDescriptor scenarioDescriptor) => null;

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
        /// Provides all attributes of type <typeparamref name="TAttribute"/> applied on <paramref name="type"/> or empty collection if none are applied.
        /// The attribute is searched in <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Type to analyze for specified attribute.</param>
        /// <typeparam name="TAttribute">Type of attribute to extract.</typeparam>
        /// <returns>All attributes or empty collection.</returns>
        protected static IEnumerable<TAttribute> ExtractAttributes<TAttribute>(Type type)
        {
            return type.GetTypeInfo().GetCustomAttributes(true).OfType<TAttribute>();
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
            return new NameInfo(NameFormatter.FormatName(featureType.Name), Array.Empty<INameParameterInfo>());
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

        /// <summary>
        /// Provides currently executed scenario details, that later can be used to build scenario metadata.
        /// </summary>
        /// <returns><see cref="ScenarioDescriptor"/> object.</returns>
        public abstract ScenarioDescriptor CaptureCurrentScenario();

        /// <summary>
        /// Provides <see cref="INameInfo"/> object containing information about scenario name represented by <paramref name="scenarioDescriptor"/>.
        /// </summary>
        /// <param name="scenarioDescriptor">Scenario descriptor.</param>
        /// <returns><see cref="INameInfo"/> object.</returns>
        public INameInfo GetScenarioName(ScenarioDescriptor scenarioDescriptor)
        {
            try
            {
                var formattedStepName = _nameParser.GetNameFormat(scenarioDescriptor.MethodInfo, scenarioDescriptor.MethodInfo.Name, scenarioDescriptor.Parameters);
                var arguments = scenarioDescriptor.Parameters.Select(p => new MethodArgument(p, GetValueFormattingServiceFor(p.ParameterInfo))).ToArray();
                return new NameInfo(
                    formattedStepName,
                    arguments.Select(p => p.FormatNameParameter()).ToArray());
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Unable to obtain scenario name for method {scenarioDescriptor.MethodInfo.Name}: {e.Message}", e);
            }
        }

        /// <summary>
        /// Provides scenario description which is determined from attribute implementing <see cref="IScenarioDescriptionAttribute"/> in first instance, then by <see cref="GetImplementationSpecificScenarioDescription"/>() method.
        /// Returns description or <c>null</c> if none is present.
        /// </summary>
        /// <param name="scenarioDescriptor">Scenario descriptor.</param>
        /// <returns>scenario description or <c>null</c>.</returns>
        public string GetScenarioDescription(ScenarioDescriptor scenarioDescriptor)
        {
            
            return ExtractAttributePropertyValue<IScenarioDescriptionAttribute>(scenarioDescriptor.MethodInfo, a => a.Description)
                   ?? GetImplementationSpecificScenarioDescription(scenarioDescriptor);
        }

        private IEnumerable<T> ConcatAndOrderAttributes<T>(params IEnumerable<T>[] sequences) where T : IOrderedAttribute
        {
            return sequences.SelectMany(sequence => sequence.OrderBy(orderable => orderable.Order));
        }
    }
}