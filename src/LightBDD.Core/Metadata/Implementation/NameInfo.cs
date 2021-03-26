using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Formatting.NameDecorators;

namespace LightBDD.Core.Metadata.Implementation
{
    internal class NameInfo : INameInfo
    {
        private readonly INameParameterInfo[] _parameters;
        public string NameFormat { get; }

        public IEnumerable<INameParameterInfo> Parameters => _parameters;

        public NameInfo(string nameFormat, IEnumerable<INameParameterInfo> parameters)
        {
            NameFormat = nameFormat;
            _parameters = parameters.ToArray();
        }

        public void UpdateParameters(IEnumerable<INameParameterInfo> parameters)
        {
            foreach (var p in parameters)
            {
                for (var i = 0; i < _parameters.Length; i++)
                {
                    if (_parameters[i].Name == p.Name)
                    {
                        _parameters[i] = p;
                        break;
                    }
                }
            }
        }

        public override string ToString()
        {
            return Format(StepNameDecorators.Default);
        }

        public string Format(INameDecorator decorator)
        {
            return !Parameters.Any()
                ? decorator.DecorateNameFormat(NameFormat)
                : string.Format(decorator.DecorateNameFormat(NameFormat), Parameters.Select(p => (object)decorator.DecorateParameterValue(p)).ToArray());
        }
    }
}