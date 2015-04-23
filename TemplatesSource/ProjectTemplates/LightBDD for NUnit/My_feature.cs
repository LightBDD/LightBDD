using LightBDD;
using NUnit.Framework;

namespace $safeprojectname$
{
    [FeatureDescription(
@"In order to 
As a
I want to ")]
    [TestFixture]
    public partial class My_feature
    {
        [Test]
        public void Template_scenario()
        {
            Runner.RunScenario(
                Given_template_method,
                When_template_method,
                Then_template_method);
        }
    }
}