using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace LightBDD.Framework.Notification.Implementation.IncrementalJson
{
    public interface IJsonItem
    {
        void WriteTo(StreamWriter writer);
    }

    internal class JsonProperty : IJsonItem
    {
        private readonly string _name;
        private readonly IJsonItem _value;

        public JsonProperty(string name, string value) : this(name, new JsonString(value))
        {
        }

        public JsonProperty(string name, long value) : this(name, new JsonLong(value))
        {
        }

        public JsonProperty(string name, IJsonItem value)
        {
            _name = name;
            _value = value;
        }

        public JsonProperty(string name, bool value) : this(name, new JsonBoolean(value))
        {
        }

        public JsonProperty(string name, double value) : this(name, new JsonDouble(value))
        {
        }

        public void WriteTo(StreamWriter writer)
        {
            writer.WriteText(_name);
            writer.Write(':');
            _value.WriteTo(writer);
        }
    }

    internal class JsonDouble : IJsonItem
    {
        private readonly double _value;

        public JsonDouble(double value)
        {
            _value = value;
        }

        public  void WriteTo(StreamWriter writer)
        {
             writer.Write(_value.ToString(CultureInfo.InvariantCulture));
        }
    }

    internal class JsonBoolean : IJsonItem
    {
        private readonly bool _value;

        public JsonBoolean(bool value)
        {
            _value = value;
        }

        public  void WriteTo(StreamWriter writer)
        {
             writer.Write(_value ? "true" : "false");
        }
    }

    internal class JsonLong : IJsonItem
    {
        private readonly long _value;

        public JsonLong(long value)
        {
            _value = value;
        }

        public  void WriteTo(StreamWriter writer)
        {
             writer.Write(_value.ToString(CultureInfo.InvariantCulture));
        }
    }

    internal class JsonString : IJsonItem
    {
        private readonly string _value;

        public JsonString(string value)
        {
            _value = value;
        }

        public void WriteTo(StreamWriter writer)
        {
            writer.WriteText(_value);
        }
    }

    internal class JsonArray : IJsonItem
    {
        private readonly IJsonItem[] _values;

        public JsonArray(IEnumerable<IJsonItem> values)
        {
            _values = values.ToArray();
        }
        public void WriteTo(StreamWriter writer)
        {
            writer.Write('[');
            for (var index = 0; index < _values.Length; index++)
            {
                if (index > 0)
                {
                    writer.Write(',');
                }
                _values[index].WriteTo(writer);
            }
            writer.Write(']');
        }
    }

    internal class JsonElement : IJsonItem
    {
        private readonly JsonProperty[] _properties;

        public JsonElement(params JsonProperty[] properties)
        {
            _properties = properties;
        }

        public void WriteTo(StreamWriter writer)
        {
            writer.Write('{');
            for (var index = 0; index < _properties.Length; index++)
            {
                if (index > 0)
                {
                    writer.Write(',');
                }
                _properties[index].WriteTo(writer);
            }
            writer.Write('}');
        }
    }
}