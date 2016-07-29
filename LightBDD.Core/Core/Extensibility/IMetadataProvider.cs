using System;
using System.Reflection;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Extensibility
{
    public interface IMetadataProvider
    {
        IFeatureInfo GetFeatureInfo(Type featureType);
        MethodBase CaptureCurrentScenarioMethod();
        INameInfo GetScenarioName(MethodBase scenarioMethod);
        string[] GetScenarioLabels(MethodBase scenarioMethod);
        string[] GetScenarioCategories(MethodBase scenarioMethod);
        IStepNameInfo GetStepName(StepDescriptor stepDescriptor);
        Func<object, string> GetStepParameterFormatter(ParameterInfo parameterInfo);
    }
}