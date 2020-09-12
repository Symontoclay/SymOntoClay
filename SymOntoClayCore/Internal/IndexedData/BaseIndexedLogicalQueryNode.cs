using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public abstract class BaseIndexedLogicalQueryNode : IndexedAnnotatedItem
    {
        public abstract KindOfLogicalQueryNode Kind { get; }
        public virtual KindOfOperatorOfLogicalQueryNode KindOfOperator => KindOfOperatorOfLogicalQueryNode.Unknown;

        public LogicalQueryNode Origin { get; set; }
        public IndexedRuleInstance RuleInstance { get; set; }
        public IndexedBaseRulePart RulePart { get; set; }

        public virtual QuestionVarIndexedLogicalQueryNode AsQuestionVarIndexedLogicalQueryNode => null;

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");
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
            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        public string GetHumanizeDbgString()
        {
            if (Origin == null)
            {
                return string.Empty;
            }

            return DebugHelperForRuleInstance.ToString(Origin);
        }

        public abstract void FillExecutingCard(QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options);
    }
}
