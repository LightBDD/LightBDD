using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace LightBDD.Core.Formatting.Diagnostics
{
    /// <summary>
    /// Class designed to dump object details into human readable diagnostic string.
    /// </summary>
    public static class ObjectFormatter
    {
        /// <summary>
        /// Formats collection of objects using <seealso cref="Dump{T}(T,int)"/>, separating them with new line.
        /// </summary>
        /// <typeparam name="T">Member type.</typeparam>
        /// <param name="items">Collection of items to format</param>
        /// <param name="maxDepth">Max depth for each item members to include in result string</param>
        /// <returns>Text representation of provided items.</returns>
        public static string DumpMany<T>(IEnumerable<T> items, int maxDepth = 3)
        {
            if (items == null) return "null";

            var sb = new StringBuilder();
            var first = true;
            foreach (var obj in items)
            {
                if (first) first = false;
                else sb.AppendLine();
                sb.Append((obj?.GetType() ?? typeof(T)).Name).Append(": ").Dump(obj, maxDepth);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns text representation of the object, including it's fields and properties.<br/>
        /// The method returns the type of provided item, followed by the list of fields' and properties' names and values.<br/>
        /// Each type member pair is space separated.<br/>
        /// The string type is wrapped with "", null values are represented as null, primitives are formatted as is using <seealso cref="CultureInfo.InvariantCulture"/>, the arrays are wrapped with [] and complex objects are represented by {}.
        /// When <paramref name="maxDepth"/> is reached, the complex objects are represented as {...}.
        /// </summary>
        /// <typeparam name="T">Type of object to format.</typeparam>
        /// <param name="item">Item to format.</param>
        /// <param name="maxDepth">Max depth of item's members to include in result string</param>
        /// <returns>Text representation of provided item.</returns>
        public static string Dump<T>(T item, int maxDepth = 5)
        {
            return new StringBuilder()
                .Append((item?.GetType() ?? typeof(T)).Name)
                .Append(": ")
                .Dump(item, maxDepth)
                .ToString();
        }

        private static StringBuilder Dump(this StringBuilder builder, object obj, int level)
        {
            level--;
            if (obj == null)
                return builder.Append("null");

            var type = obj.GetType();

            if (type == typeof(string))
                return builder.Append('\"').Append(obj).Append('\"');
            if (type.IsPrimitive)
                return builder.AppendFormat(CultureInfo.InvariantCulture, "{0}", obj);
            if (type.IsEnum)
                return builder.Append(Enum.GetName(type, obj));
            var basic = TryFormatBasic(obj);
            if (basic != null)
                return builder.Append('\"').Append(basic).Append('\"');

            if (level == 0)
                return builder.Append("{...}");

            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                builder.Append("[ ");
                foreach (var e in (IEnumerable)obj)
                    builder.Dump(e, level).Append(" ");
                return builder.Append("]");
            }

            builder.Append("{ ");
            foreach (var member in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                builder.Append(member.Name).Append("=").Dump(member.GetValue(obj), level).Append(" ");

            foreach (var member in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                builder.Append(member.Name).Append("=").DumpProperty(member, obj, level).Append(" ");

            return builder.Append("}");
        }

        private static string TryFormatBasic(object value)
        {
            return value switch
            {
                DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("O", CultureInfo.InvariantCulture),
                DateTime dateTime => dateTime.ToString("O", CultureInfo.InvariantCulture),
                IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                _ => null
            };
        }

        private static StringBuilder DumpProperty(this StringBuilder builder, PropertyInfo member, object obj, int level)
        {
            try
            {
                return builder.Dump(member.GetValue(obj), level);
            }
            catch (TargetInvocationException ex)
            {
                var inner = ex.InnerException;
                return builder.Append("!").Append(inner.GetType().Name).Append(":\"").Append(inner.Message).Append("\"");
            }
        }
    }
}