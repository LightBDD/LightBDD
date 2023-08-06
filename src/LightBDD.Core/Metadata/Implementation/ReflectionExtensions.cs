using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LightBDD.Core.Metadata.Implementation;

internal static class ReflectionExtensions
{
    /// <summary>
    /// Provides value of attribute of type <typeparamref name="TAttribute"/> applied on <paramref name="member"/> or default if attribute is not applied.
    /// The attribute is searched in <paramref name="member"/> and it's ancestors.
    /// </summary>
    /// <param name="member">Member to analyze for specified attribute.</param>
    /// <param name="valueExtractor">Attribute value extraction method.</param>
    /// <typeparam name="TAttribute">Type of attribute to extract.</typeparam>
    /// <returns>Attribute value or default.</returns>
    /// <exception cref="InvalidOperationException">Throws when attribute is applied more than once.</exception>
    public static string ExtractAttributePropertyValue<TAttribute>(this MemberInfo member, Func<TAttribute, string> valueExtractor)
    {
        return ExtractAttributePropertyValues(member, valueExtractor).SingleOrDefault();
    }

    /// <summary>
    /// Provides values of all attributes of type <typeparamref name="TAttribute"/> applied on <paramref name="member"/> or empty collection if none are applied.
    /// The attribute is searched in <paramref name="member"/> and it's ancestors.
    /// </summary>
    /// <param name="member">Member to analyze for specified attribute.</param>
    /// <param name="valueExtractor">Attribute value extraction method.</param>
    /// <typeparam name="TAttribute">Type of attribute to extract.</typeparam>
    /// <returns>Values of all attributes or empty collection.</returns>
    public static IEnumerable<string> ExtractAttributePropertyValues<TAttribute>(this MemberInfo member, Func<TAttribute, string> valueExtractor)
    {
        return ExtractAttributes<TAttribute>(member).Select(valueExtractor);
    }

    /// <summary>
    /// Provides all attributes of type <typeparamref name="TAttribute"/> applied on <paramref name="member"/> or empty collection if none are applied.
    /// The attribute is searched in <paramref name="member"/> and it's ancestors.
    /// </summary>
    /// <param name="member">Member to analyze for specified attribute.</param>
    /// <typeparam name="TAttribute">Type of attribute to extract.</typeparam>
    /// <returns>All attributes or empty collection.</returns>
    public static IEnumerable<TAttribute> ExtractAttributes<TAttribute>(this MemberInfo member)
    {
        return member.GetCustomAttributes(true).OfType<TAttribute>();
    }

    /// <summary>
    /// Provides all attributes of type <typeparamref name="TAttribute"/> applied on <paramref name="type"/> or empty collection if none are applied.
    /// The attribute is searched in <paramref name="type"/>.
    /// </summary>
    /// <param name="type">Type to analyze for specified attribute.</param>
    /// <typeparam name="TAttribute">Type of attribute to extract.</typeparam>
    /// <returns>All attributes or empty collection.</returns>
    public static IEnumerable<TAttribute> ExtractAttributes<TAttribute>(this Type type)
    {
        return type.GetCustomAttributes(true).OfType<TAttribute>();
    }
}