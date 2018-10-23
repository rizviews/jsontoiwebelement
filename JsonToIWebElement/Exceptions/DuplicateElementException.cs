using System;
using System.Collections.Generic;
using System.Linq;

namespace Ridia.TestAutomation.Exceptions
{
    /// <summary>
    /// The exception that is thrown when duplicate element definitions are found.
    /// </summary>
    [Serializable]
    public class DuplicateElementException : Exception
    {
        private const string RawErrorMessage = "Element names must be unique - the following elements appear more than once: {0}";

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateElementException"/> class.
        /// </summary>
        /// <param name="elementName">The name of the duplicated element.</param>
        public DuplicateElementException(string elementName)
            : this(new string[] { elementName })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateElementException"/> class.
        /// </summary>
        /// <param name="elementNames">The names of the duplicated elements.</param>
        public DuplicateElementException(IEnumerable<string> elementNames)
            : base(string.Format(RawErrorMessage, string.Join(",", elementNames)))
        {
            this.ElementNames = elementNames.ToArray();
        }

        /// <summary>
        /// Gets the names of the duplicate elements.
        /// </summary>
        public string[] ElementNames { get; private set; }
    }
}
