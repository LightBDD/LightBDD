using System;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Parameters;
using LightBDD.Framework.Scenarios;
using LightBDD.NUnit3;

namespace Example.LightBDD.NUnit3.Features
{
    /// <summary>
    /// This feature class presents the usage of <see cref="InputTree{TData}"/> and <see cref="VerifiableTree{TData}"/> parameter types to structurally verify and visualize hierarchical object structure.
    /// </summary>
    public partial class Record_persistence_feature
    {
        [Scenario]
        public void Saving_data()
        {
            var testRunDate = DateTimeOffset.UtcNow;

            Runner.RunScenario(
                _ => Given_no_saved_records(),
                _ => When_I_save_records(new[] { "Record 1", "Record 2" }),
                _ => Then_the_saved_records_should_match_expectation(Tree.ExpectEquivalent(new[]
                {
                    new
                    {
                        Id = Expect.To.Not.Equal(Guid.Empty),
                        ModifiedDate = Expect.To.BeGreaterOrEqual(testRunDate),
                        Name = Expect.To.BeLike("Recr*")
                    },
                    new
                    {
                        Id = Expect.To.Equal(Guid.Empty),
                        ModifiedDate = Expect.To.BeLessThan(testRunDate),
                        Name = Expect.To.Equal("Record 2")
                    },
                }))
            );
        }
    }
}