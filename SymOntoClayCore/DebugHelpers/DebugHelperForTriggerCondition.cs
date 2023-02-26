/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.DebugHelpers
{
    public static class DebugHelperForTriggerCondition
    {
        public static string ToString(TriggerConditionNode source)
        {
            switch (source.Kind)
            {
                case KindOfTriggerConditionNode.Concept:
                case KindOfTriggerConditionNode.Entity:
                case KindOfTriggerConditionNode.Var:
                    return ConceptToString(source);

                case KindOfTriggerConditionNode.EntityRef:
                    return EntityRefToString(source);

                case KindOfTriggerConditionNode.EntityCondition:
                    return EntityConditionToString(source);

                case KindOfTriggerConditionNode.BinaryOperator:
                    return BinaryOperatorToString(source);

                case KindOfTriggerConditionNode.UnaryOperator:
                    return UnaryOperatorToString(source);

                case KindOfTriggerConditionNode.Value:
                    return ValueToString(source);

                case KindOfTriggerConditionNode.Group:
                    return GroupToString(source);

                case KindOfTriggerConditionNode.FuzzyLogicNonNumericSequence:
                    return FuzzyLogicNonNumericSequenceToString(source);

                case KindOfTriggerConditionNode.Fact:
                    return FactToString(source);

                case KindOfTriggerConditionNode.Duration:
                    return DurationToString(source);

                default:
                    throw new ArgumentOutOfRangeException(nameof(source.Kind), source.Kind, null);
            }
        }

        private static string ConceptToString(TriggerConditionNode source)
        {
            return source.Name.NameValue;
        }
        
        private static string EntityRefToString(TriggerConditionNode source)
        {
            throw new NotImplementedException();
        }

        private static string EntityConditionToString(TriggerConditionNode source)
        {
            throw new NotImplementedException();
        }

        private static string BinaryOperatorToString(TriggerConditionNode source)
        {
            return $"{ToString(source.Left)} {OperatorsHelper.GetSymbol(source.KindOfOperator)} {ToString(source.Right)}";
        }

        private static string UnaryOperatorToString(TriggerConditionNode source)
        {
            if(source.KindOfOperator == KindOfOperator.CallFunction)
            {
                return CallFunctionToString(source);
            }

            return $" {OperatorsHelper.GetSymbol(source.KindOfOperator)} {ToString(source.Left)}";
        }

        private static string ValueToString(TriggerConditionNode source)
        {
            return DebugHelperForRuleInstance.ToString(source.Value);
        }

        private static string GroupToString(TriggerConditionNode source)
        {
            return $"({ToString(source.Left)})";
        }

        private static string FuzzyLogicNonNumericSequenceToString(TriggerConditionNode source)
        {
            return source.FuzzyLogicNonNumericSequenceValue.DebugView;
        }

        private static string FactToString(TriggerConditionNode source)
        {
            return DebugHelperForRuleInstance.ToString(source.RuleInstance);
        }

        private static string DurationToString(TriggerConditionNode source)
        {
            return $"duration {source.Value.ToHumanizedString()}";
        }

        private static string CallFunctionToString(TriggerConditionNode source)
        {
            var resultParamsList = new List<string>();

            var sourceParamsList = source.ParamsList;

            if (!sourceParamsList.IsNullOrEmpty())
            {
                if (source.IsNamedParameters)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    foreach (var param in sourceParamsList)
                    {
                        resultParamsList.Add(ToString(param));
                    }
                }
            }

            return $"{ToString(source.Left)} ({string.Join(",", resultParamsList)})";
        }
    }
}
