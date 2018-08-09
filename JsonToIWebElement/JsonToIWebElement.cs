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

        private List<PageElementModel> PageElements { get { return this.pageElements; } set { this.pageElements = value; } }

        /// <summary>
        /// Default empty constructor
        /// </summary>
        public JsonToIWebElement(){}

       
        /// <summary>
        /// Instantiate an instance of <see cref="JsonToIWebElement"/>
        /// </summary>
        /// <param name="definitionFileName">Name of the file containing JSON definition</param>
        /// <param name="driver">Intance of <see cref="IWebDriver"/></param>
        /// <param name="basePath">An optional parameter to hold base directory</param>
        public JsonToIWebElement(string definitionFileName, IWebDriver driver,string basePath = "")
        {
            PageElements = JsonConvert.DeserializeObject<List<PageElementModel>>(File.ReadAllText(definitionFileName));

            if (!string.IsNullOrEmpty(basePath))
            {
                this.basePath = basePath;
            }

            this.ResolveElementsFromFile();

            this.driver = driver;
        }

        /// <summary>
        /// Instantiate an instance of <see cref="JsonToIWebElement"/>
        /// </summary>
        /// <param name="definitionFile">Either a definition file name or the relative path of definition files</param>
        public JsonToIWebElement(string definitionFile)
        {
            PageElements = new List<PageElementModel>();

            if (File.Exists(definitionFile))
            {
                PageElements = JsonConvert.DeserializeObject<List<PageElementModel>>(File.ReadAllText(definitionFile));
                this.ValidateUniqueElementNames(PageElements);
                this.ResolveElementsFromFile();
            }
            else if(Directory.Exists(definitionFile))
            {
                string[] fileNames = Directory.GetFiles(definitionFile);

                foreach (string fileName in fileNames)
                {
                    PageElements.AddRange(JsonConvert.DeserializeObject<List<PageElementModel>>(File.ReadAllText(fileName)));
                }

                this.ValidateUniqueElementNames(PageElements);
            }
        }

        private void ResolveElementsFromFile()
        {
            for (int i=0; i<PageElements.Count;i++)
            {
                PageElementModel model = PageElements[i];

                if (model.How == "file")
                {
                    string filePath = string.Empty;

                    if (string.IsNullOrEmpty(this.basePath))
                    {
                        filePath = model.Definition;
                    }
                    else
                    {
                        filePath = System.IO.Path.Combine(basePath, model.Definition);
                    }

                    List<PageElementModel> models = JsonConvert.DeserializeObject<List<PageElementModel>>(File.ReadAllText(filePath));
                    PageElements[i] = models.Find(item => item.Name == model.Name);
                }
            }
        }

        /// <summary>
        /// Reload updated definition from given <paramref name="definitionFileName"/> />
        /// </summary>
        /// <param name="definitionFileName">Definition file name to reload from</param>
        public void ReloadElementDefinition(string definitionFileName)
        {
            PageElements = JsonConvert.DeserializeObject<List<PageElementModel>>(File.ReadAllText(definitionFileName));
            this.ResolveElementsFromFile();
        }

        /// <summary>
        /// Returns <see cref="IWebElement"/> matching the <see cref="elementName"/>/>
        /// </summary>
        /// <param name="elementName">Name of the element defined in the definition file</param>
        /// <param name="webDriver">Instance of the <see cref="IWebDriver"/> to find element</param>
        /// <returns></returns>
        public IWebElement GetElement (string elementName, IWebDriver webDriver = null, string elementValue = "")
        {
            PageElementModel model = PageElements.Find(item => item.Name == elementName);
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

        /// <summary>
        /// Method to get definition of the element
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public string GetDefinition(string elementName)
        {
            PageElementModel model = PageElements.Find(item => item.Name == elementName);
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
            PageElementModel model = PageElements.Find(item => item.Name == elementName);
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

        public void SetDriver(IWebDriver driver)
        {
            this.driver = driver;
        }
    }
}
