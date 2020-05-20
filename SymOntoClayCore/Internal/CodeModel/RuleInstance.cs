using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class RuleInstance: AnnotatedItem
    {
        public PrimaryRulePart PrimaryPart { get; set; }

        private void PrintHeader(StringBuilder sb, uint n, string spaces)
        {

        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            PrintHeader(sb, n, spaces);
            sb.PrintObjProp(n, nameof(PrimaryPart), PrimaryPart);
            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            PrintHeader(sb, n, spaces);
            sb.PrintShortObjProp(n, nameof(PrimaryPart), PrimaryPart);
            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            PrintHeader(sb, n, spaces);
            sb.PrintBriefObjProp(n, nameof(PrimaryPart), PrimaryPart);
            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
