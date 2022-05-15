/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using NLog;
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

#if DEBUG
        //private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public static string ToString(RuleInstance source, HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var sb = new StringBuilder();
            sb.Append("{:");

            if(options == HumanizedOptions.ShowAll)
            {
                if (source.Name != null)
                {
                    sb.Append($" {source.Name.NameValue}");
                }
            }

            sb.Append(ToString(source.PrimaryPart, options));

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
            sb.Append(AnnotatedItemToString(source, options));

            return sb.ToString();
        }

        public static string ToString(PrimaryRulePart primaryRulePart, HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var sb = new StringBuilder();
            sb.Append(" >: { ");

            sb.Append(ToString(primaryRulePart?.Expression, options));

            sb.Append(" }");
            sb.Append(AnnotatedItemToString(primaryRulePart, options));

            return sb.ToString();
        }

        public static string BaseRulePartToString(BaseRulePart baseRulePart, HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var b = baseRulePart as PrimaryRulePart;

            if (b != null)
            {
                return ToString(b, options);
            }

            var c = baseRulePart as SecondaryRulePart;

            if (c != null)
            {
                return ToString(c, options);
            }

            return string.Empty;
        }

        public static string ToString(SecondaryRulePart rulePart, HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var sb = new StringBuilder();
            sb.Append(" { ");

            sb.Append(ToString(rulePart.Expression, options));

            sb.Append(" }");
            sb.Append(AnnotatedItemToString(rulePart, options));

            return sb.ToString();
        }

        public static string ToString(LogicalQueryNode expr, HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            if (expr == null)
            {
                return string.Empty;
            }

            switch (expr.Kind)
            {
                case KindOfLogicalQueryNode.UnaryOperator:
                    return UnaryOperatorToString(expr, options);

                case KindOfLogicalQueryNode.BinaryOperator:
                    return BinaryOperatorToString(expr, options);

                case KindOfLogicalQueryNode.Relation:
                    return RelationToString(expr, options);

                case KindOfLogicalQueryNode.Group:
                    return GroupToString(expr, options);

                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.QuestionVar:
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.LogicalVar:
                case KindOfLogicalQueryNode.Var:
                    return ConceptToString(expr, options);

                case KindOfLogicalQueryNode.Value:
                    return ValueToString(expr, options);

                case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                    return FuzzyLogicNonNumericSequenceValueToString(expr.FuzzyLogicNonNumericSequenceValue, options);

                default:
                    throw new ArgumentOutOfRangeException(nameof(expr.Kind), expr.Kind, null);
            }
        }

        private static string GroupToString(LogicalQueryNode expr)
        {
            return $"({ToString(expr.Left, options)})";
        }

        private static string UnaryOperatorToString(LogicalQueryNode expr)
        {
            var mark = string.Empty;

            switch (expr.KindOfOperator)
            {
                case KindOfOperatorOfLogicalQueryNode.Not:
                    mark = "not";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(expr.KindOfOperator), expr.KindOfOperator, null);
            }

            var sb = new StringBuilder();            
            sb.Append($" {mark} ");
            sb.Append(AnnotatedItemToString(expr, options));
            sb.Append(ToString(expr.Left, options));

            return sb.ToString();
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

                case KindOfOperatorOfLogicalQueryNode.IsNot:
                    mark = "is not";
                    break;

                case KindOfOperatorOfLogicalQueryNode.More:
                    mark = ">";
                    break;
                case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                    mark = ">=";
                    break;

                case KindOfOperatorOfLogicalQueryNode.Less:
                    mark = "<";
                    break;

                case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                    mark = "<=";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(expr.KindOfOperator), expr.KindOfOperator, null);
            }

            var sb = new StringBuilder();
            sb.Append(ToString(expr.Left, options));
            sb.Append($" {mark} ");
            sb.Append(AnnotatedItemToString(expr, options));
            sb.Append(ToString(expr.Right, options));

            return sb.ToString();
        }

        private static string RelationToString(LogicalQueryNode expr)
        {
            var sb = new StringBuilder();

            sb.Append($"{expr.Name.NameValue}(");

            var resultParamsList = new List<string>();

            foreach (var param in expr.ParamsList)
            {
                resultParamsList.Add(ToString(param, options));
            }

            sb.Append(string.Join(",", resultParamsList));

            sb.Append(")");
            sb.Append(AnnotatedItemToString(expr, options));

            return sb.ToString();
        }

        private static string ConceptToString(LogicalQueryNode expr)
        {
            var sb = new StringBuilder();
            sb.Append(expr.Name.NameValue);
            sb.Append(AnnotatedItemToString(expr, options));
            return sb.ToString();
        }

        private static string ValueToString(LogicalQueryNode expr)
        {
            var sb = new StringBuilder();
            var value = expr.Value;

            sb.Append(ToString(value, options));
            sb.Append(AnnotatedItemToString(expr, options));

            return sb.ToString();
        }

        public static string AnnotatedItemToString(AnnotatedItem source, HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            if(source == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            if (source.HasModalitiesOrSections)
            {
                sb.Append(" |:");

                if(!source.WhereSection.IsNullOrEmpty())
                {
                    sb.Append(" ");
                    sb.Append(WhereSectionToString(source.WhereSection, options));
                }

                sb.Append(" :|");
            }

            return sb.ToString();
        }

        private static string WhereSectionToString(IList<Value> source)
        {
            return PrintModalityOrSection("where:", source, options);
        }

        private static string PrintModalityOrSection(string mark, IList<Value> source)
        {
            var sb = new StringBuilder(mark);

            sb.Append(" {");

            if (source.Count == 1)
            {
                sb.Append($" {ToString(source.First(), options)} ");
            }
            else
            {
                var list = new List<string>();

                foreach (var item in source)
                {
                    list.Add($"{{ {ToString(item, options)} }}");
                }

                sb.Append(string.Join(",", list));
            }

            sb.Append("}");

            return sb.ToString();
        }

        public static string ToString(Value value, HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            switch(value.KindOfValue)
            {
                case KindOfValue.NullValue:
                    return NullValueToString(value, options);

                case KindOfValue.LogicalValue:
                    return LogicalValueToString(value.AsLogicalValue, options);

                case KindOfValue.NumberValue:
                    return NumberValueToString(value.AsNumberValue, options);

                case KindOfValue.FuzzyLogicNonNumericSequenceValue:
                    return FuzzyLogicNonNumericSequenceValueToString(value.AsFuzzyLogicNonNumericSequenceValue, options);

                case KindOfValue.StrongIdentifierValue:
                    return StrongIdentifierValueToString(value.AsStrongIdentifierValue, options);

                case KindOfValue.WaypointSourceValue:
                case KindOfValue.WaypointValue:
                    return WaypointValueToString(value, options);

                case KindOfValue.ConditionalEntitySourceValue:
                    return ConditionalEntitySourceValueToString(value, options);

                default:
                    throw new ArgumentOutOfRangeException(nameof(value.KindOfValue), value.KindOfValue, null);
            }
        }

        private static string ConditionalEntitySourceValueToString(Value value)
        {
            var sb = new StringBuilder(value.ToDbgString());
            sb.Append(AnnotatedItemToString(value, options));
            return sb.ToString();
        }

        private static string WaypointValueToString(Value value)
        {
            var sb = new StringBuilder(value.ToDbgString());
            sb.Append(AnnotatedItemToString(value, options));
            return sb.ToString();
        }

        private static string NullValueToString(Value value)
        {
            var sb = new StringBuilder("NULL");
            sb.Append(AnnotatedItemToString(value, options));
            return sb.ToString();
        }

        private static string NumberValueToString(NumberValue value)
        {
            var systemValue = value.SystemValue;

            var sb = new StringBuilder();

            if (systemValue.HasValue)
            {
                sb.Append(systemValue.Value.ToString(_cultureInfo, options));
            }
            else
            {
                sb.Append("NULL");
            }

            sb.Append(AnnotatedItemToString(value, options));
            return sb.ToString();
        }

        private static string LogicalValueToString(LogicalValue value)
        {
            var systemValue = value.SystemValue;

            var sb = new StringBuilder();

            if(systemValue.HasValue)
            {
                sb.Append(systemValue.Value.ToString(_cultureInfo, optionss));
            }
            else
            {
                sb.Append("NULL");
            }

            sb.Append(AnnotatedItemToString(value, options));
            return sb.ToString();
        }

        private static string FuzzyLogicNonNumericSequenceValueToString(FuzzyLogicNonNumericSequenceValue value)
        {
            var sb = new StringBuilder(value.DebugView);
            sb.Append(AnnotatedItemToString(valuev));
            return sb.ToString();
        }

        private static string StrongIdentifierValueToString(StrongIdentifierValue value)
        {
            var sb = new StringBuilder(value.NameValue);
            sb.Append(AnnotatedItemToString(value, options));
            return sb.ToString();
        }
    }
}
