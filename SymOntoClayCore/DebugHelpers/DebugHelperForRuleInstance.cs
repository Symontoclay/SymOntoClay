/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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
                sb.Append(" -> ");

                if(source.SecondaryParts.Count == 1)
                {
                    var firstSecondaryRulePart = source.SecondaryParts.Single();

                    sb.Append(ToString(firstSecondaryRulePart));
                }
                else
                {
                    throw new NotImplementedException();
                }          
            }

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

        public static string BaseRulePartToString(BaseRulePart baseRulePart)
        {
            var b = baseRulePart as PrimaryRulePart;

            if (b != null)
            {
                return ToString(b);
            }

            var c = baseRulePart as SecondaryRulePart;

            if (c != null)
            {
                return ToString(c);
            }

            return string.Empty;
        }

        public static string ToString(SecondaryRulePart rulePart)
        {
            var sb = new StringBuilder();
            sb.Append(" { ");

            sb.Append(ToString(rulePart.Expression));

            sb.Append(" }");
            sb.Append(AnnotatedItemToString(rulePart));

            return sb.ToString();
        }

        public static string ToString(LogicalQueryNode expr)
        {
            switch(expr.Kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    return BinaryOperatorToString(expr);

                case KindOfLogicalQueryNode.Relation:
                    return RelationToString(expr);

                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.QuestionVar:
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.LogicalVar:
                    return ConceptToString(expr);

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
            sb.Append(AnnotatedItemToString(expr));
            sb.Append(ToString(expr.Right));

            return sb.ToString();
        }

        private static string RelationToString(LogicalQueryNode expr)
        {
            var sb = new StringBuilder();

            sb.Append($"{expr.Name.NameValue}(");

            var resultParamsList = new List<string>();

            foreach(var param in expr.ParamsList)
            {
                resultParamsList.Add(ToString(param));
            }

            sb.Append(string.Join(",", resultParamsList));

            sb.Append(")");
            sb.Append(AnnotatedItemToString(expr));

            return sb.ToString();
        }

        private static string ConceptToString(LogicalQueryNode expr)
        {
            var sb = new StringBuilder();
            sb.Append(expr.Name.NameValue);
            sb.Append(AnnotatedItemToString(expr));
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
