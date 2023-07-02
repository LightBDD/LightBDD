using LightBDD.NUnit3;
using NUnit.Framework;

namespace Example.LightBDD.NUnit3.Features
{
    public partial class Internationalization_feature : FeatureFixture
    {
        private string _selectedLanguage;

        private void Given_a_customer_with_LANG_language_selected(string lang)
        {
            _selectedLanguage = lang;
        }

        private void When_the_customer_opens_the_home_page()
        {
        }

        private void Then_header_should_display_LANG_language(string lang)
        {
            Assert.That(_selectedLanguage, Is.EqualTo(lang));
        }

        private void Then_page_title_should_be_translated()
        {
        }

        private void Then_all_menu_items_should_be_translated()
        {
        }
    }
}