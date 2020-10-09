/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.Logging
{
    public class CoreLoggerSettings : IObjectToString
    {
        /// <summary>
        /// Gets or sets root dir for file logging.
        /// </summary>
        public string LogDir { get; set; }

        /// <summary>
        /// Gets or sets list of platform specific loggers.
        /// It alows us to add, for example, console logger for Unity3D.
        /// </summary>
        public IList<IPlatformLogger> PlatformLoggers { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(LogDir)} = {LogDir}");
            var platformLoggersMark = PlatformLoggers == null ? "No" : PlatformLoggers.Any() ? "Yes" : "No";
            sb.AppendLine($"{spaces}{nameof(PlatformLoggers)} = {platformLoggersMark}");
            return sb.ToString();
        }
    }
}
