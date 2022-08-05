using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalSearchExplainNode: IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString
    {
        public KindOfLogicalSearchExplainNode Kind { get; set; } = KindOfLogicalSearchExplainNode.Unknown;
        public LogicalSearchExplainNode Result { get; set; }
        public LogicalSearchExplainNode Source { get; set; }
        public RuleInstance ProcessedRuleInstance { get; set; }
        public PrimaryRulePart ProcessedPrimaryRulePart { get; set; }
        public bool IsSuccess { get; set; }
        public List<ResultOfQueryToRelation> ResultsOfQueryToRelationList { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.PrintObjProp(n, nameof(Result), Result);
            sb.PrintObjProp(n, nameof(Source), Source);
            sb.PrintObjProp(n, nameof(ProcessedRuleInstance), ProcessedRuleInstance);
            sb.PrintObjProp(n, nameof(ProcessedPrimaryRulePart), ProcessedPrimaryRulePart);
            sb.AppendLine($"{spaces}{nameof(IsSuccess)} = {IsSuccess}");
            sb.PrintObjListProp(n, nameof(ResultsOfQueryToRelationList), ResultsOfQueryToRelationList);

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

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.PrintBriefObjProp(n, nameof(Result), Result);
            sb.PrintBriefObjProp(n, nameof(Source), Source);
            sb.PrintShortObjProp(n, nameof(ProcessedRuleInstance), ProcessedRuleInstance);
            sb.PrintShortObjProp(n, nameof(ProcessedPrimaryRulePart), ProcessedPrimaryRulePart);
            sb.AppendLine($"{spaces}{nameof(IsSuccess)} = {IsSuccess}");
            sb.PrintShortObjListProp(n, nameof(ResultsOfQueryToRelationList), ResultsOfQueryToRelationList);

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

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.PrintBriefObjProp(n, nameof(Result), Result);
            sb.PrintBriefObjProp(n, nameof(Source), Source);
            sb.PrintBriefObjProp(n, nameof(ProcessedRuleInstance), ProcessedRuleInstance);
            sb.PrintBriefObjProp(n, nameof(ProcessedPrimaryRulePart), ProcessedPrimaryRulePart);
            sb.AppendLine($"{spaces}{nameof(IsSuccess)} = {IsSuccess}");
            sb.PrintExistingList(n, nameof(ResultsOfQueryToRelationList), ResultsOfQueryToRelationList);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToDbgString()
        {
            return ToDbgString(0u);
        }

        /// <inheritdoc/>
        public string ToDbgString(uint n)
        {
            return this.GetDefaultToDbgStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToDbgString.PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.PrintExisting(n, nameof(Result), Result);
            sb.PrintExisting(n, nameof(Source), Source);
            sb.PrintDbgObjProp(n, nameof(ProcessedRuleInstance), ProcessedRuleInstance);
            sb.PrintDbgObjProp(n, nameof(ProcessedPrimaryRulePart), ProcessedPrimaryRulePart);
            sb.AppendLine($"{spaces}{nameof(IsSuccess)} = {IsSuccess}");
            sb.PrintExistingList(n, nameof(ResultsOfQueryToRelationList), ResultsOfQueryToRelationList);

            return sb.ToString();
        }
    }
}
