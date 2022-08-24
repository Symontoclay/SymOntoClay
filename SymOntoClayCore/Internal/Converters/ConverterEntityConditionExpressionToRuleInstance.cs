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
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Converters
{
    public static class ConverterEntityConditionExpressionToRuleInstance
    {
#if DEBUG
        //private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        private enum AdviceForConversionEntityConditionExpressionToRuleInstance
        {
            Root,
            InRelation
        }

        public static RuleInstance Convert(EntityConditionExpressionNode source, CheckDirtyOptions options)
        {
            var convertingContext = new Dictionary<object, object>();

            return Convert(source, options, convertingContext);
        }

        public static RuleInstance Convert(EntityConditionExpressionNode source, CheckDirtyOptions options, Dictionary<object, object> convertingContext)
        {
#if DEBUG
            //_gbcLogger.Info($"source = {source}");
#endif

            if (source == null)
            {
                return null;
            }

            var result = new RuleInstance();

            var primaryRulePart = new PrimaryRulePart();

            result.PrimaryPart = primaryRulePart;

            primaryRulePart.Parent = result;
            primaryRulePart.IsActive = true;

            primaryRulePart.Expression = ConvertToLogicalQueryNode(source, AdviceForConversionEntityConditionExpressionToRuleInstance.Root, options, convertingContext);

#if DEBUG
            //_gbcLogger.Info($"result = {DebugHelperForRuleInstance.ToString(result)}");
#endif

            return result;
        }

        private static LogicalQueryNode ConvertToLogicalQueryNode(EntityConditionExpressionNode source, AdviceForConversionEntityConditionExpressionToRuleInstance advice, CheckDirtyOptions options, Dictionary<object, object> convertingContext)
        {
#if DEBUG
            //_gbcLogger.Info($"source = {source}");
#endif

            switch(advice)
            {
                case AdviceForConversionEntityConditionExpressionToRuleInstance.Root:
                    switch (source.Kind)
                    {
                        case KindOfLogicalQueryNode.Concept:
                            return ConvertStandaloneConcept(source, options, convertingContext);

                        case KindOfLogicalQueryNode.Relation:
                            return ConvertRelation(source, options, convertingContext);

                        case KindOfLogicalQueryNode.BinaryOperator:
                            return ConvertBinaryOperator(source, options, convertingContext);

                        case KindOfLogicalQueryNode.Group:
                            return ConvertGroup(source, options, convertingContext);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(source.Kind), source.Kind, null);
                    }

                case AdviceForConversionEntityConditionExpressionToRuleInstance.InRelation:
                    switch (source.Kind)
                    {
                        case KindOfLogicalQueryNode.Concept:
                        case KindOfLogicalQueryNode.Entity:
                        case KindOfLogicalQueryNode.Value:
                        case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                            return ConvertInRelationConceptOrEntityOrValue(source, options, convertingContext);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(source.Kind), source.Kind, null);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(advice), advice, null);
            }
        }

        private static LogicalQueryNode ConvertGroup(EntityConditionExpressionNode source, CheckDirtyOptions options, Dictionary<object, object> convertingContext)
        {
#if DEBUG
            //_gbcLogger.Info($"source = {source}");
#endif

            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (LogicalQueryNode)convertingContext[source];
            }

            var result = new LogicalQueryNode();
            convertingContext[source] = result;

            result.Kind = KindOfLogicalQueryNode.Group;

            result.Left = ConvertToLogicalQueryNode(source.Left, AdviceForConversionEntityConditionExpressionToRuleInstance.Root, options, convertingContext);

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        private static LogicalQueryNode ConvertBinaryOperator(EntityConditionExpressionNode source, CheckDirtyOptions options, Dictionary<object, object> convertingContext)
        {
#if DEBUG
            //_gbcLogger.Info($"source = {source}");
#endif

            if (source == null)
            {
                return null;
            }

            if(source.KindOfOperator == KindOfOperatorOfLogicalQueryNode.Is && source.Left.Kind == KindOfLogicalQueryNode.Concept)
            {
                return ConvertIsOperator(source, options, convertingContext);
            }

            if (convertingContext.ContainsKey(source))
            {
                return (LogicalQueryNode)convertingContext[source];
            }

            var result = new LogicalQueryNode();
            convertingContext[source] = result;

            result.Kind = KindOfLogicalQueryNode.BinaryOperator;
            result.KindOfOperator = source.KindOfOperator;

            result.Left = ConvertToLogicalQueryNode(source.Left, AdviceForConversionEntityConditionExpressionToRuleInstance.Root, options, convertingContext);
            result.Right = ConvertToLogicalQueryNode(source.Right, AdviceForConversionEntityConditionExpressionToRuleInstance.Root, options, convertingContext);

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        private static LogicalQueryNode ConvertIsOperator(EntityConditionExpressionNode source, CheckDirtyOptions options, Dictionary<object, object> convertingContext)
        {
#if DEBUG
            //_gbcLogger.Info($"source = {source}");
#endif

            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (LogicalQueryNode)convertingContext[source];
            }

            var result = new LogicalQueryNode();
            convertingContext[source] = result;

            result.Kind = KindOfLogicalQueryNode.Relation;
            result.Name = source.Left.Name;

            var destParametersList = new List<LogicalQueryNode>();

            result.ParamsList = destParametersList;

            destParametersList.Add(CreateEntitySelfVar());

            destParametersList.Add(ConvertToLogicalQueryNode(source.Right, AdviceForConversionEntityConditionExpressionToRuleInstance.InRelation, options, convertingContext));

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        private static LogicalQueryNode ConvertRelation(EntityConditionExpressionNode source, CheckDirtyOptions options, Dictionary<object, object> convertingContext)
        {
#if DEBUG
            //_gbcLogger.Info($"source = {source}");
#endif

            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (LogicalQueryNode)convertingContext[source];
            }

            var result = new LogicalQueryNode();
            convertingContext[source] = result;

            result.Kind = KindOfLogicalQueryNode.Relation;
            result.Name = source.Name;

            var destParametersList = new List<LogicalQueryNode>();

            result.ParamsList = destParametersList;

            foreach(var sourceParam in source.ParamsList)
            {
                destParametersList.Add(ConvertToLogicalQueryNode(sourceParam, AdviceForConversionEntityConditionExpressionToRuleInstance.InRelation, options, convertingContext));
            }

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        private static LogicalQueryNode ConvertInRelationConceptOrEntityOrValue(EntityConditionExpressionNode source, CheckDirtyOptions options, Dictionary<object, object> convertingContext)
        {
#if DEBUG
            //_gbcLogger.Info($"source = {source}");
#endif

            if (source == null)
            {
                return null;
            }

            var nameValue = source.Name?.NameValue;

#if DEBUG
            //_gbcLogger.Info($"nameValue = {nameValue}");
#endif

            if(!string.IsNullOrWhiteSpace(nameValue))
            {
                if (nameValue == "this")
                {
                    return CreateEntitySelfVar();
                }
            }

            if (convertingContext.ContainsKey(source))
            {
                return (LogicalQueryNode)convertingContext[source];
            }

            var result = new LogicalQueryNode();
            convertingContext[source] = result;

            result.Kind = source.Kind;
            result.Name = source.Name;
            result.Value = source.Value;
            result.FuzzyLogicNonNumericSequenceValue = source.FuzzyLogicNonNumericSequenceValue;

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        private static LogicalQueryNode ConvertStandaloneConcept(EntityConditionExpressionNode source, CheckDirtyOptions options, Dictionary<object, object> convertingContext)
        {
#if DEBUG
            //_gbcLogger.Info($"source = {source}");
#endif

            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (LogicalQueryNode)convertingContext[source];
            }

            var result = new LogicalQueryNode();
            convertingContext[source] = result;

            result.Kind = KindOfLogicalQueryNode.Relation;
            result.Name = source.Name;

            var destParametersList = new List<LogicalQueryNode>();

            result.ParamsList = destParametersList;


            destParametersList.Add(CreateEntitySelfVar());

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        private static LogicalQueryNode CreateEntitySelfVar()
        {
            var selfVarNode = new LogicalQueryNode();
            selfVarNode.Kind = KindOfLogicalQueryNode.LogicalVar;
            selfVarNode.Name = NameHelper.CreateName("$_");

            return selfVarNode;
        }
    }
}
