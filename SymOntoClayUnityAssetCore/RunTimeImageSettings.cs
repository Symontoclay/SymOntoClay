using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// Settings for creation (saving) runtime image of executed code.
    /// </summary>
    public class RunTimeImageSettings: IObjectToString
    {
        /// <summary>
        /// Gets or sets user defined description of the image.
        /// </summary>
        public string Description { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(Description)} = {Description}");

            return sb.ToString();
        }
    }
}
