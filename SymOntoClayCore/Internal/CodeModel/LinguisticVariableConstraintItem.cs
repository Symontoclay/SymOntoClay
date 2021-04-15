using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class LinguisticVariableConstraintItem : IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString
    {
        public KindOfLinguisticVariableConstraintItem Kind { get; set; } = KindOfLinguisticVariableConstraintItem.Unknown;
        public StrongIdentifierValue RelationName { get; set; }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public LinguisticVariableConstraintItem Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public LinguisticVariableConstraintItem Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (LinguisticVariableConstraintItem)context[this];
            }

            var result = new LinguisticVariableConstraintItem();
            context[this] = result;

            result.Kind = Kind;
            result.RelationName = RelationName.Clone(context);

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
            sb.PrintObjProp(n, nameof(RelationName), RelationName);

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
            sb.PrintShortObjProp(n, nameof(RelationName), RelationName);

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
            sb.PrintBriefObjProp(n, nameof(RelationName), RelationName);

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
            var sb = new StringBuilder(spaces);
            switch(Kind)
            {
                case KindOfLinguisticVariableConstraintItem.Unknown:
                    sb.Append("unknown");
                    break;

                case KindOfLinguisticVariableConstraintItem.Inheritance:
                    sb.Append("for inheritance");
                    break;

                case KindOfLinguisticVariableConstraintItem.Relation:
                    sb.Append($"for relation `{RelationName.NameValue}`");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, null);
            }
            return sb.ToString();
        }
    }
}
