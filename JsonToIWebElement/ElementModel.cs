using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ridia.TestAutomation
{
    public class ElementModel
    {
        public IWebElement Element { get; set; }
        public By Locator { get; set; }
    }
}
