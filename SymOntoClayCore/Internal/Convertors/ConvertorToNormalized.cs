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
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Convertors
{
    public static class ConvertorToNormalized
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public static RuleInstance Convert(RuleInstance source, CheckDirtyOptions options)
        {
            var convertingContext = new Dictionary<object, object>();

            return Convert(source, options, convertingContext);
        }

        public static RuleInstance Convert(RuleInstance source, CheckDirtyOptions options, Dictionary<object, object> convertingContext)
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
                return (RuleInstance)convertingContext[source];
            }

            var result = new RuleInstance() { IsSource = false, Original = source };
            convertingContext[source] = result;
            result.Normalized = result;

            result.PrimaryPart = ConvertPrimaryRulePart(source.PrimaryPart, result, options, convertingContext);

            if (!source.SecondaryParts.IsNullOrEmpty())
            {
                foreach (var secondaryPart in source.SecondaryParts)
                {
                    result.SecondaryParts.Add(ConvertSecondaryRulePart(secondaryPart, result, options, convertingContext));
                }
            }

            result.TypeOfAccess = source.TypeOfAccess;
            result.Holder = source.Holder;

            FillAnnotationsModalitiesAndSections(source, result, options, convertingContext);

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        private static PrimaryRulePart ConvertPrimaryRulePart(PrimaryRulePart source, RuleInstance ruleInstance, CheckDirtyOptions options, Dictionary<object, object> convertingContext)
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
                return (PrimaryRulePart)convertingContext[source];
            }

            var result = new PrimaryRulePart();
            convertingContext[source] = result;

            FillBaseRulePart(source, result, ruleInstance, options, convertingContext);

            if (!source.SecondaryParts.IsNullOrEmpty())
            {
                foreach (var secondaryPart in source.SecondaryParts)
                {
                    result.SecondaryParts.Add(ConvertSecondaryRulePart(secondaryPart, ruleInstance, options, convertingContext));
                }
            }

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        private static SecondaryRulePart ConvertSecondaryRulePart(SecondaryRulePart source, RuleInstance ruleInstance, CheckDirtyOptions options, Dictionary<object, object> convertingContext)
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
                return (SecondaryRulePart)convertingContext[source];
            }

            var result = new SecondaryRulePart();
            convertingContext[source] = result;

            FillBaseRulePart(source, result, ruleInstance, options, convertingContext);

            result.PrimaryPart = ConvertPrimaryRulePart(source.PrimaryPart, ruleInstance, options, convertingContext);

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        private static void FillBaseRulePart(BaseRulePart source, BaseRulePart dest, RuleInstance ruleInstance, CheckDirtyOptions options, Dictionary<object, object> convertingContext)
        {
            dest.Parent = ruleInstance;
            dest.IsActive = source.IsActive;

            dest.Expression = ConvertLogicalQueryNode(source.Expression, options, convertingContext, source.AliasesDict);

            FillAnnotationsModalitiesAndSections(source, dest, options, convertingContext);
        }

        private static LogicalQueryNode ConvertLogicalQueryNode(LogicalQueryNode source, CheckDirtyOptions options, Dictionary<object, object> convertingContext, Dictionary<StrongIdentifierValue, LogicalQueryNode> aliasesDict)
        {
#if DEBUG
            //_gbcLogger.Info($"source = {source}");
#endif
            
            switch (source.Kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    switch (source.KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Is:
                        case KindOfOperatorOfLogicalQueryNode.IsNot:
                        case KindOfOperatorOfLogicalQueryNode.And:
                        case KindOfOperatorOfLogicalQueryNode.Or:
                        case KindOfOperatorOfLogicalQueryNode.More:
                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                        case KindOfOperatorOfLogicalQueryNode.Less:
                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(source.KindOfOperator), source.KindOfOperator, null);
                    }

                case KindOfLogicalQueryNode.UnaryOperator:
                    switch (source.KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Not:
                            return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(source.KindOfOperator), source.KindOfOperator, null);
                    }

                case KindOfLogicalQueryNode.Concept:
                    return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict);

                case KindOfLogicalQueryNode.Entity:
                    return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict);

                case KindOfLogicalQueryNode.Relation:
                    if(source.ParamsList.Count == 1)
                    {
                        return ConvertUnaryPredicateToFullIsPredicate(source, convertingContext, aliasesDict);
                    }
                    return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict);

                case KindOfLogicalQueryNode.LogicalVar:
                    return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict);

                case KindOfLogicalQueryNode.QuestionVar:
                    return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict);

                case KindOfLogicalQueryNode.Value:
                    return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict);

                case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                    return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict);

                case KindOfLogicalQueryNode.Group:
                    return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict);

                default:
                    throw new ArgumentOutOfRangeException(nameof(source.Kind), source.Kind, null);
            }
        }

        private static LogicalQueryNode ConvertUnaryPredicateToFullIsPredicate(LogicalQueryNode source, Dictionary<object, object> convertingContext, Dictionary<StrongIdentifierValue, LogicalQueryNode> aliasesDict)
        {
#if DEBUG
            //_gbcLogger.Info($"source = {source}");
            //_gbcLogger.Info($"source.GetHumanizeDbgString() = {source.GetHumanizeDbgString()}");
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
            result.Name = NameHelper.CreateName("is");
            result.ParamsList = new List<LogicalQueryNode>();

            result.ParamsList.Add(source.ParamsList[0]);

            var superNameNode = new LogicalQueryNode();
            superNameNode.Kind = KindOfLogicalQueryNode.Concept;
            superNameNode.Name = source.Name;

            result.ParamsList.Add(superNameNode);

            var rankNode = new LogicalQueryNode();
            result.ParamsList.Add(rankNode);
            rankNode.Kind = KindOfLogicalQueryNode.Value;
            rankNode.Value = new LogicalValue(1);

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
            //_gbcLogger.Info($"result.GetHumanizeDbgString() = {result.GetHumanizeDbgString()}");
#endif

            return result;
        }

        private static LogicalQueryNode ConvertLogicalQueryNodeInDefaultWay(LogicalQueryNode source, CheckDirtyOptions options, Dictionary<object, object> convertingContext, Dictionary<StrongIdentifierValue, LogicalQueryNode> aliasesDict)
        {
#if DEBUG
            //_gbcLogger.Info("ConvertLogicalQueryNodeInDefaultWay!!!!!");
            //_gbcLogger.Info($"source = {source}");
            //_gbcLogger.Info($"(options != null) = {options != null}");
#endif

            if (source == null)
            {
                return null;
            }

            if (source.Kind == KindOfLogicalQueryNode.LogicalVar && aliasesDict != null && aliasesDict.Any())
            {
                if(aliasesDict.ContainsKey(source.Name))
                {
                    return ConvertLogicalQueryNode(aliasesDict[source.Name], options, convertingContext, aliasesDict);
                }
            }

            if (convertingContext.ContainsKey(source))
            {
                return (LogicalQueryNode)convertingContext[source];
            }

            var result = new LogicalQueryNode();
            convertingContext[source] = result;

            result.Kind = source.Kind;
            result.KindOfOperator = source.KindOfOperator;
            result.Name = source.Name;

            if(source.Left != null)
            {
                result.Left = ConvertLogicalQueryNode(source.Left, options, convertingContext, aliasesDict);
            }
            
            if(source.Right != null)
            {
                result.Right = ConvertLogicalQueryNode(source.Right, options, convertingContext, aliasesDict);
            }
            
            if(!source.ParamsList.IsNullOrEmpty())
            {
                var destParametersList = new List<LogicalQueryNode>();

                foreach (var param in source.ParamsList)
                {
                    destParametersList.Add(ConvertLogicalQueryNode(param, options, convertingContext, aliasesDict));
                }

                result.ParamsList = destParametersList;
            }

            if (source.Kind == KindOfLogicalQueryNode.Value && source.Value.IsWaypointSourceValue && options != null && options.ConvertWaypointValueFromSource)
            {
                result.Value = source.Value.AsWaypointSourceValue.ConvertToWaypointValue(options.EngineContext, options.LocalContext);
            }
            else
            {
                result.Value = source.Value;
            }
            
            result.FuzzyLogicNonNumericSequenceValue = source.FuzzyLogicNonNumericSequenceValue;
            result.IsQuestion = source.IsQuestion;

            FillAnnotationsModalitiesAndSections(source, result, options, convertingContext);

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        private static void FillAnnotationsModalitiesAndSections(AnnotatedItem source, AnnotatedItem dest, CheckDirtyOptions options, Dictionary<object, object> convertingContext)
        {
            if (dest.WhereSection == null)
            {
                dest.WhereSection = new List<Value>();
            }

            if (!source.WhereSection.IsNullOrEmpty())
            {
                foreach (var item in source.WhereSection)
                {
                    dest.WhereSection.Add(item);
                }
            }

            if (!source.Annotations.IsNullOrEmpty())
            {
                dest.Annotations = new List<RuleInstance>();

                var destAnnotationsList = dest.Annotations;

                foreach (var sourceAnnotation in source.Annotations)
                {
                    var destAnnotation = Convert(sourceAnnotation, options, convertingContext);

                    destAnnotationsList.Add(destAnnotation);
                }
            }
        }
    }
}
