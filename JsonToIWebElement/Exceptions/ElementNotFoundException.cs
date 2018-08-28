using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ridia.TestAutomation.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an element cannot be found.
    /// </summary>
    [Serializable]
    public class ElementNotFoundException : Exception
    {
        private const string RawErrorMessage = "Element with name '{0}' cannot be found";

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementNotFoundException"/> class.
        /// </summary>
        /// <param name="elementName">The name of the element which cannot be found.</param>
        public ElementNotFoundException(string elementName)
            : base(string.Format(RawErrorMessage, elementName))
        {
            this.ElementName = elementName;
        }

        /// <summary>
        /// Gets the name of the element which cannot be found.
        /// </summary>
        public string ElementName { get; private set; }
    }
}
