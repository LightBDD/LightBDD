using System.Text.Json;
using Example.Domain.Domain;
using LightBDD.Framework;
using LightBDD.Framework.Parameters;
using LightBDD.XUnit2;

namespace Example.LightBDD.XUnit2.Features
{
    public partial class Address_book_feature : FeatureFixture
    {
        private AddressBook _book;
        private string _json;

        private void Given_an_empty_address_book()
        {
            _book = new AddressBook();
        }

        private void When_I_associate_contact_with_address_as_alias(InputTree<Contact> contact, InputTree<PostalAddress> address, string alias)
        {
            _book.AddContactAddress(new ContactAddress(alias, contact.Input, address.Input));
        }

        private void Then_address_by_email_should_match(VerifiableTree match)
        {
            match.SetActual(_book.ContactsByEmail);
        }

        private void Given_an_address_book_with_contacts(InputTree<ContactAddress[]> contacts)
        {
            _book = new AddressBook();
            foreach (var address in contacts.Input)
                _book.AddContactAddress(address);
        }

        private void When_I_persist_book_as_json()
        {
            _json = JsonSerializer.Serialize(_book);
        }

        private void Then_address_book_should_match_persisted_json(VerifiableTree json)
        {
            StepExecution.Current.Comment($"Underlying type: {json.Expected?.GetType().Name}");
            json.SetActual(_book);
        }

        private void Then_address_book_should_contain_contacts(VerifiableTree<Contact[]> contacts)
        {
            contacts.SetActual(_book.GetContacts());
        }

        private void Then_address_book_should_contain_postal_addresses(VerifiableTree addresses)
        {
            addresses.SetActual(_book.GetAddresses());
        }
    }
}