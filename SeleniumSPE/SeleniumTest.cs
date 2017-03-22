using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumSPE
{
    class SeleniumTest
    {
        private const string DriverPath = "C:\\drivers";
        private const int WaitTime = 2;
        private const string Url = "http://localhost:3000/";

        [SetUp]
        public void BeforeEach()
        {
            var client = new HttpClient();
            var req = new HttpRequestMessage(HttpMethod.Get, Url + "reset");
            var res = client.SendAsync(req).Result;

            if(!res.IsSuccessStatusCode)
                throw new InvalidOperationException();
        }

        [Test]
        public void InitialLoadTest()
        {
            using (var driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(DriverPath)))
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitTime));
                driver.Navigate().GoToUrl(Url);

                wait.Until(d =>
                {
                    var rows = d.FindElements(By.CssSelector("#tbodycars tr"));
                    return rows.Count == 5;
                });
            }
        }

        [Test]
        public void FilterTest()
        {
            using (var driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(DriverPath)))
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitTime));
                driver.Navigate().GoToUrl(Url);

                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".table")));

                wait.Until(d =>
                {
                    var filter = driver.FindElementById("filter");
                    if (filter == null)
                        return false;
                    filter.SendKeys("2002");
                    return true;
                });

                wait.Until(d =>
                {
                    var rows = d.FindElements(By.CssSelector("#tbodycars tr"));
                    return rows.Count == 2;
                });

                driver.FindElementById("filter").SendKeys(Keys.Control + "a");
                driver.FindElementById("filter").SendKeys(Keys.Delete);

                wait.Until(d =>
                {
                    var rows = d.FindElements(By.CssSelector("#tbodycars tr"));
                    return rows.Count == 5;
                });
            }
        }

        [Test]
        public void SortTest()
        {
            using (var driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(DriverPath)))
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitTime));
                driver.Navigate().GoToUrl(Url);

                wait.Until(d =>
                {
                    var sortBtn = driver.FindElementByCssSelector("#h_year");
                    if (sortBtn == null)
                        return false;
                    sortBtn.Click();
                    return true;
                });

                wait.Until(d =>
                {
                    var rows = d.FindElements(By.CssSelector("#tbodycars tr"));

                    if (rows == null || rows.Count <= 0)
                        return false;

                    var firstRow = rows[0];
                    var lastRow = rows[rows.Count - 1];


                    return firstRow.FindElements(By.CssSelector("td"))[0].Text == "938" &&
                           lastRow.FindElements(By.CssSelector("td"))[0].Text == "940";
                });
            }
        }
    }
}
