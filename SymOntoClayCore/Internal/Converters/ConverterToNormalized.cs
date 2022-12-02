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
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Converters
{
    public static class ConverterToNormalized
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
            //_gbcLogger.Info($"source = {source.ToHumanizedString()}");
#endif

            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (RuleInstance)convertingContext[source];
            }

            var processedRuleInstances = new List<RuleInstance>();

            var result = new RuleInstance() { IsSource = false, Original = source };
            convertingContext[source] = result;
            result.Normalized = result;

            result.PrimaryPart = ConvertPrimaryRulePart(source.PrimaryPart, result, options, convertingContext, processedRuleInstances);

            if (!source.SecondaryParts.IsNullOrEmpty())
            {
                foreach (var secondaryPart in source.SecondaryParts)
                {
                    result.SecondaryParts.Add(ConvertSecondaryRulePart(secondaryPart, result, options, convertingContext, processedRuleInstances));
                }
            }

            result.TypeOfAccess = source.TypeOfAccess;
            result.Holder = source.Holder;
            result.ObligationModality = source.ObligationModality;
            result.SelfObligationModality = source.SelfObligationModality;

            if(processedRuleInstances.Any())
            {
                var obligationModality = result.ObligationModality;

                if (obligationModality == null || obligationModality.KindOfValue == KindOfValue.NullValue)
                {
                    var targetObligationModalitiesList = processedRuleInstances.Select(p => p.ObligationModality).Where(p => p != null && p.KindOfValue == KindOfValue.NullValue);

                    if(targetObligationModalitiesList.Any())
                    {
                        result.ObligationModality = targetObligationModalitiesList.First();
                    }
                }

                var selfObligationModality = result.SelfObligationModality;

                if (selfObligationModality == null || selfObligationModality.KindOfValue == KindOfValue.NullValue)
                {
                    var targetSelfObligationModalitiesList = processedRuleInstances.Select(p => p.SelfObligationModality).Where(p => p != null && p.KindOfValue == KindOfValue.NullValue);

                    if(targetSelfObligationModalitiesList.Any())
                    {
                        result.SelfObligationModality = targetSelfObligationModalitiesList.First();
                    }
                }
            }

#if DEBUG
            //_gbcLogger.Info($"source.TypeOfAccess = {source.TypeOfAccess}");
            //_gbcLogger.Info($"result.TypeOfAccess = {result.TypeOfAccess}");
            //_gbcLogger.Info($"source.Holder = {source.Holder}");
            //_gbcLogger.Info($"result.Holder = {result.Holder}");
#endif

            FillAnnotationsModalitiesAndSections(source, result, options, convertingContext);

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
            //_gbcLogger.Info($"result = {result.ToHumanizedString()}");
