/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.DebugHelpers
{
    /// <summary>
    /// Provides methods for helping in generating debug string image.
    /// </summary>
    public interface IObjectToBriefString
    {
        /// <summary>
        /// Returns a string that represents the current instance in very short way.
        /// </summary>
        /// <returns>A string that represents the current instance in very short way.</returns>
        string ToBriefString();

        /// <summary>
        /// Returns a string that represents the current instance in very short way.
        /// </summary>
        /// <param name="n">Count of spaces in the string for more comfortable representation.</param>
        /// <returns>A string that represents the current instance in very short way.</returns>
        string ToBriefString(uint n);

        /// <summary>
        /// Internal method which returns a string that represents the current instance in very short way without additional information, only pair name of property - value.
        /// </summary>
        /// <param name="n">Count of spaces in the string for more comfortable representation.</param>
        /// <returns>A string that represents the current instance in very short way without additional information, only pair name of property - value.</returns>
        string PropertiesToBriefString(uint n);
    }
}

/*
        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            return sb.ToString();
        }
*/
