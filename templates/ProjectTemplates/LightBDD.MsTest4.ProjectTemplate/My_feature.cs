using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.MsTest4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace $safeprojectname$
{
    [Label("FEAT-1")]
    [FeatureDescription(
@"In order to 
As a
I want to ")]
    [TestClass]
    public partial class My_feature
    {
        [Label("SCENARIO-1")]
        [Scenario]
        public void Template_basic_scenario()
        {
            Runner.RunScenario(
                Given_template_method,
                When_template_method,
                Then_template_method);
        }

        [Label("SCENARIO-1")]
        [Scenario]
        public void Template_extended_scenario()
        {
            Runner.RunScenario(
                _ => Given_template_method(),
                _ => When_template_method(),
                _ => Then_template_method());
        }
    }
}