#endif

            return result;
        }

        private static PrimaryRulePart ConvertPrimaryRulePart(PrimaryRulePart source, RuleInstance ruleInstance, CheckDirtyOptions options, Dictionary<object, object> convertingContext, List<RuleInstance> processedRuleInstances)
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

            FillBaseRulePart(source, result, ruleInstance, options, convertingContext, processedRuleInstances);

            if (!source.SecondaryParts.IsNullOrEmpty())
            {
                foreach (var secondaryPart in source.SecondaryParts)
                {
                    result.SecondaryParts.Add(ConvertSecondaryRulePart(secondaryPart, ruleInstance, options, convertingContext, processedRuleInstances));
                }
            }

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        private static SecondaryRulePart ConvertSecondaryRulePart(SecondaryRulePart source, RuleInstance ruleInstance, CheckDirtyOptions options, Dictionary<object, object> convertingContext, List<RuleInstance> processedRuleInstances)
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

            FillBaseRulePart(source, result, ruleInstance, options, convertingContext, processedRuleInstances);

            result.PrimaryPart = ConvertPrimaryRulePart(source.PrimaryPart, ruleInstance, options, convertingContext, processedRuleInstances);

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        private static void FillBaseRulePart(BaseRulePart source, BaseRulePart dest, RuleInstance ruleInstance, CheckDirtyOptions options, Dictionary<object, object> convertingContext, List<RuleInstance> processedRuleInstances)
        {
            dest.Parent = ruleInstance;
            dest.IsActive = source.IsActive;

            dest.Expression = ConvertLogicalQueryNode(source.Expression, options, convertingContext, source.AliasesDict, processedRuleInstances);

            dest.SetTypeOfAccess(source.TypeOfAccess);
            dest.SetHolder(source.Holder);

            FillAnnotationsModalitiesAndSections(source, dest, options, convertingContext);
        }

        private static LogicalQueryNode ConvertLogicalQueryNode(LogicalQueryNode source, CheckDirtyOptions options, Dictionary<object, object> convertingContext, Dictionary<StrongIdentifierValue, LogicalQueryNode> aliasesDict, List<RuleInstance> processedRuleInstances)
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
                        case KindOfOperatorOfLogicalQueryNode.Or:
                        case KindOfOperatorOfLogicalQueryNode.More:
                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                        case KindOfOperatorOfLogicalQueryNode.Less:
                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict, processedRuleInstances);

                        case KindOfOperatorOfLogicalQueryNode.And:
                            return ConvertAndLogicalQueryNode(source, options, convertingContext, aliasesDict, processedRuleInstances);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(source.KindOfOperator), source.KindOfOperator, null);
                    }

                case KindOfLogicalQueryNode.UnaryOperator:
                    switch (source.KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Not:
                            return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict, processedRuleInstances);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(source.KindOfOperator), source.KindOfOperator, null);
                    }

                case KindOfLogicalQueryNode.Concept:
                    return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict, processedRuleInstances);

                case KindOfLogicalQueryNode.Entity:
                    return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict, processedRuleInstances);

                case KindOfLogicalQueryNode.Relation:
                    if(source.ParamsList.Count == 1)
                    {
                        return ConvertUnaryPredicateToFullIsPredicate(source, convertingContext, aliasesDict);
                    }
                    return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict, processedRuleInstances);

                case KindOfLogicalQueryNode.LogicalVar:
                    return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict, processedRuleInstances);

                case KindOfLogicalQueryNode.QuestionVar:
                    return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict, processedRuleInstances);

                case KindOfLogicalQueryNode.Var:
                    return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict, processedRuleInstances);

                case KindOfLogicalQueryNode.Value:
                    return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict, processedRuleInstances);

                case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                    return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict, processedRuleInstances);

                case KindOfLogicalQueryNode.Group:
                    return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict, processedRuleInstances);

                case KindOfLogicalQueryNode.Fact:
                    return ConvertFactLogicalQueryNode(source, options, convertingContext, aliasesDict, processedRuleInstances);

                default:
                    throw new ArgumentOutOfRangeException(nameof(source.Kind), source.Kind, null);
            }
        }

        private static LogicalQueryNode ConvertUnaryPredicateToFullIsPredicate(LogicalQueryNode source, Dictionary<object, object> convertingContext, Dictionary<StrongIdentifierValue, LogicalQueryNode> aliasesDict)
        {
#if DEBUG
            //_gbcLogger.Info($"source = {source}");
            //_gbcLogger.Info($"source.ToHumanizedString() = {source.ToHumanizedString()}");
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

            var kindOfSourceName = source.Name.KindOfName;

#if DEBUG
            //_gbcLogger.Info($"kindOfSourceName = {kindOfSourceName}");
#endif

            switch(kindOfSourceName)
            {
                case KindOfName.Concept:
                    superNameNode.Kind = KindOfLogicalQueryNode.Concept;
                    break;

                case KindOfName.LogicalVar:
                    superNameNode.Kind = KindOfLogicalQueryNode.LogicalVar;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSourceName), kindOfSourceName, null);
            }

            superNameNode.Name = source.Name;

            result.ParamsList.Add(superNameNode);

            var rankNode = new LogicalQueryNode();
            result.ParamsList.Add(rankNode);
            rankNode.Kind = KindOfLogicalQueryNode.Value;
            rankNode.Value = new LogicalValue(1);

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
            //_gbcLogger.Info($"result.ToHumanizedString() = {result.ToHumanizedString()}");
#endif

            return result;
        }

        private static LogicalQueryNode ConvertFactLogicalQueryNode(LogicalQueryNode source, CheckDirtyOptions options, Dictionary<object, object> convertingContext, Dictionary<StrongIdentifierValue, LogicalQueryNode> aliasesDict, List<RuleInstance> processedRuleInstances)
        {
#if DEBUG
            //_gbcLogger.Info($"source = {source}");
            //_gbcLogger.Info($"source.ToHumanizedString() = {source.ToHumanizedString()}");
#endif

            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (LogicalQueryNode)convertingContext[source];
            }

            var fact = source.Fact;

            processedRuleInstances.Add(fact);

            var targetExpr = fact.PrimaryPart.Expression;

#if DEBUG
            //_gbcLogger.Info($"targetExpr = {targetExpr}");
            //_gbcLogger.Info($"targetExpr.ToHumanizedString() = {targetExpr.ToHumanizedString()}");
#endif

            var groupNode = new LogicalQueryNode() { Kind = KindOfLogicalQueryNode.Group };
            groupNode.Left = ConvertLogicalQueryNode(targetExpr, options, convertingContext, aliasesDict, processedRuleInstances);

