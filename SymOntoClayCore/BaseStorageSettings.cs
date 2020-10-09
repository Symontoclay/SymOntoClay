/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public abstract class BaseStorageSettings: BaseCoreSettings
    {
        /// <summary>
        /// Gets or sets unique Id.
        /// It allows us to identify each item of the game.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets app file.
        /// </summary>
        public string AppFile { get; set; }

        /// <summary>
        /// Gets or ses reference to shared modules storage.
        /// </summary>
        public IModulesStorage ModulesStorage { get; set; }

        /// <summary>
        /// Gets or sets parent storage.
        /// </summary>
        public IStandaloneStorage ParentStorage { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Id)} = {Id}");
            sb.AppendLine($"{spaces}{nameof(AppFile)} = {AppFile}");

            sb.PrintExisting(n, nameof(ModulesStorage), ModulesStorage);
            sb.PrintExisting(n, nameof(ParentStorage), ParentStorage);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }
    }
}
