/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// Biped NPC (Non-Player Character) setting.
    /// </summary>
    public class HumanoidNPCSettings: BaseManualControllingGameComponentSettings
    {
        /// <summary>
        /// Gets or sets file name of SymOntoClay logic file.
        /// The file describes active logic which will be executed on the NPC.
        /// </summary>
        public string LogicFile { get; set; }

        public IPlatformSupport PlatformSupport { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(LogicFile)} = {LogicFile}");

            sb.PrintExisting(n, nameof(PlatformSupport), PlatformSupport);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }
    }
}