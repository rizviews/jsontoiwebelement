namespace Ridia.TestAutomation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Newtonsoft.Json;
    using OpenQA.Selenium;
    using Ridia.TestAutomation.Exceptions;
    using Ridia.TestAutomation.Model;

    /// <summary>
    /// Provides functionality to get <see cref="IWebElement"/>
    /// or <see cref="List{IWebElement}"/> from JSON definition
    /// </summary>
    public class JsonToIWebElement
    {
        private List<PageElementModel> pageElements;
        private IWebDriver driver;
        private string basePath = string.Empty;

        private List<PageElementModel> PageElements
        {
            get
            {
                return this.pageElements;
            }
            set
            {
                this.pageElements = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonToIWebElement" /> class.
        /// Default empty constructor
        /// </summary>
        public JsonToIWebElement()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonToIWebElement" /> class.
        /// </summary>
        /// <param name="definitionFileName">Name of the file containing JSON definition</param>
        /// <param name="driver">Instance of <see cref="IWebDriver" /></param>
        /// <param name="basePath">An optional parameter to hold base directory</param>
        public JsonToIWebElement(string definitionFileName, IWebDriver driver, string basePath = "")
        {
            this.PageElements = JsonConvert.DeserializeObject<List<PageElementModel>>(File.ReadAllText(definitionFileName));

            if (!string.IsNullOrEmpty(basePath))
            {
                this.basePath = basePath;
            }

            this.ResolveElementsFromFile();

            this.driver = driver;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonToIWebElement"/> class.
        /// </summary>
        /// <param name="definitionFile">Either a definition file name or the relative path of definition files</param>
        public JsonToIWebElement(string definitionFile)
        {
            this.PageElements = new List<PageElementModel>();

            if (File.Exists(definitionFile))
            {
                this.PageElements = JsonConvert.DeserializeObject<List<PageElementModel>>(File.ReadAllText(definitionFile));
                this.ValidateUniqueElementNames(this.PageElements);
                this.ResolveElementsFromFile();
            }
            else if(Directory.Exists(definitionFile))
            {
                string[] fileNames = Directory.GetFiles(definitionFile);

                foreach (string fileName in fileNames)
                {
                    this.PageElements.AddRange(JsonConvert.DeserializeObject<List<PageElementModel>>(File.ReadAllText(fileName)));
                }

                this.ValidateUniqueElementNames(this.PageElements);
            }
        }

        /// <summary>
        /// Reload updated definition from given <paramref name="definitionFileName"/> />
        /// </summary>
        /// <param name="definitionFileName">Definition file name to reload from</param>
        public void ReloadElementDefinition(string definitionFileName)
        {
            this.PageElements = JsonConvert.DeserializeObject<List<PageElementModel>>(File.ReadAllText(definitionFileName));
            this.ResolveElementsFromFile();
        }

        /// <summary>
        /// Returns <see cref="IWebElement" /> matching the <see cref="elementName" />/&gt;
        /// </summary>
        /// <param name="elementName">Name of the element defined in the definition file</param>
        /// <param name="webDriver">Instance of the <see cref="IWebDriver" /> to find element</param>
        /// <param name="elementValue">The element value.</param>
        /// <returns>Returns an instance of IWebElement</returns>
        /// <exception cref="ElementNotFoundException"></exception>
        public IWebElement GetElement(string elementName, IWebDriver webDriver = null, string elementValue = "")
        {
            PageElementModel model = this.PageElements.Find(item => item.Name == elementName);
            if (model == null)
            {
                throw new ElementNotFoundException(elementName);
            }

            string definition = model.Definition;

            if (!string.IsNullOrEmpty(elementValue))
            {
                definition = this.ReplaceTokenInElementDefinition(model.Definition, elementValue);
            }

            Type type = typeof(By);
            
            MethodInfo methodInfo = type.GetMethod(model.How);

            if (webDriver != null)
            {
                return webDriver.FindElement((By)methodInfo.Invoke(null, new object[] { definition }));
            }

            return this.driver.FindElement((By)methodInfo.Invoke(null, new object[] { definition }));
        }

        /// <summary>
        /// Method to get all <see cref="IWebElement"/> as list
        /// </summary>
        /// <param name="definitionFileName">Name of the JSON file containing definition of all elements</param>
        /// <returns>Returns all <see cref="IWebElement"/> as a <see cref="List{IWebElement}"/>/></returns>
        public List<IWebElement> GetAllElements(string definitionFileName = "")
        {
            if (!string.IsNullOrEmpty(definitionFileName))
            {
                this.PageElements = JsonConvert.DeserializeObject<List<PageElementModel>>(File.ReadAllText(definitionFileName));
            }
            List<IWebElement> elements = new List<IWebElement>();

            foreach (PageElementModel element in this.PageElements)
            {
                elements.Add(this.GetElement(element.Name));
            }

            return elements;
        }

        /// <summary>
        /// Method to get definition of the element
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <returns>Returns string</returns>
        /// <exception cref="ElementNotFoundException"></exception>
        public string GetDefinition(string elementName)
        {
            PageElementModel model = this.PageElements.Find(item => item.Name == elementName);
            if (model == null)
            {
                throw new ElementNotFoundException(elementName);
            }

            return model.Definition;
        }

        /// <summary>
        /// Method to get <see cref="By"/> locator of the element 
        /// </summary>
        /// <param name="elementName">Name of the element</param>
        /// <returns><see cref="By"/> locator to find this element</returns>
        public By GetByLocator(string elementName, string elementValue = "")
        {
            PageElementModel model = this.PageElements.Find(item => item.Name == elementName);
            if (model == null)
            {
                throw new ElementNotFoundException(elementName);
            }

            string definition = model.Definition;

            if (!string.IsNullOrEmpty(elementValue))
            {
                definition = this.ReplaceTokenInElementDefinition(model.Definition, elementValue);
            }
            Type type = typeof(By);
            MethodInfo methodInfo = type.GetMethod(model.How);

            return (By)methodInfo.Invoke(null, new object[] { definition });
        }

        /// <summary>
        /// Sets the driver.
        /// </summary>
        /// <param name="driver">The driver.</param>
        public void SetDriver(IWebDriver driver)
        {
            this.driver = driver;
        }

        private void ResolveElementsFromFile()
        {
            for (int i = 0; i < this.PageElements.Count; i++)
            {
                PageElementModel model = this.PageElements[i];

                if (model.How == "file")
                {
                    string filePath = string.Empty;

                    if (string.IsNullOrEmpty(this.basePath))
                    {
                        filePath = model.Definition;
                    }
                    else
                    {
                        filePath = System.IO.Path.Combine(this.basePath, model.Definition);
                    }
                    List<PageElementModel> models = JsonConvert.DeserializeObject<List<PageElementModel>>(File.ReadAllText(filePath));
                    this.PageElements[i] = models.Find(item => item.Name == model.Name);
                }
            }
        }

        private string ReplaceTokenInElementDefinition(string originalDefinition, string replaceWith)
        {
            int lastIndex = originalDefinition.LastIndexOf("}");
            int firstIndex = originalDefinition.IndexOf("{");

            string token = originalDefinition.Substring(firstIndex, (lastIndex - firstIndex) + 1);

            return originalDefinition.Replace(token, replaceWith);
            
        }

        private void ValidateUniqueElementNames(IEnumerable<PageElementModel> elements)
        {
            var duplicates = elements.GroupBy(x => x.Name).Where(x => x.Count() > 1);
            if (duplicates != null && duplicates.Any())
            {
                throw new DuplicateElementException(duplicates.Select(x => x.Key));
            }
        }
    }
}
