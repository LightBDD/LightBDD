using System.Diagnostics;
using System.Globalization;
using System.Linq;
using LightBDD.Example.Domain;
using LightBDD.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Debuggable(true, true)]

namespace LightBDD.Example.AcceptanceTests.MsTest.Features
{
    public partial class Contacts_management : FeatureFixture
    {
        public class ScenarioContext
        {
            public ContactBook ContactBook { get; set; }
            public Contact[] AddedContacts { get; set; }
            public Contact[] RemovedContacts { get; set; }

            public ScenarioContext()
            {
                ContactBook = new ContactBook();
                AddedContacts = new Contact[0];
                RemovedContacts = new Contact[0];
            }
        }

        private void Given_my_contact_book_is_empty(ScenarioContext ctx)
        {
            ctx.ContactBook = new ContactBook();
        }

        private void When_I_add_new_contacts(ScenarioContext ctx)
        {
            AddSomeContacts(ctx);
        }

        private void Then_all_of_expected_contacts_would_be_available_in_contact_book(ScenarioContext ctx)
        {
            CollectionAssert.AreEquivalent(
                ctx.AddedContacts,
                ctx.ContactBook.Contacts.ToArray(),
                "Contacts should be added to contact book");
        }

        private void Given_my_contact_book_is_filled_with_contacts(ScenarioContext ctx)
        {
            ctx.ContactBook = new ContactBook();
            AddSomeContacts(ctx);
        }

        private void When_I_remove_contact(ScenarioContext ctx)
        {
            ctx.RemovedContacts = ctx.ContactBook.Contacts.Take(1).ToArray();
            foreach (var contact in ctx.RemovedContacts)
                ctx.ContactBook.Remove(contact.Name);
        }

        private void Then_contact_book_does_not_contain_removed_contact_any_more(ScenarioContext ctx)
        {
            Assert.IsFalse(
                ctx.ContactBook.Contacts.Where(c => ctx.RemovedContacts.Contains(c)).ToArray().Any(),
                "Contact book should not contain removed books");
        }

        private void Then_contact_book_still_contains_other_contacts(ScenarioContext ctx)
        {
            CollectionAssert.AreEquivalent(
                ctx.ContactBook.Contacts.ToArray(),
                ctx.AddedContacts.Except(ctx.RemovedContacts).ToArray(),
                "All contacts that has not been explicitly removed should be still present in contact book");
        }

        private void AddSomeContacts(ScenarioContext ctx)
        {
            ctx.AddedContacts = new[]
            {
                new Contact("Jack", "123-456-789"),
                new Contact("Samantha", "321-654-987"),
                new Contact("Josh", "132-465-798")
            };

            foreach (var contact in ctx.AddedContacts)
                ctx.ContactBook.AddContact(contact.Name, contact.PhoneNumber);
        }

        private void Given_my_contact_book_is_filled_with_many_contacts(ScenarioContext ctx)
        {
            for (int i = 0; i < 10000; ++i)
                ctx.ContactBook.AddContact(i.ToString(CultureInfo.InvariantCulture), i.ToString(CultureInfo.InvariantCulture));
        }

        private void When_I_clear_it(ScenarioContext ctx)
        {
            foreach (var contact in ctx.ContactBook.Contacts.ToArray())
                ctx.ContactBook.Remove(contact.Name);
            StepExecution.Bypass("Contact book clearing is not implemented yet. Contacts are removed one by one.");
        }

        private void Then_contact_book_is_empty(ScenarioContext ctx)
        {
            Assert.IsFalse(ctx.ContactBook.Contacts.Any(), "Contact book should be empty");
        }
    }
}