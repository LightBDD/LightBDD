#nullable enable
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
using LightBDD.Core.Extensibility.Execution.Implementation;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Metadata provider offering core implementation for providing feature, scenario and step metadata.
    /// </summary>
    public class CoreMetadataProvider
    {
        private readonly NameParser _nameParser;
        private readonly StepTypeProcessor _stepTypeProcessor;
        private readonly MetadataConfiguration _metadataConfiguration;
        private readonly GlobalDecoratorsProvider _decoratorsProvider;
        private readonly ValueFormattingService _valueFormattingService;
        private readonly INameFormatter _nameFormatter;

        public CoreMetadataProvider(ValueFormattingService valueFormattingService, INameFormatter nameFormatter, StepTypeConfiguration stepTypeConfiguration, MetadataConfiguration metadataMetadataConfiguration, GlobalDecoratorsProvider decoratorsProvider)
        {
            _metadataConfiguration = metadataMetadataConfiguration;
            _decoratorsProvider = decoratorsProvider;
            _valueFormattingService = valueFormattingService;
            _nameFormatter = nameFormatter;
            _nameParser = new NameParser(_nameFormatter);
            _stepTypeProcessor = new StepTypeProcessor(_nameFormatter, stepTypeConfiguration);
        }

        /// <summary>
        /// Provides <see cref="IFeatureInfo"/> object containing information about feature represented by <paramref name="featureType"/>.
        /// 
        /// The <see cref="IFeatureInfo.Name"/> is determined from the <paramref name="featureType"/> name.
        /// The <see cref="IFeatureInfo.Labels"/> are determined from attributes implementing <see cref="ILabelAttribute"/>, applied on <paramref name="featureType"/>.
        /// The <see cref="IFeatureInfo.Description"/> is determined from attribute implementing <see cref="IFeatureDescriptionAttribute"/>. The value may be <c>null</c>.
        /// </summary>
        /// <param name="featureType">Feature type.</param>
        /// <returns><see cref="IFeatureInfo"/> object.</returns>
        public IFeatureInfo GetFeatureInfo(Type featureType)
        {
            return new FeatureInfo(GetFeatureName(featureType), GetFeatureLabels(featureType), GetFeatureDescription(featureType), featureType);
        }

        /// <summary>
        /// Provides scenario labels for scenario represented by <paramref name="scenarioMethod"/> which are determined from attributes implementing <see cref="ILabelAttribute"/>, applied on the method.
        /// </summary>
        /// <param name="scenarioMethod">Scenario method.</param>
        /// <returns>Scenario labels.</returns>
        public string[] GetScenarioLabels(MethodBase scenarioMethod)
        {
            return scenarioMethod.ExtractAttributePropertyValues<ILabelAttribute>(a => a.Label).OrderBy(l => l).ToArray();
        }

        /// <summary>
        /// Provides scenario categories for scenario represented by <paramref name="scenarioMethod"/>.
        /// The categories are determined from attributes implementing <see cref="IScenarioCategoryAttribute"/>, applied on <paramref name="scenarioMethod"/> and type declaring the method.
        /// 
        /// The categories specified on base classes will also be retrieved.
        /// </summary>
        /// <param name="scenarioMethod">Scenario method.</param>
        /// <returns>Scenario categories.</returns>
        public string[] GetScenarioCategories(MethodBase scenarioMethod)
        {
            return scenarioMethod.ExtractAttributePropertyValues<IScenarioCategoryAttribute>(a => a.Category)
                .Concat(scenarioMethod.DeclaringType.ExtractAttributePropertyValues<IScenarioCategoryAttribute>(a => a.Category))
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
        public IStepNameInfo GetStepName(StepDescriptor stepDescriptor, string? previousStepTypeName)
        {
            var formattedStepName = stepDescriptor.IsNameFormattingRequired
                ? _nameParser.GetNameFormat(stepDescriptor.MethodInfo, stepDescriptor.RawName, stepDescriptor.Parameters)
                : stepDescriptor.RawName;
            return new StepNameInfo(
                _stepTypeProcessor.GetStepTypeName(stepDescriptor.PredefinedStepType, ref formattedStepName, previousStepTypeName),
                formattedStepName,
                stepDescriptor.Parameters.Select(_ => NameParameterInfo.Unknown).ToArray());
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

            return _valueFormattingService.WithFormattersOverride(declaredFormatters);
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
            var globalDecorators = _decoratorsProvider.ProvideStepDecorators();
            if (stepDescriptor.MethodInfo == null)
                return globalDecorators;

            return globalDecorators.Concat(
                ConcatAndOrderAttributes(
                    stepDescriptor.MethodInfo.DeclaringType.ExtractAttributes<IStepDecoratorAttribute>(),
                    stepDescriptor.MethodInfo.ExtractAttributes<IStepDecoratorAttribute>()));
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
            return _decoratorsProvider.ProvideScenarioDecorators().Concat(
                ConcatAndOrderAttributes(
                    scenarioDescriptor.MethodInfo.DeclaringType.ExtractAttributes<IScenarioDecoratorAttribute>(),
                    scenarioDescriptor.MethodInfo.ExtractAttributes<IScenarioDecoratorAttribute>()));
        }

        /// <summary>
        /// Provides currently performed test run info.
        /// </summary>
        /// <returns><see cref="ITestRunInfo"/> object.</returns>
        public ITestRunInfo GetTestRunInfo(Assembly testAssembly)
        {
            //TODO: add assembly details
            return new TestRunInfo(TestSuite.Create(testAssembly), _metadataConfiguration.EngineAssemblies);
        }

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
        /// Provides labels from  attributes implementing <see cref="ILabelAttribute"/>, applied on <paramref name="featureType"/>, or empty array if none are present.
        /// </summary>
        /// <param name="featureType">Feature type.</param>
        /// <returns>Array of labels or empty array if none are present.</returns>
        private string[] GetFeatureLabels(Type featureType)
        {
            return featureType.GetTypeInfo().ExtractAttributePropertyValues<ILabelAttribute>(a => a.Label)
                .OrderBy(l => l)
                .ToArray();
        }

        /// <summary>
        /// Provides feature name which is determined from name of <paramref name="featureType"/>.
        /// </summary>
        /// <param name="featureType">Feature type.</param>
        /// <returns>Feature name.</returns>
        private INameInfo GetFeatureName(Type featureType)
        {
            return new NameInfo(_nameFormatter.FormatName(featureType.Name), Array.Empty<INameParameterInfo>());
        }

        /// <summary>
        /// Provides feature description which is determined from attribute implementing <see cref="IFeatureDescriptionAttribute"/>.
        /// Returns description or <c>null</c> if none is present.
        /// </summary>
        /// <param name="featureType">Feature type.</param>
        /// <returns>Feature description or <c>null</c>.</returns>
        private string GetFeatureDescription(Type featureType)
        {
            return featureType.ExtractAttributePropertyValue<IFeatureDescriptionAttribute>(a => a.Description);
        }

        private static IEnumerable<T> ConcatAndOrderAttributes<T>(params IEnumerable<T>[] sequences) where T : IOrderedAttribute
        {
            return sequences.SelectMany(sequence => sequence.OrderBy(orderable => orderable.Order));
        }
    }
}