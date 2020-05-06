using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public class ModulesStorageSettings : IObjectToString
    {
        /// <summary>
        /// Gets or sets reference to logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Gets or ses reference to shared dictionary.
        /// </summary>
        public IDictionary Dictionary { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.PrintExisting(n, nameof(Logger), Logger);
            sb.PrintExisting(n, nameof(Dictionary), Dictionary);
            return sb.ToString();
        }
    }
}
