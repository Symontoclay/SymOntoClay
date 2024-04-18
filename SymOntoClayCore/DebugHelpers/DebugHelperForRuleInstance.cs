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
using System.Threading;

namespace SymOntoClay.Core.DebugHelpers
{
    public static class DebugHelperForRuleInstance
    {
        private static readonly CultureInfo _cultureInfo = new CultureInfo("en-GB");

        public static string ToString(RuleInstance source)
        {
            return ToString(source, null, HumanizedOptions.ShowAll);
        }

        public static string ToString(RuleInstance source, HumanizedOptions options)
        {
            return ToString(source, null, options);
        }
        
        public static string ToString(RuleInstance source, MutablePartOfRuleInstance mutablePartOfRuleInstance)
        {
            return ToString(source, mutablePartOfRuleInstance, HumanizedOptions.ShowAll);
        }

        public static string ToString(RuleInstance source, MutablePartOfRuleInstance mutablePartOfRuleInstance, HumanizedOptions options)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options,
                MutablePartOfRuleInstance = mutablePartOfRuleInstance
            };

            return ToString(source, opt);
        }

        public static string ToString(RuleInstance source, DebugHelperOptions options)
        {
            var sb = new StringBuilder();

            var isHtml = options.IsHtml;

            var isSelected = isHtml && (options.ItemsForSelection?.Any(p => p.Equals(source)) ?? false);

            if (isSelected)
            {
                sb.Append("<b>");
            }

            if(isHtml)
            {
                sb.Append(StringHelper.ToHtmlCode("{:"));
            }
            else
            {
                sb.Append("{:");
            }

            if(options.HumanizedOptions == HumanizedOptions.ShowAll)
            {
                if (source.Name != null)
                {
                    sb.Append($" {source.Name.NameValue}");
                }
            }

            sb.Append(ToString(source.PrimaryPart, options));

            if(!source.SecondaryParts.IsNullOrEmpty())
            {
                if (isHtml)
                {
                    sb.Append(StringHelper.ToHtmlCode(" -> "));
                }
                else
                {
                    sb.Append(" -> ");
                }

                if(source.SecondaryParts.Count == 1)
                {
                    var firstSecondaryRulePart = source.SecondaryParts.Single();

                    sb.Append(ToString(firstSecondaryRulePart, options));
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            var mutablePartOfRuleInstance = options.MutablePartOfRuleInstance;

            if (source.ObligationModality != null && source.ObligationModality.KindOfValue != KindOfValue.NullValue)
            {
                sb.Append(" o: ");

                sb.Append(ModalityValueToString(source.ObligationModality, options));
            }
            else
            {
                if (mutablePartOfRuleInstance != null)
                {
                    if(mutablePartOfRuleInstance.ObligationModality != null && mutablePartOfRuleInstance.ObligationModality.KindOfValue != KindOfValue.NullValue)
                    {
                        sb.Append(" o: ");

                        sb.Append(ModalityValueToString(mutablePartOfRuleInstance.ObligationModality, options));
                    }
                }
            }

            if (source.SelfObligationModality != null && source.SelfObligationModality.KindOfValue != KindOfValue.NullValue)
            {
                sb.Append(" so: ");

                sb.Append(ModalityValueToString(source.SelfObligationModality, options));
            }
            else
            {
                if (mutablePartOfRuleInstance != null)
                {
                    if (mutablePartOfRuleInstance.SelfObligationModality != null && mutablePartOfRuleInstance.SelfObligationModality.KindOfValue != KindOfValue.NullValue)
                    {
                        sb.Append(" so: ");

                        sb.Append(ModalityValueToString(mutablePartOfRuleInstance.SelfObligationModality, options));
                    }
                }
            }

            if (isHtml)
            {
                sb.Append(StringHelper.ToHtmlCode(" :}"));
            }
            else
            {
                sb.Append(" :}");
            }
            
            sb.Append(AnnotatedItemToString(source, options));

            if (isSelected)
            {
                sb.Append("</b>");
            }

            return sb.ToString();
        }

        public static string ToString(PrimaryRulePart primaryRulePart, HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return ToString(primaryRulePart, opt);
        }

        public static string ToString(PrimaryRulePart primaryRulePart, DebugHelperOptions options)
        {
            var sb = new StringBuilder();

            var isSelected = options.IsHtml && (options.ItemsForSelection?.Any(p => p.Equals(primaryRulePart)) ?? false);

            if (isSelected)
            {
                sb.Append("<b>");
            }

            if (options.IsHtml)
            {
                sb.Append(StringHelper.ToHtmlCode(" >: { "));
            }
            else
            {
                sb.Append(" >: { ");
            }

            sb.Append(ToString(primaryRulePart?.Expression, options));

            sb.Append(" }");
            sb.Append(AnnotatedItemToString(primaryRulePart, options));

            if (isSelected)
            {
                sb.Append("</b>");
            }

            return sb.ToString();
        }

        public static string BaseRulePartToString(BaseRulePart baseRulePart, HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return BaseRulePartToString(baseRulePart, opt);
        }

        public static string BaseRulePartToString(BaseRulePart baseRulePart, DebugHelperOptions options)
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
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return ToString(rulePart, opt);
        }

        public static string ToString(SecondaryRulePart rulePart, DebugHelperOptions options)
        {
            var sb = new StringBuilder();

            var isSelected = options.IsHtml && (options.ItemsForSelection?.Any(p => p.Equals(rulePart)) ?? false);

            if (isSelected)
            {
                sb.Append("<b>");
            }

            sb.Append(" { ");

            sb.Append(ToString(rulePart.Expression, options));

            sb.Append(" }");
            sb.Append(AnnotatedItemToString(rulePart, options));

            if (isSelected)
            {
                sb.Append("</b>");
            }

            return sb.ToString();
        }

        public static string ToString(LogicalQueryNode expr, HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return ToString(expr, opt);
        }

        public static string ToString(LogicalQueryNode expr, DebugHelperOptions options)
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

                case KindOfLogicalQueryNode.Fact:
                    return FactToString(expr, options);

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

        private static string GroupToString(LogicalQueryNode expr, DebugHelperOptions options)
        {
            var sb = new StringBuilder();

            var isSelected = options.IsHtml && (options.ItemsForSelection?.Any(p => p.Equals(expr)) ?? false);

            if (isSelected)
            {
                sb.Append("<b>");
            }

            if (!expr.LinkedVars.IsNullOrEmpty())
            {
                foreach (var linkedVar in expr.LinkedVars)
                {
                    sb.Append($"{linkedVar.Name.NameValue} = ");
                }
            }
            
            sb.Append($"({ToString(expr.Left, options)})");

            if (isSelected)
            {
                sb.Append("</b>");
            }

            return sb.ToString();
        }

        private static string FactToString(LogicalQueryNode expr, DebugHelperOptions options)
        {
            var sb = new StringBuilder();

            var isSelected = options.IsHtml && (options.ItemsForSelection?.Any(p => p.Equals(expr)) ?? false);

            if(isSelected)
            {
                sb.Append("<b>");
            }

            if (!expr.LinkedVars.IsNullOrEmpty())
            {
                foreach (var linkedVar in expr.LinkedVars)
                {
                    sb.Append($"{linkedVar.Name.NameValue} = ");
                }
            }

            sb.Append(ToString(expr.Fact, options));

            if (isSelected)
            {
                sb.Append("</b>");
            }

            return sb.ToString();
        }

        private static string UnaryOperatorToString(LogicalQueryNode expr, DebugHelperOptions options)
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

            var isSelected = options.IsHtml && (options.ItemsForSelection?.Any(p => p.Equals(expr)) ?? false);

            var sb = new StringBuilder();

            if (isSelected)
            {
                sb.Append("<b>");
            }
                        
            sb.Append($" {mark} ");
            sb.Append(AnnotatedItemToString(expr, options));
            sb.Append(ToString(expr.Left, options));

            if (isSelected)
            {
                sb.Append("</b>");
            }

            return sb.ToString();
        }

        private static string BinaryOperatorToString(LogicalQueryNode expr, DebugHelperOptions options)
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

            if (options.IsHtml)
            {
                mark = StringHelper.ToHtmlCode(mark);
            }

            var isSelected = options.IsHtml && (options.ItemsForSelection?.Any(p => p.Equals(expr)) ?? false);

            var sb = new StringBuilder();

            if (isSelected)
            {
                sb.Append("<b>");
            }
            
            sb.Append(ToString(expr.Left, options));
            sb.Append($" {mark} ");
            sb.Append(AnnotatedItemToString(expr, options));
            sb.Append(ToString(expr.Right, options));

            if (isSelected)
            {
                sb.Append("</b>");
            }

            return sb.ToString();
        }

        private static string RelationToString(LogicalQueryNode expr, DebugHelperOptions options)
        {
            var sb = new StringBuilder();

            var isSelected = options.IsHtml && (options.ItemsForSelection?.Any(p => p.Equals(expr)) ?? false);

            if (isSelected)
            {
                sb.Append("<b>");
            }

            if (!expr.LinkedVars.IsNullOrEmpty())
            {
                foreach (var linkedVar in expr.LinkedVars)
                {
                    sb.Append($"{linkedVar.Name.NameValue} = ");
                }
            }

            sb.Append($"{expr.Name.NameValue}(");

            var resultParamsList = new List<string>();

            foreach (var param in expr.ParamsList)
            {
                resultParamsList.Add(ToString(param, options));
            }

            sb.Append(string.Join(",", resultParamsList));

            sb.Append(")");
            sb.Append(AnnotatedItemToString(expr, options));

            if (isSelected)
            {
                sb.Append("</b>");
            }

            return sb.ToString();
        }

        private static string ConceptToString(LogicalQueryNode expr, DebugHelperOptions options)
        {
            var sb = new StringBuilder();

            var isSelected = options.IsHtml && (options.ItemsForSelection?.Any(p => p.Equals(expr)) ?? false);

            if (isSelected)
            {
                sb.Append("<b>");
            }

            sb.Append(expr.Name.NameValue);
            sb.Append(AnnotatedItemToString(expr, options));

            if (isSelected)
            {
                sb.Append("</b>");
            }

            return sb.ToString();
        }

        private static string ValueToString(LogicalQueryNode expr, DebugHelperOptions options)
        {
            var sb = new StringBuilder();
            var value = expr.Value;

            var isSelected = options.IsHtml && (options.ItemsForSelection?.Any(p => p.Equals(expr)) ?? false);

            if (isSelected)
            {
                sb.Append("<b>");
            }

            sb.Append(ToString(value, options));
            sb.Append(AnnotatedItemToString(expr, options));

            if (isSelected)
            {
                sb.Append("</b>");
            }

            return sb.ToString();
        }

        public static string AnnotatedItemToString(AnnotatedItem source, HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return AnnotatedItemToString(source, opt);
        }

        public static string AnnotatedItemToString(AnnotatedItem source, DebugHelperOptions options)
        {
            if(source == null || options.HumanizedOptions == HumanizedOptions.ShowOnlyMainContent)
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
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return ToString(value, opt);
        }

        public static string ToString(Value value, DebugHelperOptions options)
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
                case KindOfValue.ConditionalEntityValue:
                    return ConditionalEntitySourceValueToString(value, options);

                default:
                    throw new ArgumentOutOfRangeException(nameof(value.KindOfValue), value.KindOfValue, null);
            }
        }

        private static string ConditionalEntitySourceValueToString(Value value, DebugHelperOptions options)
        {
            var sb = new StringBuilder(value.ToHumanizedString(options));
            sb.Append(AnnotatedItemToString(value, options));
            return sb.ToString();
        }

        private static string WaypointValueToString(Value value, DebugHelperOptions options)
        {
            var sb = new StringBuilder(value.ToHumanizedString(options));
            sb.Append(AnnotatedItemToString(value, options));
            return sb.ToString();
        }

        private static string NullValueToString(Value value, DebugHelperOptions options)
        {
            var sb = new StringBuilder("NULL");
            sb.Append(AnnotatedItemToString(value, options));
            return sb.ToString();
        }

        private static string NumberValueToString(NumberValue value, DebugHelperOptions options)
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

            sb.Append(AnnotatedItemToString(value, options));
            return sb.ToString();
        }

        private static string LogicalValueToString(LogicalValue value, DebugHelperOptions options)
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

            sb.Append(AnnotatedItemToString(value, options));
            return sb.ToString();
        }

        private static string FuzzyLogicNonNumericSequenceValueToString(FuzzyLogicNonNumericSequenceValue value, DebugHelperOptions options)
        {
            var sb = new StringBuilder(value.DebugView);
            sb.Append(AnnotatedItemToString(value, options));
            return sb.ToString();
        }

        private static string StrongIdentifierValueToString(StrongIdentifierValue value, DebugHelperOptions options)
        {
            var sb = new StringBuilder(value.NameValue);
            sb.Append(AnnotatedItemToString(value, options));
            return sb.ToString();
        }

        public static string ModalityValueToString(Value value, DebugHelperOptions options)
        {
            switch(value.KindOfValue)
            {
                case KindOfValue.LogicalValue:
                    return LogicalValueToString(value.AsLogicalValue, options);

                case KindOfValue.NumberValue:
                    return NumberValueToString(value.AsNumberValue, options);

                case KindOfValue.FuzzyLogicNonNumericSequenceValue:
                    return FuzzyLogicNonNumericSequenceValueToString(value.AsFuzzyLogicNonNumericSequenceValue, options);

                case KindOfValue.StrongIdentifierValue:
                    return StrongIdentifierValueToString(value.AsStrongIdentifierValue, options);

                case KindOfValue.LogicalModalityExpressionValue:
                    return DebugHelperForLogicalModalityExpression.ToString(value.AsLogicalModalityExpressionValue.Expression, options);

                default:
                    throw new ArgumentOutOfRangeException(nameof(value.KindOfValue), value.KindOfValue, null);
            }
        }
    }
}
