namespace LightBDD.Core.Metadata.Implementation
{
    internal class NameInfo : INameInfo
    {
        private readonly string _name;

        public NameInfo(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}