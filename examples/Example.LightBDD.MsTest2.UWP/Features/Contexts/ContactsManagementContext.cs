using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Example.Domain.Domain;
using LightBDD.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Example.LightBDD.MsTest2.UWP.Features.Contexts
{
    public class ContactsManagementContext
    {
        public ContactBook ContactBook { get; private set; } = new ContactBook();
        public List<Contact> AddedContacts { get; } = new List<Contact>();
        public List<Contact> RemovedContacts { get; } = new List<Contact>();

        public void Given_my_contact_book_is_empty()
        {
            ContactBook = new ContactBook();
        }

        public void When_I_add_new_contacts()
        {
            AddSomeContacts();
        }

        public void Then_all_contacts_should_be_available_in_the_contact_book()
        {
            CollectionAssert.AreEquivalent(
                AddedContacts,
                ContactBook.Contacts.ToArray(),
                "Contacts should be added to contact book");
        }

        public void Given_my_contact_book_is_filled_with_contacts()
        {
            ContactBook = new ContactBook();
            AddSomeContacts();
        }

        public void When_I_remove_one_contact()
        {
            RemoveContact(ContactBook.Contacts.First());
        }

        public void Then_the_contact_book_should_not_contain_removed_contact_any_more()
        {
            Assert.IsFalse(
                ContactBook.Contacts.Where(c => RemovedContacts.Contains(c)).ToArray().Any(),
                "Contact book should not contain removed books");
        }

        public void Then_the_contact_book_should_contains_all_other_contacts()
        {
            CollectionAssert.AreEquivalent(
                    ContactBook.Contacts.ToArray(),
                    AddedContacts.Except(RemovedContacts).ToArray(),
                    "All contacts that has not been explicitly removed should be still present in contact book");
        }

        public void Given_my_contact_book_is_filled_with_many_contacts()
        {
            for (var i = 0; i < 10000; ++i)
                ContactBook.AddContact(i.ToString(CultureInfo.InvariantCulture), i.ToString(CultureInfo.InvariantCulture), i.ToString(CultureInfo.InvariantCulture));
        }

        public void When_I_clear_it()
        {
            foreach (var contact in ContactBook.Contacts.ToArray())
                RemoveContact(contact);
            StepExecution.Current.Bypass("Contact book clearing is not implemented yet. Contacts are removed one by one.");
        }

        private void RemoveContact(Contact contact)
        {
            RemovedContacts.Add(contact);
            ContactBook.Remove(contact.Name);
        }

        public void Then_the_contact_book_should_be_empty()
        {
            Assert.IsFalse(ContactBook.Contacts.Any(), "Contact book should be empty");
        }

        private void AddSomeContacts()
        {
            var contacts = new[]
            {
            new Contact("Jack", "123-456-789","justjack@hotmail.com"),
            new Contact("Samantha", "321-654-987","samantha359@gmai.com"),
            new Contact("Josh", "132-465-798","jos4@gmail.com")
        };

            foreach (var contact in contacts)
                AddContact(contact);
        }

        private void AddContact(Contact contact)
        {
            AddedContacts.Add(contact);
            ContactBook.AddContact(contact.Name, contact.PhoneNumber, contact.Email);
        }
    }
}