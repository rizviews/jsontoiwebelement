namespace Ridia.TestAutomation.Tests
{
    using Xunit;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Ridia.TestAutomation;
    using FluentAssertions;
    using System.IO;

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

        [Fact]
        public void ShouldReadContentOfAllFiles()
        {
            JsonToIWebElement element = new JsonToIWebElement(@"TestData");
        }

        [Fact]
        public void ShouldGenerateAClass()
        {
            JsonToNativeElementsConverter jsnc = new JsonToNativeElementsConverter();

            foreach (string file in Directory.GetFiles(@"C:\Nintex_Works\Code\o365-ui-automation\source\O365.UI.Automation.Common\Element\ElementDefinitions","*.json"))
            {
                jsnc.ToElements(file, "O365.UI.Automation.Common.Element");
                
            }

            //jsnc.ToElements("TestData/TestElements.json");
        }
    }
}
