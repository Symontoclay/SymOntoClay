using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.DebugHelpers
{
    public static class DebugHelperForRuleInstance
    {
        private static readonly CultureInfo _cultureInfo = new CultureInfo("en-GB");

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
            sb.Append(" >: { ");

            sb.Append(ToString(primaryRulePart.Expression));

            sb.Append(" }");
            sb.Append(AnnotatedItemToString(primaryRulePart));

            return sb.ToString();
        }

        public static string ToString(LogicalQueryNode expr)
        {
            switch(expr.Kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    return BinaryOperatorToString(expr);

                case KindOfLogicalQueryNode.Concept:
                    return expr.Name.NameValue;

                default:
                    throw new ArgumentOutOfRangeException(nameof(expr.Kind), expr.Kind, null);
            }
        }

        private static string BinaryOperatorToString(LogicalQueryNode expr)
        {
            var mark = string.Empty;

            switch(expr.KindOfOperator)
            {
                case KindOfOperatorOfLogicalQueryNode.And:
                    mark = "&";
                    break;

                case KindOfOperatorOfLogicalQueryNode.Or:
                    mark = "|";
                    break;

                case KindOfOperatorOfLogicalQueryNode.Is:
                    mark = "is";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(expr.KindOfOperator), expr.KindOfOperator, null);
            }

            var sb = new StringBuilder();
            sb.Append(ToString(expr.Left));
            sb.Append($" {mark} ");
            sb.Append(ToString(expr.Right));

            return sb.ToString();
        }

        public static string AnnotatedItemToString(AnnotatedItem source)
        {
            var sb = new StringBuilder();

            if (source.HasModalitiesOrSections)
            {
                sb.Append(" |:");

                if(!source.QuantityQualityModalities.IsNullOrEmpty())
                {
                    sb.Append(" ");
                    sb.Append(QuantityQualityModalitiesToString(source.QuantityQualityModalities));
                }

                if(!source.WhereSection.IsNullOrEmpty())
                {
                    sb.Append(" ");
                    sb.Append(WhereSectionToString(source.WhereSection));
                }

                sb.Append(" :|");
            }

            return sb.ToString();
        }

        private static string QuantityQualityModalitiesToString(IList<Value> source)
        {
            return PrintModalityOrSection("=:", source);
        }

        private static string WhereSectionToString(IList<Value> source)
        {
            return PrintModalityOrSection("where:", source);
        }

        private static string PrintModalityOrSection(string mark, IList<Value> source)
        {
            var sb = new StringBuilder(mark);

            sb.Append(" {");

            if (source.Count == 1)
            {
                sb.Append($" {ToString(source.First())} ");
            }
            else
            {
                var list = new List<string>();

                foreach (var item in source)
                {
                    list.Add($"{{ {ToString(item)} }}");
                }

                sb.Append(string.Join(",", list));
            }

            sb.Append("}");

            return sb.ToString();
        }

        public static string ToString(Value value)
        {
            switch(value.KindOfValue)
            {
                case KindOfValue.NullValue:
                    return NullValueToString(value);

                case KindOfValue.LogicalValue:
                    return LogicalValueToString(value.AsLogicalValue);

                case KindOfValue.NumberValue:
                    return NumberValueToString(value.AsNumberValue);

                default:
                    throw new ArgumentOutOfRangeException(nameof(value.KindOfValue), value.KindOfValue, null);
            }
        }

        private static string NullValueToString(Value value)
        {
            var sb = new StringBuilder("NULL");
            sb.Append(AnnotatedItemToString(value));
            return sb.ToString();
        }

        private static string NumberValueToString(NumberValue value)
        {
            var systemValue = value.SystemValue;

            var sb = new StringBuilder();

            if (systemValue.HasValue)
            {
                sb.Append(systemValue.Value.ToString(_cultureInfo));
            }
            else
            {
                sb.Append("NULL");
            }

            sb.Append(AnnotatedItemToString(value));
            return sb.ToString();
        }

        private static string LogicalValueToString(LogicalValue value)
        {
            var systemValue = value.SystemValue;

            var sb = new StringBuilder();

            if(systemValue.HasValue)
            {
                sb.Append(systemValue.Value.ToString(_cultureInfo));
            }
            else
            {
                sb.Append("NULL");
            }

            sb.Append(AnnotatedItemToString(value));
            return sb.ToString();
        }
    }
}
