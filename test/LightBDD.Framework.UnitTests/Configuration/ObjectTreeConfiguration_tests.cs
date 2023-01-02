using LightBDD.Core.Configuration;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Parameters.ObjectTrees;
using NUnit.Framework;
using Shouldly;
using System;

namespace LightBDD.Framework.UnitTests.Configuration
{
    [TestFixture]
    public class ObjectTreeConfiguration_tests
    {
        [Test]
        public void It_should_return_default_builder()
        {
            new ObjectTreeConfiguration().Builder.ShouldBeSameAs(ObjectTreeBuilder.Default);
        }

        [Test]
        public void It_should_configure_builder()
        {
            var options = ObjectTreeBuilderOptions.Default.ClearMappers();
            var builder = new ObjectTreeConfiguration().ConfigureBuilder(options).Builder;
            builder.Options.ShouldBeSameAs(options);
        }

        [Test]
        public void It_should_not_allow_configuring_builder_when_sealed()
        {
            var cfg = new LightBddConfiguration();
            cfg.Seal();
            Assert.Throws<InvalidOperationException>(() => cfg.ObjectTreeConfiguration().ConfigureBuilder(ObjectTreeBuilderOptions.Default));
        }
    }
}
