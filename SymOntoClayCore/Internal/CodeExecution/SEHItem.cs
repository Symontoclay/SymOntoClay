using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class SEHItem : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public StrongIdentifierValue VariableName { get; set; }
        public RuleInstance Condition { get; set; }
        public int TargetPosition { get; set; }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public SEHItem Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public SEHItem Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (SEHItem)context[this];
            }

            var result = new SEHItem();
            context[this] = result;

            result.VariableName = VariableName?.Clone(context);
            result.Condition = Condition?.Clone(context);
            result.TargetPosition = TargetPosition;

            return result;
        }

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
            sb.AppendLine($"{spaces}{nameof(TargetPosition)} = {TargetPosition}");

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
            sb.AppendLine($"{spaces}{nameof(TargetPosition)} = {TargetPosition}");

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
            sb.AppendLine($"{spaces}{nameof(TargetPosition)} = {TargetPosition}");

            return sb.ToString();
        }
    }
}
