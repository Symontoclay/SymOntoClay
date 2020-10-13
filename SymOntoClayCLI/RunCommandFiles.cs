/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CLI
{
    public class RunCommandFiles : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public string LogicFile { get; set; }
        public string WorldFile { get; set; }
        public string SharedModulesDir { get; set; }
        public string ImagesRootDir { get; set; }
        public string TmpDir { get; set; }

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
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(LogicFile)} = {LogicFile}");
            sb.AppendLine($"{spaces}{nameof(WorldFile)} = {WorldFile}");
            sb.AppendLine($"{spaces}{nameof(SharedModulesDir)} = {SharedModulesDir}");
            sb.AppendLine($"{spaces}{nameof(ImagesRootDir)} = {ImagesRootDir}");
            sb.AppendLine($"{spaces}{nameof(TmpDir)} = {TmpDir}");

            return sb.ToString();
        }

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

            sb.AppendLine($"{spaces}{nameof(LogicFile)} = {LogicFile}");
            sb.AppendLine($"{spaces}{nameof(WorldFile)} = {WorldFile}");

            return sb.ToString();
        }

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

            sb.AppendLine($"{spaces}{nameof(LogicFile)} = {LogicFile}");
            sb.AppendLine($"{spaces}{nameof(WorldFile)} = {WorldFile}");

            return sb.ToString();
        }
    }
}
