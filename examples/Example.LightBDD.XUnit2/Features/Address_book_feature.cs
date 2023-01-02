using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using Example.Domain.Domain;
using LightBDD.Framework;
using LightBDD.Framework.Parameters;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Newtonsoft.Json;

namespace Example.LightBDD.XUnit2.Features
{
    /// <summary>
    /// This feature class presents the usage of <see cref="InputTree{TData}"/> and <see cref="VerifiableTree{TData}"/> parameter types to structurally verify and visualize hierarchical object structure.
    /// </summary>
    [FeatureDescription(
@"In order to maintain my product dispatch
As an application user
I want to add and browse my client postal addresses by client emails")]
    public partial class Address_book_feature
    {

        [Scenario]
        public void Adding_contacts()
        {
            Runner.RunScenario(
                _ => Given_an_empty_address_book(),
                _ => When_I_associate_contact_with_address(
                    new Contact("Joe Jonnes", "666777888", "joe67@email.com"),
                    new PostalAddress("UK", "London", "AB1 2CD", "47 Main Street")),
                _ => When_I_associate_contact_with_address(
                    new Contact("Jan Nowak", "123654789", "nowak33@email.com"),
                    new PostalAddress("Poland", "Kraków", "31-042", "Rynek Główny 1")),
                _ => Then_address_by_email_should_match(Tree.ExpectAtLeastContaining(new Dictionary<string, object>
                {
                    { "nowak33@email.com", new { Address = new { Country = "Poland", City = "Kraków", PostCode = "31-042", Address = "Rynek Główny 1" } } },
                    { "joe67@email.com", new { Address = new { Country = "UK", City = "London", PostCode = "AB1 2CD", Address = "47 Main Street" } } }
                })));
        }

        [Scenario]
        public void Persisting_address_book()
        {
            Runner.RunScenario(
                _ => Given_an_address_book_with_contacts(new[]
                {
                    new ContactAddress(new Contact("Joe Jonnes", "666777888", "joe67@email.com"),
                        new PostalAddress("UK", "London", "AB1 2CD", "47 Main Street")),
                    new ContactAddress(new Contact("Jan Nowak", "123654789", "nowak33@email.com"),
                        new PostalAddress("Poland", "Kraków", "31-042", "Rynek Główny 1"))
                }),
                _ => When_I_persist_book_as_json(),
                _ => Then_address_book_should_match_persisted_json(Tree.ExpectEquivalent(JsonDocument.Parse(_json, default).RootElement)),
                _ => Then_address_book_should_match_persisted_json(Tree.ExpectEquivalent(JsonConvert.DeserializeObject<ExpandoObject>(_json))));
        }
    }
}