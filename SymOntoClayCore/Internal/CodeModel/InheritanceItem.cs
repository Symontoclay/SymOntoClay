using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class InheritanceItem: AnnotatedItem
    {
        public InheritanceItem(ICodeModelContext context)
            : base(context)
        {
            Name = new Name(context);
        }

        public Name Name { get; set; }
        public Value Range { get; set; }
        public bool IsSystemDefined { get; set; }

        private void PrintHeader(StringBuilder sb, uint n, string spaces)
        {
            sb.PrintObjProp(n, nameof(Name), Name);
            sb.PrintObjProp(n, nameof(Range), Range);
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
