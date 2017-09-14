namespace JsonToIWebElement.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Ridia.TestAutomation;

    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    public class JsonToIWebElementTest
    {
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void ShouldReturnElementFromJson()
        {
            ChromeDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.google.com");
            JsonToIWebElement pageElement = new JsonToIWebElement("Google.json", driver);
            IWebElement element = pageElement.GetElement("SearchBox");
            element.SendKeys("test with xunit");
        }

        [TestMethod]
        public void ShouldGetDefinition()
        {
            JsonToIWebElement pageElement = new JsonToIWebElement("Google.json");

            string definition = pageElement.GetDefinition("SearchBox");

            Assert.IsNotNull(definition);
        }
    }
}
