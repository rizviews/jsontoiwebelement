# JsonToIWebElement
Library to return IWebelement from JSON file

### Test Url
www.google.com

### Sample Defnition File

#### Example 1 - Element definition from another definition file

```json
[
    {
	    "name": "SearchBox",
	    "how": "file",
	    "definition": "TestElements.json"
    }
]
```

#### Example 2 - Element definition defined in the same file

```json
[
    {
	    "name": "SearchBox",
	    "how": "name",
	    "definition": "q"
    }
]
```

- **name**: Any custom name to refer to this element
- **how**: Two variations are available to define "how". This means how to find the element. Variations are described below:  
  - Selenium [By](https://seleniumhq.github.io/selenium/docs/api/java/org/openqa/selenium/By.html) properties to find this element.
  - Use _file_ instead of By properties if element definition requires to be read from any external JSON definition file (Please refer example 1 above). 

- **definition**: Also available two variations depending on the value defined for _how_. Variations are as follows:

    - If _how_ defined as By property, then any valid defnition to look for the element. (Please refer to Example 2 above).  
    - If _how_ defined as _file_ then mentioned the file name (please refer to Example 1 above).

## Example usage
```csharp
namespace SampleNameSpace
{
    using FluentAssertions;
    using OpenQA.Selenium;
    using Ridia.TestAutomation;

    public class SamplePage
    {
        private JsonToIWebElement jsonToIWebElement;
        public JsonToIWebElement JsonToIWebElement { get { return this.jsonToIWebElement; } set { this.jsonToIWebElement = value; } }
        
        public SamplePage(IWebDriver driver)
        {
            JsonToIWebElement = new JsonToIWebElement("DefinitionFile.json", driver);
        }
        
        [Fact]
        public void ShouldReturnElementFromJson()
        {
            ChromeDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.google.com");
            IWebElement element = JsonToIWebElement.GetElement("SearchBox");
            element.SendKeys("test with xunit");
        }
}
```
