using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.DebugHelpers
{
    /// <summary>
    /// Provides methods for helping in generating debug string image.
    /// </summary>
    public interface IObjectToShortString
    {
        /// <summary>
        /// Returns a string that represents the current instance in short way.
        /// </summary>
        /// <returns>A string that represents the current instance in short way.</returns>
        string ToShortString();

        /// <summary>
        /// Returns a string that represents the current instance in short way.
        /// </summary>
        /// <param name="n">Count of spaces in the string for more comfortable representation.</param>
        /// <returns>A string that represents the current instance in short way.</returns>
        string ToShortString(uint n);

        /// <summary>
        /// Internal method which returns a string that represents the current instance in short way without additional information, only pair name of property - value.
        /// </summary>
        /// <param name="n">Count of spaces in the string for more comfortable representation.</param>
        /// <returns>A string that represents the current instance in short way without additional information, only pair name of property - value.</returns>
        string PropertiesToShortString(uint n);
    }
}

/*
        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            return sb.ToString();
        }
*/
