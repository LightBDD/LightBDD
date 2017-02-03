using LightBDD.Core.Configuration;
using LightBDD.Example.AcceptanceTests.NUnit3;
using LightBDD.Framework.Commenting.Configuration;
using LightBDD.NUnit3;

[assembly: ConfiguredLightBddScope]
namespace LightBDD.Example.AcceptanceTests.NUnit3
{
    class ConfiguredLightBddScope : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration
                .ExecutionExtensionsConfiguration()
                .EnableStepCommenting();
        }
    }
}