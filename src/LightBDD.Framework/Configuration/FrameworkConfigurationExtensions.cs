using LightBDD.Core.Configuration;
using LightBDD.Framework.Commenting.Configuration;
using LightBDD.Framework.Formatting.Configuration;
using LightBDD.Framework.Formatting.Values;

namespace LightBDD.Framework.Configuration
{
    public static class FrameworkConfigurationExtensions
    {
        public static LightBddConfiguration WithFrameworkDefaults(this LightBddConfiguration configuration)
        {
            configuration
                .ExecutionExtensionsConfiguration()
                .EnableStepCommenting();

            configuration
                .ValueFormattingConfiguration()
                .Register(new DictionaryFormatter())
                .Register(new CollectionFormatter());

            return configuration;
        }
    }
}
