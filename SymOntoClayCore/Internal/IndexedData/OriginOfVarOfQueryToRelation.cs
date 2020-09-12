using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class OriginOfVarOfQueryToRelation : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public ulong KeyOfRuleInstance { get; set; }
        public IndexedRuleInstance IndexedRuleInstance { get; set; }
        public IndexedBaseRulePart IndexedRulePart { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(KeyOfRuleInstance)} = {KeyOfRuleInstance}");
            sb.PrintObjProp(n, nameof(IndexedRuleInstance), IndexedRuleInstance);
            sb.PrintObjProp(n, nameof(IndexedRulePart), IndexedRulePart);

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

            sb.AppendLine($"{spaces}{nameof(KeyOfRuleInstance)} = {KeyOfRuleInstance}");
            sb.PrintShortObjProp(n, nameof(IndexedRuleInstance), IndexedRuleInstance);
            sb.PrintShortObjProp(n, nameof(IndexedRulePart), IndexedRulePart);

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

            sb.AppendLine($"{spaces}{nameof(KeyOfRuleInstance)} = {KeyOfRuleInstance}");
            sb.PrintBriefObjProp(n, nameof(IndexedRuleInstance), IndexedRuleInstance);
            sb.PrintBriefObjProp(n, nameof(IndexedRulePart), IndexedRulePart);

            return sb.ToString();
        }
    }
}
