/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
using SymOntoClay.CoreHelper;
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
        private static readonly CultureInfo _cultureInfo = new CultureInfo("en-GB");

        private static DebugHelperOptions _defaultOptions = new DebugHelperOptions();

        public static string ToString(EntityConditionExpressionNode expr)
        {
            return ToString(expr, _defaultOptions);
        }

        public static string ToString(EntityConditionExpressionNode expr, DebugHelperOptions options)
        {
            switch (expr.Kind)
            {
                case KindOfLogicalQueryNode.Relation:
                    return RelationToString(expr, options);

                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.QuestionVar:
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.LogicalVar:
                    return ConceptToString(expr, options);

                case KindOfLogicalQueryNode.Value:
                    return ValueToString(expr, options);

                case KindOfLogicalQueryNode.BinaryOperator:
                    return BinaryOperatorToString(expr, options);

                case KindOfLogicalQueryNode.Group:
                    return GroupToString(expr, options);

                default:
                    throw new ArgumentOutOfRangeException(nameof(expr.Kind), expr.Kind, null);
            }
        }

        private static string GroupToString(EntityConditionExpressionNode expr, DebugHelperOptions options)
        {
            return $"({ToString(expr.Left)})";
        }

        private static string BinaryOperatorToString(EntityConditionExpressionNode expr, DebugHelperOptions options)
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

            if(options.IsHtml)
            {
                mark = StringHelper.ToHtmlCode(mark);
            }

            var sb = new StringBuilder();
            sb.Append(ToString(expr.Left));
            sb.Append($" {mark} ");
            sb.Append(AnnotatedItemToString(expr, options));
            sb.Append(ToString(expr.Right));

            return sb.ToString();
        }

        private static string RelationToString(EntityConditionExpressionNode expr, DebugHelperOptions options)
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

        private static string ConceptToString(EntityConditionExpressionNode expr, DebugHelperOptions options)
        {
            var sb = new StringBuilder();
            sb.Append(expr.Name.NameValue);
            sb.Append(AnnotatedItemToString(expr, options));
            return sb.ToString();
        }

        private static string ValueToString(EntityConditionExpressionNode expr, DebugHelperOptions options)
        {
            var sb = new StringBuilder();
            var value = expr.Value;

            sb.Append(DebugHelperForRuleInstance.ToString(value, options));
            sb.Append(AnnotatedItemToString(expr, options));

            return sb.ToString();
        }

        public static string AnnotatedItemToString(AnnotatedItem source, DebugHelperOptions options)
        {
            var sb = new StringBuilder();

            if (source.HasModalitiesOrSections)
            {
                sb.Append(" |:");

                if (!source.WhereSection.IsNullOrEmpty())
                {
                    sb.Append(" ");
                    sb.Append(WhereSectionToString(source.WhereSection, options));
                }

                sb.Append(" :|");
            }

            return sb.ToString();
        }

        private static string WhereSectionToString(IList<Value> source, DebugHelperOptions options)
        {
            return PrintModalityOrSection("where:", source, options);
        }

        private static string PrintModalityOrSection(string mark, IList<Value> source, DebugHelperOptions options)
        {
            var sb = new StringBuilder(mark);

            sb.Append(" {");

            if (source.Count == 1)
            {
                sb.Append($" {DebugHelperForRuleInstance.ToString(source.First(), options)} ");
            }
            else
            {
                var list = new List<string>();

                foreach (var item in source)
                {
                    list.Add($"{{ {DebugHelperForRuleInstance.ToString(item, options)} }}");
                }

                sb.Append(string.Join(",", list));
            }

            sb.Append("}");

            return sb.ToString();
        }
    }
}
