using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class QueryExecutingCardForIndexedPersistLogicalData : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public ulong TargetRelation { get; set; }
        public int CountParams { get; set; }
        public IList<QueryExecutingCardAboutVar> VarsInfoList { get; set; }
        public IList<QueryExecutingCardAboutKnownInfo> KnownInfoList { get; set; }
        public IList<ResultOfQueryToRelation> ResultsOfQueryToRelationList { get; set; } = new List<ResultOfQueryToRelation>();
        public IndexedRuleInstance SenderIndexedRuleInstance { get; set; }
        public IndexedBaseRulePart SenderIndexedRulePart { get; set; }
        public BaseIndexedLogicalQueryNode SenderExpressionNode { get; set; }

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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(TargetRelation)} = {TargetRelation}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");
            sb.PrintObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintObjListProp(n, nameof(KnownInfoList), KnownInfoList);
            sb.PrintObjListProp(n, nameof(ResultsOfQueryToRelationList), ResultsOfQueryToRelationList);
            sb.PrintExisting(n, nameof(SenderIndexedRuleInstance), SenderIndexedRuleInstance);
            sb.PrintExisting(n, nameof(SenderIndexedRulePart), SenderIndexedRulePart);
            sb.PrintExisting(n, nameof(SenderExpressionNode), SenderExpressionNode);

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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(TargetRelation)} = {TargetRelation}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");
            sb.PrintShortObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintShortObjListProp(n, nameof(KnownInfoList), KnownInfoList);
            sb.PrintShortObjListProp(n, nameof(ResultsOfQueryToRelationList), ResultsOfQueryToRelationList);
            sb.PrintExisting(n, nameof(SenderIndexedRuleInstance), SenderIndexedRuleInstance);
            sb.PrintExisting(n, nameof(SenderIndexedRulePart), SenderIndexedRulePart);
            sb.PrintExisting(n, nameof(SenderExpressionNode), SenderExpressionNode);

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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(TargetRelation)} = {TargetRelation}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");
            sb.PrintBriefObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintBriefObjListProp(n, nameof(KnownInfoList), KnownInfoList);
            sb.PrintBriefObjListProp(n, nameof(ResultsOfQueryToRelationList), ResultsOfQueryToRelationList);
            sb.PrintExisting(n, nameof(SenderIndexedRuleInstance), SenderIndexedRuleInstance);
            sb.PrintExisting(n, nameof(SenderIndexedRulePart), SenderIndexedRulePart);
            sb.PrintExisting(n, nameof(SenderExpressionNode), SenderExpressionNode);

            return sb.ToString();
        }
    }
}
