﻿using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedInheritanceItem: IndexedAnnotatedItem
    {
        public InheritanceItem OriginalInheritanceItem { get; set; }

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

        private void PrintHeader(StringBuilder sb, uint n, string spaces)
        {
            sb.PrintBriefObjProp(n, nameof(OriginalInheritanceItem), OriginalInheritanceItem);
            sb.PrintObjProp(n, nameof(SubName), SubName);
            sb.PrintObjProp(n, nameof(SuperName), SuperName);
            sb.PrintObjProp(n, nameof(Rank), Rank);
            sb.AppendLine($"{spaces}{nameof(IsSystemDefined)} = {IsSystemDefined}");
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
