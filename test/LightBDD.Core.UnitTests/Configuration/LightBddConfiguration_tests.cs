using System;
using LightBDD.Core.Configuration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Configuration
{
    [TestFixture]
    public class LightBddConfiguration_tests
    {
        class SealableFeatureConfig : ISealableFeatureConfiguration
        {
            public void Seal()
            {
                IsSealed = true;
            }

            public bool IsSealed { get; private set; }
        }

        class NotSealableFeatureConfig : IFeatureConfiguration
        {
        }

        class TestableFeatureConfig : FeatureConfiguration
        {
            public void Foo() { ThrowIfSealed(); }
        }

        [Test]
        public void Get_should_instantiate_and_store_feature_config()
        {
            var cfg = new LightBddConfiguration();
            Assert.IsNotNull(cfg.Get<SealableFeatureConfig>());
            Assert.AreSame(cfg.Get<SealableFeatureConfig>(), cfg.Get<SealableFeatureConfig>());
        }

        [Test]
        public void Seal_should_make_existing_config_sealed()
        {
            var cfg = new LightBddConfiguration();
            var feature = cfg.Get<SealableFeatureConfig>();

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
            cfg.Seal();

            Assert.IsTrue(cfg.Get<SealableFeatureConfig>().IsSealed);
        }

        [Test]
        public void LightBddConfiguration_should_accept_config_not_supporting_sealing()
        {
            Assert.IsNotNull(new LightBddConfiguration().Seal().Get<NotSealableFeatureConfig>());

            var cfg = new LightBddConfiguration();
            var feat = cfg.Get<NotSealableFeatureConfig>();
            cfg.Seal();
            Assert.AreSame(feat, cfg.Get<NotSealableFeatureConfig>());
        }

        [Test]
        public void FeatureConfiguration_should_offer_protection_from_modifying_sealed_config()
        {
            var cfg = new LightBddConfiguration();
            Assert.DoesNotThrow(() => cfg.Get<TestableFeatureConfig>().Foo());

            cfg.Seal();

            var ex = Assert.Throws<InvalidOperationException>(() => cfg.Get<TestableFeatureConfig>().Foo());
            Assert.That(ex.Message, Is.EqualTo("Feature configuration is sealed. Please update configuration only during LightBDD initialization."));
        }
    }
}
