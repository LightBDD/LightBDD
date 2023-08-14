using System;
using LightBDD.Core.Configuration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Configuration
{
    [TestFixture]
    public class LightBddConfiguration_tests
    {
        private class SealableFeatureConfig : FeatureConfiguration
        {
            public new bool IsSealed => base.IsSealed;
        }

        private class TestableFeatureConfig : FeatureConfiguration
        {
            public void Foo() { ThrowIfSealed(); }
        }

        [Test]
        public void Get_should_instantiate_and_store_feature_config()
        {
            var cfg = new LightBddConfiguration();
            Assert.IsNotNull(cfg.ConfigureFeature<SealableFeatureConfig>());
            Assert.AreSame(cfg.ConfigureFeature<SealableFeatureConfig>(), cfg.ConfigureFeature<SealableFeatureConfig>());
        }

        [Test]
        public void Seal_should_make_existing_config_sealed()
        {
            var cfg = new LightBddConfiguration();
            var feature = cfg.ConfigureFeature<SealableFeatureConfig>();

            Assert.IsFalse(feature.IsSealed);
            Assert.IsFalse(cfg.IsSealed);

            cfg.Seal();

            Assert.IsTrue(cfg.IsSealed);
            Assert.IsTrue(feature.IsSealed);
        }

        [Test]
        public void Seal_should_make_future_config_sealed()
        {
            var cfg = new LightBddConfiguration();
            var feature = cfg.ConfigureFeature<SealableFeatureConfig>();
            cfg.Seal();

            Assert.IsTrue(feature.IsSealed);
        }

        [Test]
        public void FeatureConfiguration_should_offer_protection_from_modifying_sealed_config()
        {
            var cfg = new LightBddConfiguration();
            Assert.DoesNotThrow(() => cfg.ConfigureFeature<TestableFeatureConfig>().Foo());

            cfg.Seal();

            var ex = Assert.Throws<InvalidOperationException>(() => cfg.ConfigureFeature<TestableFeatureConfig>().Foo());
            Assert.That(ex.Message, Is.EqualTo("Feature configuration is sealed. Please update configuration only during LightBDD initialization."));
        }
    }
}
