using SymOntoClay.CoreHelper.DebugHelpers;
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
        public InlineTrigger InlineTrigger { get; set; }
        public Operator Operator { get; set; }
        public Channel Channel { get; set; }

        public CodeFile CodeFile { get; set; }
        public CodeEntity ParentCodeEntity { get; set; }
        public List<CodeEntity> SubItems { get; set; } = new List<CodeEntity>();

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.PrintObjProp(n, nameof(Name), Name);

            sb.PrintObjListProp(n, nameof(InheritanceItems), InheritanceItems);
            sb.PrintObjProp(n, nameof(RuleInstance), RuleInstance);
            sb.PrintObjProp(n, nameof(InlineTrigger), InlineTrigger);
            sb.PrintObjProp(n, nameof(Operator), Operator);
            sb.PrintObjProp(n, nameof(Channel), Channel);

            sb.PrintBriefObjProp(n, nameof(CodeFile), CodeFile);
            sb.PrintBriefObjProp(n, nameof(ParentCodeEntity), ParentCodeEntity);
            sb.PrintObjListProp(n, nameof(SubItems), SubItems);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.PrintShortObjListProp(n, nameof(InheritanceItems), InheritanceItems);
            sb.PrintShortObjProp(n, nameof(RuleInstance), RuleInstance);
            sb.PrintShortObjProp(n, nameof(InlineTrigger), InlineTrigger);
            sb.PrintShortObjProp(n, nameof(Operator), Operator);
            sb.PrintShortObjProp(n, nameof(Channel), Channel);

            sb.PrintBriefObjProp(n, nameof(CodeFile), CodeFile);
            sb.PrintBriefObjProp(n, nameof(ParentCodeEntity), ParentCodeEntity);
            sb.PrintShortObjListProp(n, nameof(SubItems), SubItems);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.PrintBriefObjProp(n, nameof(Name), Name);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
