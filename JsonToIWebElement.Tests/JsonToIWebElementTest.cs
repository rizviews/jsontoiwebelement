namespace JsonToIWebElement.Tests
{
    using Xunit;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Ridia.TestAutomation;
    using FluentAssertions;

    /// <summary>
    /// 
    /// </summary>
    public class JsonToIWebElementTest
    {
        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void ShouldReturnElementFromJson()
        {
            ChromeDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.google.com");
            JsonToIWebElement pageElement = new JsonToIWebElement("Google.json", driver);
            IWebElement element = pageElement.GetElement("SearchBox");
            element.SendKeys("test with xunit");

            driver.Close();
        }

        [Fact]
        public void ShouldGetDefinition()
        {
            JsonToIWebElement pageElement = new JsonToIWebElement("Google.json");

            string definition = pageElement.GetDefinition("SearchBox");
            definition.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void ShouldGetElementWithTokenReplaced()
        {
            ChromeDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.google.com");

            JsonToIWebElement pageElement = new JsonToIWebElement("Google.json", driver);

            IWebElement element = pageElement.GetElement("SearchBoxWithToken",null,"q");

            element.Should().NotBeNull();
            element.TagName.Should().Be("input");

            driver.Close();
        }
    }
}
