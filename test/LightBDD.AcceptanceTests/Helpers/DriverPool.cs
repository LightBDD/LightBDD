using System;
using System.Collections.Concurrent;
using OpenQA.Selenium.Chrome;

namespace LightBDD.AcceptanceTests.Helpers
{
    public static class DriverPool
    {
        private static readonly ConcurrentQueue<ChromeDriver> Drivers = new ConcurrentQueue<ChromeDriver>();

        public static ChromeDriver Acquire()
        {
            return Drivers.TryDequeue(out var item) 
                ? item 
                : CreateDriver();
        }

        public static void Release(ChromeDriver driver)
        {
            Drivers.Enqueue(driver);
        }

        private static ChromeDriver CreateDriver()
        {
            var driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(0);
            return driver;
        }

        public static void CloseAll()
        {
            foreach (var driver in Drivers)
            {
                driver.Close();
                driver.Dispose();
            }
        }
    }
}