/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedInheritanceItem: IndexedAnnotatedItem
    {
        public InheritanceItem OriginalInheritanceItem { get; set; }

        /// <inheritdoc/>
        public override AnnotatedItem OriginalAnnotatedItem => OriginalInheritanceItem;

        public ulong Key { get; set; }

        public IndexedStrongIdentifierValue SubName { get; set; } = new IndexedStrongIdentifierValue();

        /// <summary>
        /// Represents ancestor.
        /// </summary>
        public IndexedStrongIdentifierValue SuperName { get; set; } = new IndexedStrongIdentifierValue();

        /// <summary>
        /// Represents rank of inheritance between two objects.
        /// It must be resolved to LogicalValue.
        /// </summary>
        [ResolveToType(typeof(IndexedLogicalValue))]
        public IndexedValue Rank { get; set; }
        public bool IsSystemDefined { get; set; }

        public IList<ulong> KeysOfPrimaryRecords { get; set; } = new List<ulong>();

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            var result = base.CalculateLongHashCode() ^ SubName.GetLongHashCode() ^ SuperName.GetLongHashCode();

            if(Rank != null)
            {
                result ^= LongHashCodeWeights.BaseModalityWeight ^ Rank.GetLongHashCode();
            }

            return result;
        }

        private void PrintHeader(StringBuilder sb, uint n, string spaces)
        {
            sb.PrintExisting(n, nameof(OriginalInheritanceItem), OriginalInheritanceItem);
            sb.AppendLine($"{spaces}{nameof(Key)} = {Key}");
            sb.PrintObjProp(n, nameof(SubName), SubName);
            sb.PrintObjProp(n, nameof(SuperName), SuperName);
            sb.PrintObjProp(n, nameof(Rank), Rank);
            sb.AppendLine($"{spaces}{nameof(IsSystemDefined)} = {IsSystemDefined}");
            sb.PrintPODList(n, nameof(KeysOfPrimaryRecords), KeysOfPrimaryRecords);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            PrintHeader(sb, n, spaces);
            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            PrintHeader(sb, n, spaces);
            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            PrintHeader(sb, n, spaces);
            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
