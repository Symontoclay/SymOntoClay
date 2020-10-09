/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class WeightedInheritanceItem: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public WeightedInheritanceItem()
        {
        }

        public WeightedInheritanceItem(WeightedInheritanceItem source)
        {
            IsSelf = source.IsSelf;
            Distance = source.Distance;
            SuperNameKey = source.SuperNameKey;
            Rank = source.Rank;
            OriginalIndexedItem = source.OriginalIndexedItem;
        }

        public bool IsSelf { get; set; }
        public uint Distance { get; set; }
        public ulong SuperNameKey { get; set; }
        public float Rank { get; set; }
        public IndexedInheritanceItem OriginalIndexedItem { get; set; }

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
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsSelf)} = {IsSelf}");
            sb.AppendLine($"{spaces}{nameof(Distance)} = {Distance}");
            sb.AppendLine($"{spaces}{nameof(SuperNameKey)} = {SuperNameKey}");
           
            sb.AppendLine($"{spaces}{nameof(Rank)} = {Rank}");

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
            return PropertiesToShortString(n);
        }

        protected virtual string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsSelf)} = {IsSelf}");
            sb.AppendLine($"{spaces}{nameof(Distance)} = {Distance}");

            sb.AppendLine($"{spaces}{nameof(SuperNameKey)} = {SuperNameKey}");

            sb.AppendLine($"{spaces}{nameof(Rank)} = {Rank}");

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
            return PropertiesToBriefString(n);
        }

        protected virtual string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsSelf)} = {IsSelf}");
            sb.AppendLine($"{spaces}{nameof(Distance)} = {Distance}");

            sb.AppendLine($"{spaces}{nameof(SuperNameKey)} = {SuperNameKey}");

            sb.AppendLine($"{spaces}{nameof(Rank)} = {Rank}");

            return sb.ToString();
        }
    }
}
