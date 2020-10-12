/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// General settings of a game world.
    /// </summary>
    public class WorldSettings: IObjectToString
    {
        /// <summary>
        /// Gets or sets list of file paths for describing places for searching shared modules.
        /// </summary>
        public IList<string> SharedModulesDirs { get; set; }

        /// <summary>
        /// Gets or sets root dir for saving and loading images of executed code.
        /// </summary>
        public string ImagesRootDir { get; set; }

        /// <summary>
        /// Gets or sets list of file paths for describing places searching dictionaries of translation text to facts.
        /// </summary>
        public IList<string> DictionariesDirs { get; set; }

        public string TmpDir { get; set; }

        /// <summary>
        /// Gets or sets logging settings.
        /// </summary>
        public LoggingSettings Logging { get; set; }

        /// <summary>
        /// Gets or sets file name of SymOntoClay host file.
        /// The file describes facts which are visible for other NPCs or can be recognized in some way by player.
        /// </summary>
        public string HostFile { get; set; }

        public IInvokerInMainThread InvokerInMainThread { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var nextNSpaces = DisplayHelper.Spaces(nextN);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Logging), Logging);
            sb.PrintPODList(n, nameof(SharedModulesDirs), SharedModulesDirs);
            sb.AppendLine($"{spaces}{nameof(ImagesRootDir)} = {ImagesRootDir}");
            sb.PrintPODList(n, nameof(DictionariesDirs), DictionariesDirs);
            sb.AppendLine($"{spaces}{nameof(TmpDir)} = {TmpDir}");
            sb.AppendLine($"{spaces}{nameof(HostFile)} = {HostFile}");
            sb.PrintExisting(n, nameof(InvokerInMainThread), InvokerInMainThread);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }
    }
}
