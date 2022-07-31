using OpenQA.Selenium.Chrome;

namespace LightBDD.AcceptanceTests.Helpers;

internal interface IChromeDriverContext
{
    ChromeDriver Driver { get; }
}