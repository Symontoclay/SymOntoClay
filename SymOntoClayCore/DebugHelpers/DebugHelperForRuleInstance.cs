using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.DebugHelpers
{
    public static class DebugHelperForRuleInstance
    {
        public static string ToString(RuleInstance source)
        {
            var sb = new StringBuilder();
            sb.Append("{:");

            if(source.Name != null)
            {
                sb.Append($" {source.Name.NameValue}");
            }

            sb.Append(ToString(source.PrimaryPart));

            if(!source.SecondaryParts.IsNullOrEmpty())
            {
                throw new NotImplementedException();
            }

            //throw new NotImplementedException();

            sb.Append(" :}");
            sb.Append(AnnotatedItemToString(source));

            return sb.ToString();
        }

        public static string ToString(PrimaryRulePart primaryRulePart)
        {
            var sb = new StringBuilder();
            sb.Append(" >:{");

            sb.Append(ToString(primaryRulePart.Expression));

            sb.Append("}");
            sb.Append(AnnotatedItemToString(primaryRulePart));

            return sb.ToString();
        }

        public static string ToString(LogicalQueryNode expr)
        {
            throw new NotImplementedException();
        }

        public static string AnnotatedItemToString(AnnotatedItem source)
        {
            var sb = new StringBuilder();

            if (source.HasModalitiesOrSections)
            {
                sb.Append(" |:");

                throw new NotImplementedException();

                sb.Append(":|");
            }

            return sb.ToString();
        }
    }
}
