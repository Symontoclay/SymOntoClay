using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class MutablePartOfRuleInstance: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public RuleInstance Parent { get; set; }

        [Modality]
        [ResolveToType(typeof(LogicalValue))]
        public Value ObligationModality { get; set; }

        [Modality]
        [ResolveToType(typeof(LogicalValue))]
        public Value SelfObligationModality { get; set; }

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

            sb.PrintBriefObjProp(n, nameof(Parent), Parent);

            sb.PrintObjProp(n, nameof(ObligationModality), ObligationModality);
            sb.PrintObjProp(n, nameof(SelfObligationModality), SelfObligationModality);

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

            sb.PrintBriefObjProp(n, nameof(Parent), Parent);

            sb.PrintShortObjProp(n, nameof(ObligationModality), ObligationModality);
            sb.PrintShortObjProp(n, nameof(SelfObligationModality), SelfObligationModality);

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

            sb.PrintExisting(n, nameof(Parent), Parent);

            sb.PrintExisting(n, nameof(ObligationModality), ObligationModality);
            sb.PrintExisting(n, nameof(SelfObligationModality), SelfObligationModality);

            return sb.ToString();
        }
    }
}
