using LightBDD.Core.Configuration;
using LightBDD.Example.AcceptanceTests.XUnit2;
using LightBDD.Framework.Commenting.Configuration;
using LightBDD.XUnit2;

[assembly: ConfiguredLightBddScope]
namespace LightBDD.Example.AcceptanceTests.XUnit2
{
    public class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration
                .ExecutionExtensionsConfiguration()
                .EnableStepCommenting();
        }
    }
}