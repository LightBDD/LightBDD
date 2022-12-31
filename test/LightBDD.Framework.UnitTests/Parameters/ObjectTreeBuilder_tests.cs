#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Parameters;
using LightBDD.Framework.Parameters.ObjectTrees;
using LightBDD.Framework.Parameters.ObjectTrees.Mappers;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Framework.UnitTests.Parameters
{
    [TestFixture, FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class ObjectTreeBuilder_tests
    {
        [Test]
        public void It_should_create_root_node()
        {
            var builder = new ObjectTreeBuilder(new());
            var root = builder.Build(null);

            AssertValueNode(root, "$", null);
        }

        [Test]
        [TestCaseSource(nameof(GetValueInstances))]
        public void It_should_create_value_nodes_for_nulls_primitives_and_registered_types(object input)
        {
            var builder = new ObjectTreeBuilder(new());
            var root = builder.Build(input);

            AssertValueNode(root, "$", input);
        }

        [Test]
        [TestCaseSource(nameof(GetFlatCollectionInstances))]
        public void It_should_create_array_node_for_collections(IEnumerable input)
        {
            var builder = new ObjectTreeBuilder(new());
            var root = builder.Build(input);
            var expectations = input.Cast<object>().ToArray();

            root.Kind.ShouldBe(ObjectTreeNodeKind.Array);
            root.Path.ShouldBe("$");

            var items = root.AsArray().Items;
            for (var i = 0; i < items.Count; ++i)
                AssertValueNode(items[i], $"$[{i}]", expectations[i]);
        }

        [Test]
        public void It_should_create_object_node_for_poco_type()
        {
            var builder = new ObjectTreeBuilder(new());
            var input = new
            {
                Name = "text",
                Number = 34,
                Flag = DateTimeKind.Utc,
                Value = (object?)null
            };
            var root = builder.Build(input);

            root.Kind.ShouldBe(ObjectTreeNodeKind.Object);
            var properties = root.AsObject().Properties;
            AssertValueNode(properties["Name"], "$.Name", "text");
            AssertValueNode(properties["Number"], "$.Number", 34);
            AssertValueNode(properties["Flag"], "$.Flag", DateTimeKind.Utc);
            AssertValueNode(properties["Value"], "$.Value", null);
        }

        [Test]
        public void It_should_only_map_nonhidden_readable_public_instance_members_of_poco_type()
        {
            var root = new ObjectTreeBuilder(new()).Build(new Poco());
            var properties = root.AsObject().Properties;
            properties.Count.ShouldBe(3);
            AssertValueNode(properties["Field"], "$.Field", "field");
            AssertValueNode(properties["Property"], "$.Property", "prop");
            AssertValueNode(properties["Base"], "$.Base", 'B');
        }

        [Test]
        public void It_should_map_expando_objects_as_object()
        {
            dynamic expando = new ExpandoObject();
            expando.Name = "Bob";
            expando.Surname = "Johnson";

            var root = new ObjectTreeBuilder(new()).Build((object)expando);
            root.Kind.ShouldBe(ObjectTreeNodeKind.Object);
            var properties = root.AsObject().Properties;
            properties.Count.ShouldBe(2);
            AssertValueNode(properties["Name"], "$.Name", "Bob");
            AssertValueNode(properties["Surname"], "$.Surname", "Johnson");
        }

        [Test]
        public void It_should_map_nonstandard_property_names_to_nodes_with_bracket_notation()
        {
            IDictionary<string, object?> expando = new ExpandoObject();
            expando["Name"] = "Bob";
            expando["Last Name"] = "Johnson";

            var root = new ObjectTreeBuilder(new()).Build(expando);
            root.Kind.ShouldBe(ObjectTreeNodeKind.Object);
            var properties = root.AsObject().Properties;
            properties.Count.ShouldBe(2);
            AssertValueNode(properties["Name"], "$.Name", "Bob");
            AssertValueNode(properties["Last Name"], "$['Last Name']", "Johnson");
        }

        [Test]
        public void It_should_detect_and_map_circular_references()
        {
            var p1 = new Parent { Name = "P1" };
            p1.Children.Add(new Child { Name = "C1", Parent = p1 });
            var p2 = new Parent { Name = "P2" };
            p2.Children.Add(new Child { Name = "C2", Parent = p2 });
            var input = new[] { p1, p2 };

            var root = new ObjectTreeBuilder(new()).Build(input);
            var nodes = root.EnumerateAll().ToDictionary(x => x.Path);
            nodes["$[0].Children[0].Parent"].ToString().ShouldBe("<ref: $[0]>");
            nodes["$[1].Children[0].Parent"].ToString().ShouldBe("<ref: $[1]>");
        }

        [Test]
        public void It_should_map_json_structure_from_JsonDocument()
        {
            var json = @"{
    ""name"":""John"",
    ""surname"":""Smith"",
    ""items"":[1,2,3],
    ""inner"":{""label"":""some text""}
}";
            var doc = JsonDocument.Parse(json);
            var root = new ObjectTreeBuilder(new()).Build(doc);
            var nodes = root.EnumerateAll().ToDictionary(x => x.Path);

            nodes["$.RootElement"].ShouldBeOfType<ObjectTreeObject>();
            nodes["$.RootElement.name"].ShouldBeOfType<ObjectTreeValue>().Value.ShouldBe("John");
            nodes["$.RootElement.surname"].ShouldBeOfType<ObjectTreeValue>().Value.ShouldBe("Smith");
            nodes["$.RootElement.items"].ShouldBeOfType<ObjectTreeArray>();
            nodes["$.RootElement.items[0]"].ShouldBeOfType<ObjectTreeValue>().Value.ShouldBe(1);
            nodes["$.RootElement.items[1]"].ShouldBeOfType<ObjectTreeValue>().Value.ShouldBe(2);
            nodes["$.RootElement.items[2]"].ShouldBeOfType<ObjectTreeValue>().Value.ShouldBe(3);
            nodes["$.RootElement.inner"].ShouldBeOfType<ObjectTreeObject>();
            nodes["$.RootElement.inner.label"].ShouldBeOfType<ObjectTreeValue>().Value.ShouldBe("some text");
        }

        [Test]
        public void It_should_map_json_numbers_to_suitable_types()
        {
            var input = new
            {
                i32 = int.MaxValue,
                ni32 = int.MinValue,
                i64 = long.MaxValue,
                ni64 = long.MinValue,
                Double = double.MaxValue,
                nDouble = double.MinValue
            };
            var json = JsonSerializer.Serialize(input);
            TestContext.WriteLine(json);
            var doc = JsonDocument.Parse(json);
            var root = new ObjectTreeBuilder(new()).Build(doc.RootElement);
            var nodes = root.EnumerateAll().ToDictionary(x => x.Path);

            nodes["$.i32"].ShouldBeOfType<ObjectTreeValue>().Value.ShouldBeOfType<int>().ShouldBe(int.MaxValue);
            nodes["$.ni32"].ShouldBeOfType<ObjectTreeValue>().Value.ShouldBeOfType<int>().ShouldBe(int.MinValue);
            nodes["$.i64"].ShouldBeOfType<ObjectTreeValue>().Value.ShouldBeOfType<long>().ShouldBe(long.MaxValue);
            nodes["$.ni64"].ShouldBeOfType<ObjectTreeValue>().Value.ShouldBeOfType<long>().ShouldBe(long.MinValue);
            nodes["$.Double"].ShouldBeOfType<ObjectTreeValue>().Value.ShouldBeOfType<double>().ShouldBe(double.MaxValue);
            nodes["$.nDouble"].ShouldBeOfType<ObjectTreeValue>().Value.ShouldBeOfType<double>().ShouldBe(double.MinValue);
        }

        [Test]
        public void It_should_map_Expect_expressions_as_values()
        {
            var input = new
            {
                Name = Expect.To.Not.BeEmpty<string>(),
                Surname = Expect.To.Not.BeEmpty<string>(),
                Items = new[] { Expect.To.BeGreaterOrEqual(0).And(x => x.BeLessOrEqual(5)) }
            };
            var root = new ObjectTreeBuilder(new()).Build(input);
            var nodes = root.EnumerateAll().ToDictionary(x => x.Path);
            nodes["$.Name"].AsValue().Value?.ToString().ShouldBe("not empty");
            nodes["$.Surname"].AsValue().Value?.ToString().ShouldBe("not empty");
            nodes["$.Items[0]"].AsValue().Value?.ToString().ShouldBe("(greater or equal '0' and less or equal '5')");
        }

        [Test]
        public void It_should_allow_registration_of_open_generics_as_value_nodes()
        {
            var input = new
            {
                A = Mock.Of<ISomething<string>>(),
                B = new Something<DateTime>()
            };

            var options = new ObjectTreeBuilderOptions { ValueTypes = { typeof(ISomething<>), typeof(Something<>) } };
            var root = new ObjectTreeBuilder(options).Build(input);
            var nodes = root.EnumerateAll().ToDictionary(x => x.Path);
            nodes["$.A"].ShouldBeOfType<ObjectTreeValue>();
            nodes["$.B"].ShouldBeOfType<ObjectTreeValue>();
        }

        [Test]
        public void It_should_sort_unordered_collections_if_underlying_type_is_sortable()
        {
            var set = Enumerable.Range(0, 10).Select(_ => Guid.NewGuid()).ToHashSet();
            var root = new ObjectTreeBuilder(new ObjectTreeBuilderOptions()).Build(set);
            var actualItems = root.AsArray().Items.Select(x => x.RawObject).Cast<Guid>().ToArray();
            actualItems.ShouldBe(set.OrderBy(x => x).ToArray());
        }

        [Test]
        public void It_should_return_items_in_enumeration_order_for_unordered_collections_if_underlying_type_is_not_sortable()
        {
            var set = Enumerable.Range(0, 10).Select(_ => new object()).ToHashSet();
            var root = new ObjectTreeBuilder(new ObjectTreeBuilderOptions()).Build(set);
            var actualItems = root.AsArray().Items.Select(x => x.RawObject).ToArray();
            actualItems.ShouldBe(set.ToArray());
        }

        [Test]
        public void It_should_return_items_in_enumeration_order_for_ordered_collections_and_enumerables()
        {
            var input = new
            {
                Enumerable = Enumerable.Range(0, 5).OrderByDescending(x => x),
                Array = new[] { 1, 5, 3 }
            };
            var root = new ObjectTreeBuilder(new ObjectTreeBuilderOptions()).Build(input).AsObject();
            var actualEnumerable = root.Properties["Enumerable"].AsArray().Items.Select(x => x.RawObject).ToArray();
            actualEnumerable.ShouldBe(new object[] { 4, 3, 2, 1, 0 });
            var actualArray = root.Properties["Array"].AsArray().Items.Select(x => x.RawObject).ToArray();
            actualArray.ShouldBe(new object[] { 1, 5, 3 });
        }

        [Test]
        public void It_should_return_items_in_enumeration_order_for_non_generic_collections()
        {
            var input = new
            {
                List = new ArrayList { 5, 1, 3 },
                Array = new object[] { 1, 5, 1 }
            };
            var root = new ObjectTreeBuilder(new ObjectTreeBuilderOptions()).Build(input).AsObject();
            var actualEnumerable = root.Properties["List"].AsArray().Items.Select(x => x.RawObject).ToArray();
            actualEnumerable.ShouldBe(new object[] { 5, 1, 3 });
            var actualArray = root.Properties["Array"].AsArray().Items.Select(x => x.RawObject).ToArray();
            actualArray.ShouldBe(new object[] { 1, 5, 1 });
        }

        class ExceptionalObject
        {
            public string Name => "name";
            public string Exceptional => throw new InvalidOperationException("exceptional");
        }

        [Test]
        public void It_should_capture_exceptions_thrown_during_object_mapping()
        {
            var input = new ExceptionalObject();
            var root = new ObjectTreeBuilder(new ObjectTreeBuilderOptions()).Build(input);
            var nodes = root.EnumerateAll().ToDictionary(x => x.Path);
            nodes["$.Name"].AsValue().Value.ShouldBe("name");
            nodes["$.Exceptional"].AsValue().Value.ShouldBeOfType<ExceptionCapture>().ToString().ShouldBe("InvalidOperationException: exceptional");
        }

        [Test]
        public void It_should_capture_exceptions_caused_by_mappers()
        {
            var mapper = new Mock<ObjectMapper>();
            mapper.Setup(x => x.CanMap(It.Is((object o) => o is Parent))).Returns(true);
            mapper.Setup(x => x.GetProperties(It.IsAny<object>())).Throws(new IndexOutOfRangeException("foo"));

            var options = new ObjectTreeBuilderOptions();
            options.Mappers.Push(mapper.Object);
            var builder = new ObjectTreeBuilder(options);

            var input = new { Name = "Bob", Value = new Parent(), Value2 = 5 };

            var root = builder.Build(input);
            var nodes = root.EnumerateAll().ToDictionary(x => x.Path);
            nodes["$.Name"].AsValue().Value.ShouldBe("Bob");
            nodes["$.Value2"].AsValue().Value.ShouldBe(5);
            nodes["$.Value"].AsValue().Value.ShouldBeOfType<ExceptionCapture>().ToString().ShouldBe("IndexOutOfRangeException: foo");
        }

        [Test]
        public void It_should_map_exception_without_entering_long_recursion_on_TargetSite()
        {
            Exception GetThrownException()
            {
                try
                {
                    throw new InvalidOperationException("exception");
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            var input = new { Exception = GetThrownException() };

            var root = new ObjectTreeBuilder(new(){MaxDepth = 4}).Build(input);
            var nodes = root.EnumerateAll().Select(x => x.Path).ToArray();
            nodes.ShouldBe(new[]
            {
                "$", 
                "$.Exception", 
                "$.Exception.Data", 
                "$.Exception.HelpLink", 
                "$.Exception.HResult",
                "$.Exception.InnerException", 
                "$.Exception.Message", 
                "$.Exception.Source", 
                "$.Exception.StackTrace",
                "$.Exception.TargetSite"
            });
        }

        [Test]
        public void It_should_support_recursion_limits()
        {
            var input = new Node();
            var node = input;
            for (int i = 0; i < 5; ++i)
            {
                node.Sub = new Node();
                node = node.Sub;
            }

            var options = new ObjectTreeBuilderOptions { MaxDepth = 4 };
            var root = new ObjectTreeBuilder(options).Build(input);
            var nodes = root.EnumerateAll().Select(x => $"{x.Path}: {x}").ToArray();
            nodes.ShouldBe(new[]
            {
                "$: <object>", 
                "$.Sub: <object>", 
                "$.Sub.Sub: <object>", 
                "$.Sub.Sub.Sub: <object>", 
                "$.Sub.Sub.Sub.Sub: InvalidOperationException: Maximum node depth reached"
            });
        }

        private void AssertValueNode(ObjectTreeNode node, string path, object? value)
        {
            node.Kind.ShouldBe(ObjectTreeNodeKind.Value);
            node.Path.ShouldBe(path);
            node.AsValue().Value.ShouldBe(value);
        }

        public static IEnumerable<object?> GetValueInstances()
        {
            yield return null;
            yield return true;
            yield return (byte)1;
            yield return (short)2;
            yield return 3;
            yield return (long)3;
            yield return 3.14f;
            yield return 3.14;
            yield return "text";
            yield return 'c';

            yield return 3.77m;
            yield return Guid.NewGuid();
            yield return DateTime.Now;
            yield return DateTimeOffset.Now;
            yield return TimeSpan.FromSeconds(157);
            yield return DateTimeKind.Utc;
            yield return new FormattableType(5);
        }

        public static IEnumerable<IEnumerable> GetFlatCollectionInstances()
        {
            yield return new[] { "a", "ab", "abc" };
            yield return new List<TimeSpan> { TimeSpan.FromSeconds(1), TimeSpan.FromHours(2) };
            yield return Enumerable.Range(3, 7);
        }
    }

    class FormattableType : IFormattable
    {
        private readonly int _value;

        public FormattableType(int value)
        {
            _value = value;
        }

        public string ToString(string? format, IFormatProvider? formatProvider) => string.Format(formatProvider, format ?? "G", _value);
        public override string ToString() => ToString(null, null);
    }

    class PocoBase
    {
        public int Field = 1;
        public int Property { get; set; } = 2;
        public char Base => 'B';
    }
    class Poco : PocoBase
    {
        public static string? StaticField = null;
        public static string? StaticProperty { get; set; }
#pragma warning disable CS0414
        private string? _privateField = null;
#pragma warning restore CS0414
        private string? _setterOnlyProperty;
        private string? PrivateProperty { get; set; }

        public string SetterOnlyProperty
        {
            set => _setterOnlyProperty = value;
        }

        public new string Field = "field";
        public new string Property { get; set; } = "prop";
        public string this[int index] => string.Empty;
    }

    class Parent
    {
        public string Name { get; set; } = string.Empty;
        public List<Child> Children { get; } = new();
    }

    class Child
    {
        public string Name { get; set; } = string.Empty;
        public Parent? Parent { get; set; }
    }

    class Node { public Node? Sub { get; set; } }

    public interface ISomething<T> { }
    public class Something<T> { }
}