#if DEBUG
            //_gbcLogger.Info($"groupNode = {groupNode}");
            //_gbcLogger.Info($"groupNode.ToHumanizedString() = {groupNode.ToHumanizedString()}");
#endif

            convertingContext[source] = groupNode;

            return groupNode;
        }

        private static LogicalQueryNode ConvertAndLogicalQueryNode(LogicalQueryNode source, CheckDirtyOptions options, Dictionary<object, object> convertingContext, Dictionary<StrongIdentifierValue, LogicalQueryNode> aliasesDict, List<RuleInstance> processedRuleInstances)
        {
#if DEBUG
            //_gbcLogger.Info("ConvertAndLogicalQueryNode!!!!!");
            //_gbcLogger.Info($"source = {source}");
            //_gbcLogger.Info($"source.Kind = {source.Kind}");
            //_gbcLogger.Info($"source = {source.ToHumanizedString()}");
            //_gbcLogger.Info($"(options != null) = {options != null}");
#endif

            if(!(options?.IgnoreStandaloneConceptsInNormalization ?? false))
            {
                return ConvertLogicalQueryNodeInDefaultWay(source, options, convertingContext, aliasesDict, processedRuleInstances);
            }

#if DEBUG
            _gbcLogger.Info("ConvertAndLogicalQueryNode NEXT");
            _gbcLogger.Info($"source = {source}");
            _gbcLogger.Info($"source = {source.ToHumanizedString()}");
#endif

            var leftOperand = source.Left;
            var rightOperand = source.Right;

            throw new NotImplementedException();
        }

        private static LogicalQueryNode ConvertLogicalQueryNodeInDefaultWay(LogicalQueryNode source, CheckDirtyOptions options, Dictionary<object, object> convertingContext, Dictionary<StrongIdentifierValue, LogicalQueryNode> aliasesDict, List<RuleInstance> processedRuleInstances)
        {
#if DEBUG
            //_gbcLogger.Info("ConvertLogicalQueryNodeInDefaultWay!!!!!");
            //_gbcLogger.Info($"source = {source}");
            //_gbcLogger.Info($"source.Kind = {source.Kind}");
            //_gbcLogger.Info($"source = {source.ToHumanizedString()}");
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
                    return ConvertLogicalQueryNode(aliasesDict[source.Name], options, convertingContext, aliasesDict, processedRuleInstances);
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

            if(options != null && options.ReplaceConcepts != null && (source.Kind == KindOfLogicalQueryNode.Concept || source.Kind == KindOfLogicalQueryNode.Entity))
            {
                if(options.ReplaceConcepts.ContainsKey(source.Name))
                {
                    result.Name = options.ReplaceConcepts[source.Name];
                }                
            }

            if(source.Left != null)
            {
                result.Left = ConvertLogicalQueryNode(source.Left, options, convertingContext, aliasesDict, processedRuleInstances);
            }
            
            if(source.Right != null)
            {
                result.Right = ConvertLogicalQueryNode(source.Right, options, convertingContext, aliasesDict, processedRuleInstances);
            }

            var sourceParamsList = source.ParamsList;

            if (!sourceParamsList.IsNullOrEmpty())
            {
#if DEBUG
                //_gbcLogger.Info($"sourceParamsList.Count = {sourceParamsList.Count}");
#endif

                var destParametersList = new List<LogicalQueryNode>();

                foreach (var param in sourceParamsList)
                {
                    destParametersList.Add(ConvertLogicalQueryNode(param, options, convertingContext, aliasesDict, processedRuleInstances));
                }

                if(sourceParamsList.Count == 2 && source.Name.NormalizedNameValue == "is")
                {
                    destParametersList.Add(new LogicalQueryNode()
                    {
                        Kind = KindOfLogicalQueryNode.Value,
                        Value = LogicalValue.TrueValue
                    });
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
            //_gbcLogger.Info($"result = {result.ToHumanizedString()}");
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
                dest.Annotations = new List<Annotation>();

                var destAnnotationsList = dest.Annotations;

                foreach (var sourceAnnotation in source.Annotations)
                {
                    var destAnnotation = new Annotation();

                    if(!sourceAnnotation.Facts.IsNullOrEmpty())
                    {
                        destAnnotation.Facts = new List<RuleInstance>();

                        foreach(var fact in sourceAnnotation.Facts)
                        {
                            destAnnotation.Facts.Add(Convert(fact, options, convertingContext));
                        }
                    }

                    destAnnotationsList.Add(destAnnotation);
                }
            }
        }
    }
}
