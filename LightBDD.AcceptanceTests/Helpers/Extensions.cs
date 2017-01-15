using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace LightBDD.AcceptanceTests.Helpers
{
    public static class Extensions
    {
        public static IWebElement FindFeature(this ChromeDriver driver, int featureNo)
        {
            return FindFeatures(driver).ElementAt(featureNo - 1);
        }

        public static IEnumerable<IWebElement> FindFeatures(this ChromeDriver driver)
        {
            return driver.FindElementsByClassName("feature");
        }

        public static IEnumerable<IWebElement> FindAllScenarios(this ChromeDriver driver)
        {
            return driver.FindElementsByClassName("scenario");
        }

        public static IEnumerable<IWebElement> FindAllSteps(this ChromeDriver driver)
        {
            return driver.FindElementsByClassName("step");
        }

        public static IWebElement FindScenario(this IWebElement element, int scenarioNo)
        {
            return FindScenarios(element).ElementAt(scenarioNo - 1);
        }

        public static IEnumerable<IWebElement> FindScenarios(this IWebElement element)
        {
            return element.FindElements(By.ClassName("scenario"));
        }

        public static IEnumerable<IWebElement> FindSteps(this IWebElement element)
        {
            return element.FindElements(By.ClassName("step"));
        }

        public static string[] GetClassNames(this IWebElement element)
        {
            return element.GetAttribute("class").Split(' ');
        }

        public static bool HasClassName(this IWebElement element, string className)
        {
            return element.GetClassNames().Any(c => c == className);
        }

        public static IWebElement FindLabelByText(this ChromeDriver driver, string text)
        {
            return driver.FindElementsByTagName("label").Single(l => l.Text == text);
        }

        public static IWebElement FindLabelTarget(this ChromeDriver driver, IWebElement label)
        {
            return driver.FindElementById(label.GetAttribute("for"));
        }
    }
}