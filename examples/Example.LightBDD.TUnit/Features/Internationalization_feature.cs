﻿using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;

namespace Example.LightBDD.TUnit.Features
{
    /// <summary>
    /// This feature class shows how to create parameterized scenarios: https://github.com/LightBDD/LightBDD/wiki/Scenario-Steps-Definition#parameterized-scenarios
    /// </summary>
    [Label("Story-10")]
    [FeatureDescription(
@"In order to easily use the website
As a user
I want to see the website in my language")]
    public partial class Internationalization_feature
    {
        [Scenario]
        [Arguments("EN")]
        [Arguments("PL")]
        [Arguments("DE")]
        public async Task Displaying_home_page_in_LANG(string lang)
        {
            await Runner.RunScenarioAsync(
                _ => Given_a_customer_with_LANG_language_selected(lang),
                _ => When_the_customer_opens_the_home_page(),
                _ => Then_header_should_display_LANG_language(lang),
                _ => Then_page_title_should_be_translated(),
                _ => Then_all_menu_items_should_be_translated()
            );
        }
    }
}