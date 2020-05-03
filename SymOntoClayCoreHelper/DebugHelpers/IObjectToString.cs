using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.DebugHelpers
{
    /// <summary>
    /// Provides methods for helping in generating debug string image.
    /// </summary>
    public interface IObjectToString
    {
        /// <summary>
        /// Returns a string that represents the current instance.
        /// </summary>
        /// <param name="n">Count of spaces in the string for more comfortable representation.</param>
        /// <returns>A string that represents the current instance.</returns>
        string ToString(uint n);

        /// <summary>
        /// Internal method which returns a string that represents the current instance without additional information, only pair name of property - value.
        /// </summary>
        /// <param name="n">Count of spaces in the string for more comfortable representation.</param>
        /// <returns>A string that represents the current instance without additional information, only pair name of property - value.</returns>
        string PropertiesToString(uint n);
    }
}

/*
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
            return sb.ToString();
        }
*/
