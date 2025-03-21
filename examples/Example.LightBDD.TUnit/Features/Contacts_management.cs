using System.Threading.Tasks;
using Example.Domain.Domain;
using Example.LightBDD.TUnit.Features.Contexts;
using LightBDD.Framework;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Parameters;
using LightBDD.Framework.Scenarios;
using LightBDD.TUnit;

namespace Example.LightBDD.TUnit.Features
{
    /// <summary>
    /// This feature class presents the usage of contextual scenarios that are used in conjunction with extended step format.
    /// With contextual scenarios it is possible to share state between all steps, which is useful in the situation where
    /// sharing state via feature class fields is not desired.
    /// 
    /// To use contextual scenarios, the Runner.WithContext() method has to be used and the context object can be then accessed by step with lambda parameter.
    /// The scenario below shows the recommended usage of context, where given-when-then steps are declared on the context class (it is however not enforced rule).
    /// 
    /// Another feature presented here is that if lambda parameter name is 1 character only, it is ignored in the reports, and the step type (GIVEN/WHEN/THEN/etc)
    /// is inferred from the step method name.
    ///
    /// Finally, the scenarios presents usage of tabular parameters, including <see cref="InputTable{TRow}"/>, <see cref="VerifiableDataTable{TRow}"/> and <see cref="TableValidator{TRow}"/>.
    /// 
    /// More information on contextual scenarios can be found here: https://github.com/LightBDD/LightBDD/wiki/Scenario-Steps-Definition#contextual-scenarios
    /// </summary>
    [FeatureDescription(
@"In order to maintain my contact book
As an application user
I want to add, browse and remove my contacts")]
    [Label("Story-6")]
    public class Contacts_management : FeatureFixture
    {
        [Scenario]
        [Label("Ticket-8")]
        public async Task Contact_book_should_allow_me_to_add_multiple_contacts()
        {
            await Runner.WithContext<ContactsManagementContext>().RunScenarioAsync(
                _ => _.Given_my_contact_book_is_empty(),
                _ => _.When_I_add_new_contacts(),
                _ => _.Then_all_contacts_should_be_available_in_the_contact_book());
        }

        [Scenario]
        [Label("Ticket-9")]
        public async Task Contact_book_should_allow_me_to_remove_contacts()
        {
            await Runner.WithContext<ContactsManagementContext>().RunScenarioAsync(
                _ => _.Given_my_contact_book_is_filled_with_contacts(),
                _ => _.When_I_remove_one_contact(),
                _ => _.Then_the_contact_book_should_not_contain_removed_contact_any_more(),
                _ => _.Then_the_contact_book_should_contains_all_other_contacts());
        }

        [Scenario]
        [Label("Ticket-9")]
        [ScenarioDescription("This scenario presents how LightBDD reports bypassed steps")]
        public async Task Contact_book_should_allow_me_to_remove_all_contacts()
        {
            await Runner.WithContext<ContactsManagementContext>().RunScenarioAsync(
                _ => _.Given_my_contact_book_is_filled_with_many_contacts(),
                _ => _.When_I_clear_it(),
                _ => _.Then_the_contact_book_should_be_empty());
        }

        [Scenario]
        [ScenarioDescription("This scenario presents failures captured by VerifiableTree")]
        public async Task Searching_for_contacts_by_phone()
        {
            await Runner.WithContext<ContactsManagementContext>().RunScenarioAsync(
                c => c.Given_my_contact_book_is_empty(),
                c => c.Given_I_added_contacts(Table.For(
                    new Contact("John", "111-222-333", "john123@gmail.com"),
                    new Contact("John", "111-303-404", "jo@hotmail.com"),
                    new Contact("Greg", "213-444-444", "greg22@gmail.com"),
                    new Contact("Emily", "111-222-5556", "emily1@gmail.com"),
                    new Contact("Kathy", "111-555-330", "ka321@gmail.com"))),
                c => c.When_I_search_for_contacts_by_phone_starting_with("111"),
                c => c.Then_I_should_receive_contacts(Table.ExpectData(
                    b => b.WithInferredColumns()
                        .WithKey(x => x.Name),
                    new Contact("Emily", "111-222-5556", "emily1@gmail.com"),
                    new Contact("John", "111-222-333", "john@hotmail.com"),
                    new Contact("John", "111-303-404", "jo@hotmail.com"),
                    new Contact("Kathie", "111-555-330", "ka321@gmail.com")
                )));
        }

        [Scenario]
        public async Task Displaying_contacts_alphabetically()
        {
            await Runner.WithContext<ContactsManagementContext>().RunScenarioAsync(
                c => c.Given_my_contact_book_is_empty(),
                c => c.Given_I_added_contacts(Table.For(
                    new Contact("John", "111-222-333", "john123@gmail.com"),
                    new Contact("Greg", "213-444-444", "greg22@gmail.com"),
                    new Contact("Emily", "111-222-5556", "emily1@gmail.com"),
                    new Contact("Kathy", "111-555-330", "ka321@gmail.com"))),
                c => c.When_I_request_contacts_sorted_by_name(),
                c => c.Then_I_should_receive_contacts(Table.ExpectData(
                    new Contact("Emily", "111-222-5556", "emily1@gmail.com"),
                    new Contact("Greg", "213-444-444", "greg22@gmail.com"),
                    new Contact("John", "111-222-333", "john123@gmail.com"),
                    new Contact("Kathy", "111-555-330", "ka321@gmail.com"))));
        }

        [Scenario]
        [ScenarioDescription("This scenario presents failures captured by VerifiableTable")]
        public async Task Normalizing_contact_details()
        {
            await Runner.WithContext<ContactsManagementContext>().RunScenarioAsync(
                c => c.Given_I_added_contacts(Table.For(
                    new Contact("John", "00441123344555", "john253@mymail.com"),
                    new Contact("Jenny", "112334455", "jenny213@mymail.com"),
                    new Contact("Jerry", "1123344556", "jerry123@mymail.com"),
                    new Contact("Josh", "12111333444", "jos#@mymail.com"))),
                c => c.When_I_request_contacts_sorted_by_name(),
                c => c.Then_I_should_receive_contacts(Table.Validate<Contact>(b => b
                    .WithColumn(x => x.Name, Expect.To.Not.BeEmpty())
                    .WithColumn(x => x.Email, Expect.To.Match("[a-z0-9.-]+@[a-z0-9.-]+"))
                    .WithColumn(x => x.PhoneNumber, Expect.To.Match("[0-9]{10,14}"))
                )));
        }
    }
}