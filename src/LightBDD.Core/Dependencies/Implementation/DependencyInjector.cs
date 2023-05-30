using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LightBDD.Core.Dependencies.Implementation
{
    internal class DependencyInjector
    {
        private readonly ConcurrentDictionary<Type, InjectableProperties> _properties = new();
        public static DependencyInjector Instance { get; } = new();
        private DependencyInjector() { }

        public void Inject(object fixture, IDependencyResolver resolver)
        {
            if (fixture == null)
                return;
            var fixtureType = fixture.GetType();

            var injectable = _properties.GetOrAdd(fixtureType, t => new InjectableProperties(t));
            
            if (injectable.Errors != null)
                throw new InvalidOperationException($"Unable to inject dependencies on '{fixtureType.Name}': {injectable.Errors}");

            foreach (var property in injectable.Properties)
            {
                try
                {
                    property.SetValue(fixture, resolver.Resolve(property.PropertyType));
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Unable to inject dependency of type '{property.PropertyType.Name}' to '{fixtureType.Name}.{property.Name}' property: {ex.Message}",
                        ex);
                }
            }
        }

        class InjectableProperties
        {
            private List<PropertyInfo> _properties = null;
            public string Errors { get; private set; } = null;
            public IReadOnlyList<PropertyInfo> Properties => (IReadOnlyList<PropertyInfo>)_properties ?? Array.Empty<PropertyInfo>();

            public InjectableProperties(Type type)
            {
                while (type != null)
                {
                    foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Where(p => p.GetCustomAttributes().Any(a => a is IFixtureDependencyAttribute)))
                    {
                        var setter = propertyInfo.GetSetMethod();
                        if (setter == null)
                            AddError($"'{type.Name}.{propertyInfo.Name}' property has to have a public setter");
                        else if (setter.IsStatic)
                            AddError($"'{type.Name}.{propertyInfo.Name}' property cannot be static");
                        else
                            AddProperty(propertyInfo);
                    }
                    type = type.BaseType;
                }
                _properties?.TrimExcess();
            }

            private void AddProperty(PropertyInfo property)
            {
                (_properties ??= new()).Add(property);
            }

            private void AddError(string error)
            {
                Errors = Errors == null ? error : $"{Errors}{Environment.NewLine}{error}";
            }
        }
    }
}
