using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Example.Domain.Domain;
using LightBDD.Framework;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Parameters;
using NUnit.Framework;

namespace Example.LightBDD.NUnit2.Features.Contexts
{
    public class ContactsManagementContext
    {
        private ContactBook _contactBook = new ContactBook();
        private readonly List<Contact> _addedContacts = new List<Contact>();
        private readonly List<Contact> _removedContacts = new List<Contact>();
        private Contact[] _searchResults = new Contact[0];

        public void Given_my_contact_book_is_empty()
        {
            _contactBook = new ContactBook();
        }

        public void When_I_add_new_contacts()
        {
            AddSomeContacts();
        }

        public void Then_all_contacts_should_be_available_in_the_contact_book()
        {
            Assert.That(
                _contactBook.Contacts.ToArray(),
                Is.EquivalentTo(_addedContacts),
                "Contacts should be added to contact book");
        }

        public void Given_my_contact_book_is_filled_with_contacts()
        {
            _contactBook = new ContactBook();
            AddSomeContacts();
        }

        public void When_I_remove_one_contact()
        {
            RemoveContact(_contactBook.Contacts.First());
        }

        public void Then_the_contact_book_should_not_contain_removed_contact_any_more()
        {
            Assert.AreEqual(
                Enumerable.Empty<Contact>(),
                _contactBook.Contacts.Where(c => _removedContacts.Contains(c)).ToArray(),
                "Contact book should not contain removed books");
        }

        public void Then_the_contact_book_should_contains_all_other_contacts()
        {
            Assert.That(
                _addedContacts.Except(_removedContacts).ToArray(),
                Is.EquivalentTo(_contactBook.Contacts.ToArray()),
                "All contacts that has not been explicitly removed should be still present in contact book");
        }

        public void Given_my_contact_book_is_filled_with_many_contacts()
        {
            for (var i = 0; i < 10000; ++i)
                _contactBook.AddContact(i.ToString(CultureInfo.InvariantCulture), i.ToString(CultureInfo.InvariantCulture), i.ToString(CultureInfo.InvariantCulture));
        }

        public void When_I_clear_it()
        {
            foreach (var contact in _contactBook.Contacts.ToArray())
                RemoveContact(contact);
            StepExecution.Current.Bypass("Contact book clearing is not implemented yet. Contacts are removed one by one.");
        }

        private void RemoveContact(Contact contact)
        {
            _removedContacts.Add(contact);
            _contactBook.Remove(contact.Name);
        }

        public void Then_the_contact_book_should_be_empty()
        {
            Assert.IsEmpty(_contactBook.Contacts, "Contact book should be empty");
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
            _addedContacts.Add(contact);
            _contactBook.AddContact(contact.Name, contact.PhoneNumber, contact.Email);
        }

        public void Given_I_added_contact_with_name_phone_and_email(string name, string phone, string email)
        {
            AddContact(new Contact(name, phone, email));
        }

        public void When_I_search_for_contacts_by_phone_starting_with(string with)
        {
            _searchResults = _contactBook.SearchByPhoneStartingWith(with).ToArray();
        }

        public void Then_the_result_should_contain_name_with_phone_and_email(string name, Expected<string> phone, Expected<string> email)
        {
            var contact = _searchResults.First(x => x.Name == name);
            phone.SetActual(contact.PhoneNumber);
            email.SetActual(contact.Email);
        }

        public void Then_I_should_receive_contacts(VerifiableTable<KeyValuePair<string, Contact>> contacts)
        {
            contacts.SetActual(_searchResults.ToDictionary(x => x.Name, x => x));
        }
    }
}