namespace JsonToIWebElement.Tests
{
    using FluentAssertions;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Ridia.TestAutomation;
    using Ridia.TestAutomation.Exceptions;
    using Xunit;

    /// <summary>
    /// Test for JsonToIWebElement 
    /// </summary>
    public class JsonToIWebElementTest
    {
        /// <summary>
        /// Should return element from JSON.
        /// </summary>
        [Fact(Skip = "Cannot find search box element")]
        public void ShouldReturnElementFromJson()
        {
            ChromeDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.google.com");
            JsonToIWebElement pageElement = new JsonToIWebElement("Google.json", driver);
            IWebElement element = pageElement.GetElement("SearchBox");
            element.SendKeys("test with xunit");

            driver.Close();
        }

        /// <summary>
        /// Should not return element from JSON as element name is case sensitive.
        /// </summary>
        [Fact(DisplayName = "Should not find an element in definition when case doesn't match")]
        public void ShouldNotReturnElementFromJsonCaseSensitive()
        {
            JsonToIWebElement pageElement = new JsonToIWebElement("Google.json");
            var exception = Assert.Throws<ElementNotFoundException>(() => pageElement.GetElement("SearchBOX"));
            Assert.Contains("SearchBOX", exception.Message);
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

            IWebElement element = pageElement.GetElement("SearchBoxWithToken", null, "q");

            element.Should().NotBeNull();
            element.TagName.Should().Be("input");

            driver.Close();
        }

        [Fact]
        public void ShouldReadContentOfAllFiles()
        {
            JsonToIWebElement element = new JsonToIWebElement(@"TestData");
        }

        [Fact(DisplayName = "Should fail when loading a single file which contains duplicate element definitions")]
        public void ShouldFailWithDuplicateElementsInSingleFile()
        {
            var exception = Assert.Throws<DuplicateElementException>(() => new JsonToIWebElement("DuplicateElements.json"));
            Assert.Contains("SearchBox", exception.Message);
            Assert.Contains("SearchBox3", exception.Message);
        }

        [Fact(DisplayName = "Should fail when loading multiple files which contain duplicate element definitions")]
        public void ShouldFailWithDuplicateElementsInMultipleFiles()
        {
            var exception = Assert.Throws<DuplicateElementException>(() => new JsonToIWebElement("DuplicateTestData"));
            Assert.Contains("ElementB", exception.Message);
            Assert.Contains("ElementD", exception.Message);
        }
    }
}
