using LightBDD.Commenting.Configuration;
using LightBDD.Core.Configuration;
using LightBDD.Example.AcceptanceTests.XUnit2;

[assembly: ConfiguredLightBddScope]
namespace LightBDD.Example.AcceptanceTests.XUnit2
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