using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// IScenarioRunner interface allowing to programatically construct scenario to execute.
    /// The interface is dedicated for projects extending LightBDD with user friendly API for running scenarios - it should not be used directly by regular LightBDD users.
    /// </summary>
    public interface IScenarioRunner
    {
        /// <summary>
        /// Configures steps to be executed with scenario.
        /// </summary>
        /// <param name="steps">Steps to execute.</param>
        /// <returns>Self.</returns>
        IScenarioRunner WithSteps(IEnumerable<StepDescriptor> steps);
        
        IScenarioRunner WithCapturedScenarioDetails();
        IScenarioRunner WithLabels(string[] labels);
        IScenarioRunner WithCategories(string[] categories);
        IScenarioRunner WithName(string name);
        IScenarioRunner WithContext(Func<object> contextProvider);
        Task RunAsynchronously();
        void RunSynchronously();
    }
}