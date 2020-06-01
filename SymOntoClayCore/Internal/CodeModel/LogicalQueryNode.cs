using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class LogicalQueryNode: AnnotatedItem
    {
        public KindOfLogicalQueryNode Kind { get; set; } = KindOfLogicalQueryNode.Unknown;
        public KindOfOperatorOfLogicalQueryNode KindOfOperator { get; set; } = KindOfOperatorOfLogicalQueryNode.Unknown;
        public Name Name { get; set; }
        public LogicalQueryNode Left { get; set; }
        public LogicalQueryNode Right { get; set; }
        public List<LogicalQueryNode> ParamsList { get; set; }
        public List<Name> VarsList { get; set; }
        public bool IsGroup { get; set; }
        public Value Value { get; set; }
        public bool IsQuestion { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintObjProp(n, nameof(Name), Name);

            sb.PrintObjProp(n, nameof(Left), Left);
            sb.PrintObjProp(n, nameof(Right), Right);
            sb.PrintObjListProp(n, nameof(ParamsList), ParamsList);
            sb.PrintObjListProp(n, nameof(VarsList), VarsList);

            sb.AppendLine($"{spaces}{nameof(IsGroup)} = {IsGroup}");

            sb.PrintObjProp(n, nameof(Value), Value);

            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.PrintShortObjProp(n, nameof(Left), Left);
            sb.PrintShortObjProp(n, nameof(Right), Right);
            sb.PrintShortObjListProp(n, nameof(ParamsList), ParamsList);

            sb.PrintShortObjListProp(n, nameof(VarsList), VarsList);

            sb.AppendLine($"{spaces}{nameof(IsGroup)} = {IsGroup}");

            sb.PrintShortObjProp(n, nameof(Value), Value);

            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintBriefObjProp(n, nameof(Name), Name);

            sb.PrintExisting(n, nameof(Left), Left);
            sb.PrintExisting(n, nameof(Right), Right);
            sb.PrintExistingList(n, nameof(ParamsList), ParamsList);

            sb.PrintExistingList(n, nameof(VarsList), VarsList);

            sb.AppendLine($"{spaces}{nameof(IsGroup)} = {IsGroup}");

            sb.PrintBriefObjProp(n, nameof(Value), Value);

            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
