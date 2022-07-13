using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class MutablePartOfRuleInstance: IItemWithModalities, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public RuleInstance Parent { get; set; }

        [Modality]
        [ResolveToType(typeof(LogicalValue))]
        public Value ObligationModality { get; set; }
        
        [Modality]
        [ResolveToType(typeof(LogicalValue))]
        public Value SelfObligationModality { get; set; }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public MutablePartOfRuleInstance Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public MutablePartOfRuleInstance Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (MutablePartOfRuleInstance)context[this];
            }

            var result = new MutablePartOfRuleInstance();
            context[this] = result;

            result.ObligationModality = ObligationModality?.CloneValue(context);
            result.SelfObligationModality = SelfObligationModality?.CloneValue(context);

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
