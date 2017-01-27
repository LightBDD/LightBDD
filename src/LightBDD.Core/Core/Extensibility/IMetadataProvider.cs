using System;
using System.Reflection;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Test metadata provider allowing to provide feature, scenario and step metadata.
    /// </summary>
    public interface IMetadataProvider
    {
        /// <summary>
        /// Provides feature information details for specified feature type.
        /// </summary>
        /// <param name="featureType">Feature type.</param>
        /// <returns>Feature information details.</returns>
        IFeatureInfo GetFeatureInfo(Type featureType);
        /// <summary>
        /// Provides currently executed scenario method.
        /// </summary>
        /// <returns>MethodBase describing currently executed scenario method.</returns>
        /// <exception cref="InvalidOperationException">Thrown when called outside of scenario method.</exception>
        MethodBase CaptureCurrentScenarioMethod();
        INameInfo GetScenarioName(MethodBase scenarioMethod);
        string[] GetScenarioLabels(MethodBase scenarioMethod);
        string[] GetScenarioCategories(MethodBase scenarioMethod);
        IStepNameInfo GetStepName(StepDescriptor stepDescriptor, string lastStepTypeName);
        Func<object, string> GetStepParameterFormatter(ParameterInfo parameterInfo);
    }
}