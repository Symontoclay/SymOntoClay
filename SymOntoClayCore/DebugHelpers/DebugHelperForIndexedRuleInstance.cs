/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.DebugHelpers
{
    public static class DebugHelperForIndexedRuleInstance
    {
        private static readonly CultureInfo _cultureInfo = new CultureInfo("en-GB");

        public static string ToString(IndexedRuleInstance source, IEntityDictionary entityDictionary)
        {
            var sb = new StringBuilder();
            sb.Append("{:");

            if (source.Name != null)
            {
                sb.Append($" {source.Name.NameValue}");
            }

            sb.Append(ToString(source.PrimaryPart, entityDictionary));

            if (!source.SecondaryParts.IsNullOrEmpty())
            {
                sb.Append(" -> ");

                if (source.SecondaryParts.Count == 1)
                {
                    var firstSecondaryRulePart = source.SecondaryParts.Single();

                    sb.Append(ToString(firstSecondaryRulePart, entityDictionary));
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

        public static string ToString(IndexedPrimaryRulePart primaryRulePart, IEntityDictionary entityDictionary)
        {
            var sb = new StringBuilder();
            sb.Append(" >: { ");

            sb.Append(ToString(primaryRulePart.Expression, entityDictionary));

            sb.Append(" }");
            sb.Append(AnnotatedItemToString(primaryRulePart));

            return sb.ToString();
        }

        public static string ToString(IndexedSecondaryRulePart rulePart, IEntityDictionary entityDictionary)
        {
            var sb = new StringBuilder();
            sb.Append(" { ");

            sb.Append(ToString(rulePart.Expression, entityDictionary));

            sb.Append(" }");
            sb.Append(AnnotatedItemToString(rulePart));

            return sb.ToString();
        }

        public static string ToString(BaseIndexedLogicalQueryNode expr, IEntityDictionary entityDictionary)
        {
            switch(expr.Kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    return BinaryOperatorToString(expr as BinaryOperatorIndexedLogicalQueryNode, entityDictionary);

                case KindOfLogicalQueryNode.Relation:
                    return RelationToString(expr as RelationIndexedLogicalQueryNode, entityDictionary);

                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.QuestionVar:
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.LogicalVar:
                    return BaseKeyRefToString(expr as BaseKeyRefIndexedLogicalQueryNode, entityDictionary);

                default:
                    throw new ArgumentOutOfRangeException(nameof(expr.Kind), expr.Kind, null);
            }
        }

        private static string BinaryOperatorToString(BinaryOperatorIndexedLogicalQueryNode expr, IEntityDictionary entityDictionary)
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
                    mark = "is";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(expr.KindOfOperator), expr.KindOfOperator, null);
            }

            var sb = new StringBuilder();
            sb.Append(ToString(expr.Left, entityDictionary));
            sb.Append($" {mark} ");
            sb.Append(AnnotatedItemToString(expr));
            sb.Append(ToString(expr.Right, entityDictionary));

            return sb.ToString();
        }

        private static string RelationToString(RelationIndexedLogicalQueryNode expr, IEntityDictionary entityDictionary)
        {
            var sb = new StringBuilder();

            sb.Append($"{entityDictionary.GetName(expr.Key)}(");

            var resultParamsList = new List<string>();

            foreach (var param in expr.Params)
            {
                resultParamsList.Add(ToString(param, entityDictionary));
            }

            sb.Append(string.Join(",", resultParamsList));

            sb.Append(")");
            sb.Append(AnnotatedItemToString(expr));

            return sb.ToString();
        }

        private static string BaseKeyRefToString(BaseKeyRefIndexedLogicalQueryNode expr, IEntityDictionary entityDictionary)
        {
            var sb = new StringBuilder();
            sb.Append(entityDictionary.GetName(expr.Key));
            sb.Append(AnnotatedItemToString(expr));
            return sb.ToString();
        }

        public static string AnnotatedItemToString(IndexedAnnotatedItem source)
        {
            var sb = new StringBuilder();

            if (source.HasModalitiesOrSections)
            {
                sb.Append(" |:");

                if (!source.QuantityQualityModalities.IsNullOrEmpty())
                {
                    sb.Append(" ");
                    sb.Append(QuantityQualityModalitiesToString(source.QuantityQualityModalities));
                }

                if (!source.WhereSection.IsNullOrEmpty())
                {
                    sb.Append(" ");
                    sb.Append(WhereSectionToString(source.WhereSection));
                }

                sb.Append(" :|");
            }

            return sb.ToString();
        }

        private static string QuantityQualityModalitiesToString(IList<IndexedValue> source)
        {
            return PrintModalityOrSection("=:", source);
        }

        private static string WhereSectionToString(IList<IndexedValue> source)
        {
            return PrintModalityOrSection("where:", source);
        }

        private static string PrintModalityOrSection(string mark, IList<IndexedValue> source)
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

        public static string ToString(IndexedValue value)
        {
            switch (value.KindOfValue)
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

        private static string NullValueToString(IndexedValue value)
        {
            var sb = new StringBuilder("NULL");
            sb.Append(AnnotatedItemToString(value));
            return sb.ToString();
        }

        private static string NumberValueToString(IndexedNumberValue value)
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

        private static string LogicalValueToString(IndexedLogicalValue value)
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
    }
}
