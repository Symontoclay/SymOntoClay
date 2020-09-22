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
