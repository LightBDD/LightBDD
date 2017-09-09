using System;
using System.Collections.Generic;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Test metadata provider interface allowing to provide feature, scenario and step metadata.
    /// </summary>
    public interface IMetadataProvider
    {
        /// <summary>
        /// Provides <see cref="IFeatureInfo"/> object containing information about feature represented by <paramref name="featureType"/>.
        /// </summary>
        /// <param name="featureType">Feature type.</param>
        /// <returns><see cref="IFeatureInfo"/> object.</returns>
        IFeatureInfo GetFeatureInfo(Type featureType);
        /// <summary>
        /// Provides currently executed scenario method.
        /// </summary>
        /// <returns><see cref="MethodBase"/> describing currently executed scenario method.</returns>
        /// <exception cref="InvalidOperationException">Thrown when called outside of scenario method.</exception>
        [Obsolete("Use " + nameof(CaptureCurrentScenario) + "() instead.")]
        MethodBase CaptureCurrentScenarioMethod();
        /// <summary>
        /// Provides currently executed scenario details, that later can be used to build scenario metadata.
        /// </summary>
        /// <returns><see cref="ScenarioDescriptor"/> object.</returns>
        ScenarioDescriptor CaptureCurrentScenario();
        /// <summary>
        /// Provides <see cref="INameInfo"/> object containing information about scenario name represented by <paramref name="scenarioMethod"/>.
        /// </summary>
        /// <param name="scenarioMethod">Scenario method.</param>
        /// <returns><see cref="INameInfo"/> object.</returns>
        [Obsolete("Use " + nameof(GetScenarioName) + "(ScenarioDescriptor descriptor)")]
        INameInfo GetScenarioName(MethodBase scenarioMethod);
        /// <summary>
        /// Provides <see cref="INameInfo"/> object containing information about scenario name represented by <paramref name="scenarioDescriptor"/>.
        /// </summary>
        /// <param name="scenarioDescriptor">Scenario descriptor.</param>
        /// <returns><see cref="INameInfo"/> object.</returns>
        INameInfo GetScenarioName(ScenarioDescriptor scenarioDescriptor);
        /// <summary>
        /// Provides scenario labels for scenario represented by <paramref name="scenarioMethod"/>.
        /// </summary>
        /// <param name="scenarioMethod">Scenario method.</param>
        /// <returns>Scenario labels.</returns>
        string[] GetScenarioLabels(MethodBase scenarioMethod);
        /// <summary>
        /// Provides scenario categories for scenario represented by <paramref name="scenarioMethod"/>.
        /// </summary>
        /// <param name="scenarioMethod">Scenario method.</param>
        /// <returns>Scenario categories.</returns>
        string[] GetScenarioCategories(MethodBase scenarioMethod);
        /// <summary>
        /// Provides <see cref="IStepNameInfo"/> object containing information about step name represented by <paramref name="stepDescriptor"/>.
        /// The <paramref name="previousStepTypeName"/> represents the step type name of previous step.
        /// <para>
        /// The <see cref="IStepNameInfo.StepTypeName"/> is determined from <see cref="StepDescriptor.PredefinedStepType"/> or parsed from <see cref="StepDescriptor.RawName"/> if former is <c>null</c>.
        /// When determined step type is the same as <paramref name="previousStepTypeName"/>, it is being replaced with <see cref="StepTypeConfiguration.RepeatedStepReplacement"/>.
        /// </para>
        /// See also: <seealso cref="StepTypeConfiguration"/>, <seealso cref="LightBddConfiguration"/>.
        /// </summary>
        /// <param name="stepDescriptor">Step descriptor.</param>
        /// <param name="previousStepTypeName">Step type name of previous step, or <c>null</c> if current step is first one.</param>
        /// <returns><see cref="IStepNameInfo"/> object.</returns>
        IStepNameInfo GetStepName(StepDescriptor stepDescriptor, string previousStepTypeName);
        /// <summary>
        /// Provides step parameter formatter function for provided <paramref name="parameterInfo"/>.
        /// </summary>
        /// <param name="parameterInfo"><see cref="ParameterInfo"/> object describing step or scenario method parameter.</param>
        /// <returns>Formatter function.</returns>
        [Obsolete("Use " + nameof(GetParameterFormatter) + "() instead.")]
        Func<object, string> GetStepParameterFormatter(ParameterInfo parameterInfo);
        /// <summary>
        /// Provides parameter formatter function for provided <paramref name="parameterInfo"/>.
        /// </summary>
        /// <param name="parameterInfo"><see cref="ParameterInfo"/> object describing step or scenario method parameter.</param>
        /// <returns>Formatter function.</returns>
        Func<object, string> GetParameterFormatter(ParameterInfo parameterInfo);
        /// <summary>
        /// Returns a collection of <see cref="IStepExtension"/> extensions that are applied on step described by <paramref name="stepDescriptor"/> parameter.
        /// The <see cref="IStepExtension"/> are inferred from method attributes that implements <see cref="IStepExtensionAttribute"/> type.
        /// The returned collection would be sorted ascending based on <see cref="IStepExtensionAttribute.Order"/> property.
        /// </summary>
        /// <param name="stepDescriptor">Step descriptor.</param>
        /// <returns>Collection of extensions or empty collection if none are present.</returns>
        IEnumerable<IStepExtension> GetStepExecutionExtensions(StepDescriptor stepDescriptor);
        /// <summary>
        /// Returns a collection of <see cref="IScenarioExtension"/> extensions that are applied on scenario described by <paramref name="scenarioDescriptor"/> parameter.
        /// The <see cref="IScenarioExtension"/> are inferred from method attributes that implements <see cref="IScenarioExtensionAttribute"/> type.
        /// The returned collection would be sorted ascending based on <see cref="IScenarioExtensionAttribute.Order"/> property.
        /// </summary>
        /// <param name="scenarioDescriptor">Scenario descriptor.</param>
        /// <returns>Collection of extensions or empty collection if none are present.</returns>
        IEnumerable<IScenarioExtension> GetScenarioExecutionExtensions(ScenarioDescriptor scenarioDescriptor);
    }
}