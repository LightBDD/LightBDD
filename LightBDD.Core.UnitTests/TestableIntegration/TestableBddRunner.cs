using System;
using LightBDD.Core.Implementation;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    public class TestableBddRunner : CoreBddRunner
    {
        public TestableBddRunner(Type featureType)
            : base(featureType, new TestableIntegrationContext())
        {
        }
    }
}