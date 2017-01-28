using System;
using System.Reflection;
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
        MethodBase CaptureCurrentScenarioMethod();
        /// <summary>
        /// Provides <see cref="INameInfo"/> object containing information about scenario name represented by <paramref name="scenarioMethod"/>.
        /// </summary>
        /// <param name="scenarioMethod">Scenario method.</param>
        /// <returns><see cref="INameInfo"/> object.</returns>
        INameInfo GetScenarioName(MethodBase scenarioMethod);
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
        Func<object, string> GetStepParameterFormatter(ParameterInfo parameterInfo);
    }
}