using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class PostFilterOfQueryExecutingCardForPersistLogicalData : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public KindOfOperatorOfLogicalQueryNode KindOfBinaryOperator { get; set; } = KindOfOperatorOfLogicalQueryNode.Unknown;
        public LogicalQueryNode ProcessedExpr { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(KindOfBinaryOperator)} = {KindOfBinaryOperator}");
            sb.PrintShortObjProp(n, nameof(ProcessedExpr), ProcessedExpr);

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

            sb.AppendLine($"{spaces}{nameof(KindOfBinaryOperator)} = {KindOfBinaryOperator}");
            sb.PrintShortObjProp(n, nameof(ProcessedExpr), ProcessedExpr);

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

            sb.AppendLine($"{spaces}{nameof(KindOfBinaryOperator)} = {KindOfBinaryOperator}");
            sb.PrintBriefObjProp(n, nameof(ProcessedExpr), ProcessedExpr);

            return sb.ToString();
        }
    }
}
