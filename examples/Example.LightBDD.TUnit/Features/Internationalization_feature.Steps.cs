using System.Threading.Tasks;
using LightBDD.TUnit;

namespace Example.LightBDD.TUnit.Features
{
    public partial class Internationalization_feature : FeatureFixture
    {
        private string _selectedLanguage;

        private async Task Given_a_customer_with_LANG_language_selected(string lang)
        {
            _selectedLanguage = lang;
        }

        private async Task When_the_customer_opens_the_home_page()
        {
        }

        private async Task Then_header_should_display_LANG_language(string lang)
        {
           await Assert.That(_selectedLanguage).IsEqualTo(lang);
        }

        private async Task Then_page_title_should_be_translated()
        {
        }

        private async Task Then_all_menu_items_should_be_translated()
        {
        }
    }
}