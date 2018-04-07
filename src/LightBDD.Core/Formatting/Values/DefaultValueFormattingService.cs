using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting.Values.Implementation;

namespace LightBDD.Core.Formatting.Values
{
    //TODO: rework
    public static class DefaultValueFormattingService
    {
        public static IValueFormattingService Instance { get; }=new ValueFormattingService(new ValueFormattingConfiguration(), new DefaultCultureInfoProvider());
    }
}