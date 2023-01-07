using System;
using System.Collections.Generic;
using System.Threading;
using LightBDD.Framework.Parameters;
using LightBDD.XUnit2;

namespace Example.LightBDD.XUnit2.Features
{
    public partial class Record_persistence_feature : FeatureFixture
    {
        class DataRecord
        {
            public Guid Id { get; set; }
            public DateTimeOffset ModifiedDate { get; set; }
            public string Name { get; set; }
        }

        private readonly List<DataRecord> _records = new List<DataRecord>();

        private void Given_no_saved_records()
        {
            _records.Clear();
        }

        private void When_I_save_records(InputTree<string[]> records)
        {
            foreach (var name in records.Input)
            {
                _records.Add(new DataRecord { Id = Guid.NewGuid(), ModifiedDate = DateTimeOffset.UtcNow, Name = name });
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

        private void Then_the_saved_records_should_match_expectation(VerifiableTree expectation)
        {
            expectation.SetActual(_records);
        }
    }
}