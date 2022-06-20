/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

namespace SymOntoClay.Core.DebugHelpers
{
    public static class DebugHelperForEntityConditionExpression
    {
#if DEBUG
        //private static ILogger _logger = LogManager.GetCurrentClassLogger();
#endif

        private static readonly CultureInfo _cultureInfo = new CultureInfo("en-GB");

        public static string ToString(EntityConditionExpressionNode expr)
        {
#if DEBUG
            //_logger.Info($"expr = {expr}");
#endif

            switch (expr.Kind)
            {
                case KindOfLogicalQueryNode.Relation:
                    return RelationToString(expr);

                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.QuestionVar:
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.LogicalVar:
                    return ConceptToString(expr);

                case KindOfLogicalQueryNode.Value:
                    return ValueToString(expr);

                case KindOfLogicalQueryNode.BinaryOperator:
                    return BinaryOperatorToString(expr);

                case KindOfLogicalQueryNode.Group:
                    return GroupToString(expr);

                default:
                    throw new ArgumentOutOfRangeException(nameof(expr.Kind), expr.Kind, null);
            }
        }

        private static string GroupToString(EntityConditionExpressionNode expr)
        {
            return $"({ToString(expr.Left)})";
        }

        private static string BinaryOperatorToString(EntityConditionExpressionNode expr)
        {
            var mark = string.Empty;

            switch (expr.KindOfOperator)
            {
                case KindOfOperatorOfLogicalQueryNode.And:
                    mark = "&";
                    break;

                case KindOfOperatorOfLogicalQueryNode.Or:
                    mark = "|";
                    break;

                case KindOfOperatorOfLogicalQueryNode.Is:
                    mark = "=";
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
            sb.Append(ToString(expr.Left));
            sb.Append($" {mark} ");
            sb.Append(AnnotatedItemToString(expr));
            sb.Append(ToString(expr.Right));

            return sb.ToString();
        }

        private static string RelationToString(EntityConditionExpressionNode expr)
        {
            var sb = new StringBuilder();

            sb.Append($"{expr.Name.NameValue}(");

            var resultParamsList = new List<string>();

            foreach (var param in expr.ParamsList)
            {
                resultParamsList.Add(ToString(param));
            }

            sb.Append(string.Join(",", resultParamsList));

            sb.Append(")");
            sb.Append(AnnotatedItemToString(expr));

            return sb.ToString();
        }

        private static string ConceptToString(EntityConditionExpressionNode expr)
        {
            var sb = new StringBuilder();
            sb.Append(expr.Name.NameValue);
            sb.Append(AnnotatedItemToString(expr));
            return sb.ToString();
        }

        private static string ValueToString(EntityConditionExpressionNode expr)
        {
            var sb = new StringBuilder();
            var value = expr.Value;

            sb.Append(DebugHelperForRuleInstance.ToString(value));
            sb.Append(AnnotatedItemToString(expr));

            return sb.ToString();
        }

        public static string AnnotatedItemToString(AnnotatedItem source)
        {
            var sb = new StringBuilder();

            if (source.HasModalitiesOrSections)
            {
                sb.Append(" |:");

                if (!source.WhereSection.IsNullOrEmpty())
                {
                    sb.Append(" ");
                    sb.Append(WhereSectionToString(source.WhereSection));
                }

                sb.Append(" :|");
            }

            return sb.ToString();
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
                sb.Append($" {DebugHelperForRuleInstance.ToString(source.First())} ");
            }
            else
            {
                var list = new List<string>();

                foreach (var item in source)
                {
                    list.Add($"{{ {DebugHelperForRuleInstance.ToString(item)} }}");
                }

                sb.Append(string.Join(",", list));
            }

            sb.Append("}");

            return sb.ToString();
        }
    }
}
