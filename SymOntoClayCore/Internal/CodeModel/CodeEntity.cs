﻿using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class CodeEntity : AnnotatedItem
    {
        public KindOfCodeEntity Kind { get; set; } = KindOfCodeEntity.Unknown;
        public Name Name { get; set; } = new Name();
        public List<InheritanceItem> InheritanceItems { get; set; } = new List<InheritanceItem>();
        public RuleInstance RuleInstance { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.PrintObjProp(n, nameof(Name), Name);
            sb.PrintObjListProp(n, nameof(InheritanceItems), InheritanceItems);
            sb.PrintObjProp(n, nameof(RuleInstance), RuleInstance);
            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
            sb.PrintShortObjListProp(n, nameof(InheritanceItems), InheritanceItems);
            sb.PrintShortObjProp(n, nameof(RuleInstance), RuleInstance);
            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
            sb.PrintBriefObjListProp(n, nameof(InheritanceItems), InheritanceItems);
            sb.PrintBriefObjProp(n, nameof(RuleInstance), RuleInstance);
            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
