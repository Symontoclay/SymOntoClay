using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class BindingVariableItem: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public KindOfBindingVariable Kind { get; set; } = KindOfBindingVariable.Unknown;
        public StrongIdentifierValue LeftVariable { get; set; }
        public StrongIdentifierValue RightVariable { get; set; }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public BindingVariableItem Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public BindingVariableItem Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (BindingVariableItem)context[this];
            }

            var result = new BindingVariableItem();
            context[this] = result;

            result.Kind = Kind;
            result.LeftVariable = LeftVariable?.Clone(context);
            result.RightVariable = RightVariable?.Clone(context);

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

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.PrintObjProp(n, nameof(LeftVariable), LeftVariable);
            sb.PrintObjProp(n, nameof(RightVariable), RightVariable);

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
            sb.PrintShortObjProp(n, nameof(LeftVariable), LeftVariable);
            sb.PrintShortObjProp(n, nameof(RightVariable), RightVariable);

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
            sb.PrintBriefObjProp(n, nameof(LeftVariable), LeftVariable);
            sb.PrintBriefObjProp(n, nameof(RightVariable), RightVariable);

            return sb.ToString();
        }
    }
}
