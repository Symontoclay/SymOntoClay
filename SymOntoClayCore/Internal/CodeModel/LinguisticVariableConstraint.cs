using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class LinguisticVariableConstraint : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public List<LinguisticVariableConstraintItem> Items { get; set; } = new List<LinguisticVariableConstraintItem>();

        public bool isFit(ReasonOfFuzzyLogicResolving reason)
        {
            if((reason == null || reason.Kind == KindOfReasonOfFuzzyLogicResolving.Unknown) && (Items.IsNullOrEmpty() || Items.All(p => p.Kind == KindOfLinguisticVariableСonstraintItem.Unknown)))
            {
                return true;
            }

            var kindOfReson = reason.Kind;

            switch(kindOfReson)
            {
                case KindOfReasonOfFuzzyLogicResolving.Inheritance:
                    return Items.Any(p => p.Kind == KindOfLinguisticVariableСonstraintItem.Inheritance);

                case KindOfReasonOfFuzzyLogicResolving.Relation:
                    return Items.Any(p => p.Kind == KindOfLinguisticVariableСonstraintItem.Relation && p.RelationName == reason.RelationName);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfReson), kindOfReson, null);
            }
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public LinguisticVariableConstraint Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public LinguisticVariableConstraint Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (LinguisticVariableConstraint)context[this];
            }

            var result = new LinguisticVariableConstraint();
            context[this] = result;

            result.Items = Items?.Select(p => p.Clone(context)).ToList();

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

            sb.PrintObjListProp(n, nameof(Items), Items);

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

            sb.PrintShortObjListProp(n, nameof(Items), Items);

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

            sb.PrintBriefObjListProp(n, nameof(Items), Items);

            return sb.ToString();
        }
    }
}
