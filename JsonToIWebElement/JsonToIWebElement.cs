namespace Ridia.TestAutomation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Newtonsoft.Json;
    using OpenQA.Selenium;
    using Ridia.TestAutomation.Model;

    /// <summary>
    /// Provides functionality to get <see cref="IWebElement"/>
    /// or <see cref="List{IWebElement}"/> from JSON definition
    /// </summary>
    public class JsonToIWebElement
    {
        private List<PageElementModel> pageElements;
        private IWebDriver driver;

        private List<PageElementModel> PageElements { get { return this.pageElements; } set { this.pageElements = value; } }

        /// <summary>
        /// Instrantiate an instance of <see cref="JsonToIWebElement"/>
        /// </summary>
        /// <param name="definitionFileName">Name of the file containing JSON definition</param>
        /// <param name="driver">Intance of <see cref="IWebDriver"/></param>
        public JsonToIWebElement(string definitionFileName, IWebDriver driver)
        {
            PageElements = JsonConvert.DeserializeObject<List<PageElementModel>>(File.ReadAllText(definitionFileName));
            this.driver = driver;
        }

        /// <summary>
        /// Returns <see cref="IWebElement"/> matching the <see cref="elementName"/>/>
        /// </summary>
        /// <param name="elementName">Name of the element defined in the definition file</param>
        /// <param name="webDriver">Instance of the <see cref="IWebDriver"/> to find element</param>
        /// <returns></returns>
        public IWebElement GetElement (string elementName, IWebDriver webDriver = null)
        {
            PageElementModel model = PageElements.Find(item => item.Name == elementName);

            if (model.How == "file")
            {
                List<PageElementModel> models = JsonConvert.DeserializeObject<List<PageElementModel>>(File.ReadAllText(model.Definition));
                model = models.Find(item => item.Name == elementName);
            }

            Type type = typeof(By);
            MethodInfo methodInfo = type.GetMethod(model.How);

            if (webDriver != null)
            {
                return webDriver.FindElement((By)methodInfo.Invoke(null, new object[] { model.Definition }));
            }
            return driver.FindElement((By)methodInfo.Invoke(null, new object[] { model.Definition }));
        }

        /// <summary>
        /// Method to get all <see cref="IWebElement"/> as list
        /// </summary>
        /// <param name="definitionFileName">Name of the JSON file containing definition of all elements</param>
        /// <returns>Returns all <see cref="IWebElement"/> as a <see cref="List{IWebElement}"/>/></returns>
        public List<IWebElement> GetAllElements(string definitionFileName = "")
        {
            if (definitionFileName != "")
            {
                PageElements = JsonConvert.DeserializeObject<List<PageElementModel>>(File.ReadAllText(definitionFileName));
            }
            List<IWebElement> elements = new List<IWebElement>();

            foreach (PageElementModel element in PageElements)
            {
                elements.Add(this.GetElement(element.Name));
            }

            return elements;
        }
    }
}
