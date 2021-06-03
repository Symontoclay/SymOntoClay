using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class IntermediateSEHItem : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public StrongIdentifierValue VariableName { get; set; }
        public RuleInstance Condition { get; set; }
        public IntermediateScriptCommand JumpToMe { get; set; }

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

            sb.PrintObjProp(n, nameof(VariableName), VariableName);
            sb.PrintBriefObjProp(n, nameof(Condition), Condition);
            sb.PrintBriefObjProp(n, nameof(JumpToMe), JumpToMe);

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

            sb.PrintShortObjProp(n, nameof(VariableName), VariableName);
            sb.PrintBriefObjProp(n, nameof(Condition), Condition);
            sb.PrintBriefObjProp(n, nameof(JumpToMe), JumpToMe);

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

            sb.PrintBriefObjProp(n, nameof(VariableName), VariableName);
            sb.PrintBriefObjProp(n, nameof(Condition), Condition);
            sb.PrintBriefObjProp(n, nameof(JumpToMe), JumpToMe);

            return sb.ToString();
        }
    }
}
