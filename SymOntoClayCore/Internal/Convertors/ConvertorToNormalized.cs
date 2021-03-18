using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Convertors
{
    public static class ConvertorToNormalized
    {
#if DEBUG
        //private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public static RuleInstance Convert(RuleInstance source)
        {
            var convertingContext = new Dictionary<object, object>();

            return Convert(source, convertingContext);
        }

        public static RuleInstance Convert(RuleInstance source, Dictionary<object, object> convertingContext)
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

            result.PrimaryPart = ConvertPrimaryRulePart(source.PrimaryPart, result, convertingContext);

            if (!source.SecondaryParts.IsNullOrEmpty())
            {
                foreach (var secondaryPart in source.SecondaryParts)
                {
                    result.SecondaryParts.Add(ConvertSecondaryRulePart(secondaryPart, result, convertingContext));
                }
            }

            FillAnnotationsModalitiesAndSections(source, result, convertingContext);

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        private static PrimaryRulePart ConvertPrimaryRulePart(PrimaryRulePart source, RuleInstance ruleInstance, Dictionary<object, object> convertingContext)
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

            FillBaseRulePart(source, result, ruleInstance, convertingContext);

            if (!source.SecondaryParts.IsNullOrEmpty())
            {
                foreach (var secondaryPart in source.SecondaryParts)
                {
                    result.SecondaryParts.Add(ConvertSecondaryRulePart(secondaryPart, ruleInstance, convertingContext));
                }
            }

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        private static SecondaryRulePart ConvertSecondaryRulePart(SecondaryRulePart source, RuleInstance ruleInstance, Dictionary<object, object> convertingContext)
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

            FillBaseRulePart(source, result, ruleInstance, convertingContext);

            result.PrimaryPart = ConvertPrimaryRulePart(source.PrimaryPart, ruleInstance, convertingContext);

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        private static void FillBaseRulePart(BaseRulePart source, BaseRulePart dest, RuleInstance ruleInstance, Dictionary<object, object> convertingContext)
        {
            dest.Parent = ruleInstance;

            dest.Expression = ConvertLogicalQueryNode(source.Expression, convertingContext);

            FillAnnotationsModalitiesAndSections(source, dest, convertingContext);
        }

        private static LogicalQueryNode ConvertLogicalQueryNode(LogicalQueryNode source, Dictionary<object, object> convertingContext)
        {
            switch (source.Kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    switch (source.KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Is:
                            return ConvertIsOperatorLogicalQueryNode(source, convertingContext);

                        case KindOfOperatorOfLogicalQueryNode.And:
                            return ConvertLogicalQueryNodeInDefaultWay(source, convertingContext);

                        case KindOfOperatorOfLogicalQueryNode.Or:
                            return ConvertLogicalQueryNodeInDefaultWay(source, convertingContext);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(source.KindOfOperator), source.KindOfOperator, null);
                    }

                case KindOfLogicalQueryNode.UnaryOperator:
                    switch (source.KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Not:
                            return ConvertLogicalQueryNodeInDefaultWay(source, convertingContext);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(source.KindOfOperator), source.KindOfOperator, null);
                    }

                case KindOfLogicalQueryNode.Concept:
                    return ConvertLogicalQueryNodeInDefaultWay(source, convertingContext);

                case KindOfLogicalQueryNode.Entity:
                    return ConvertLogicalQueryNodeInDefaultWay(source, convertingContext);

                case KindOfLogicalQueryNode.Relation:
                    return ConvertLogicalQueryNodeInDefaultWay(source, convertingContext);

                case KindOfLogicalQueryNode.LogicalVar:
                    return ConvertLogicalQueryNodeInDefaultWay(source, convertingContext);

                case KindOfLogicalQueryNode.QuestionVar:
                    return ConvertLogicalQueryNodeInDefaultWay(source, convertingContext);

                case KindOfLogicalQueryNode.Value:
                    return ConvertLogicalQueryNodeInDefaultWay(source, convertingContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(source.Kind), source.Kind, null);
            }
        }

        private static LogicalQueryNode ConvertLogicalQueryNodeInDefaultWay(LogicalQueryNode source, Dictionary<object, object> convertingContext)
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

            result.Kind = source.Kind;
            result.KindOfOperator = source.KindOfOperator;
            result.Name = source.Name;

            if(source.Left != null)
            {
                result.Left = ConvertLogicalQueryNode(source.Left, convertingContext);
            }
            
            if(source.Right != null)
            {
                result.Right = source.Right;
            }
            
            if(!source.ParamsList.IsNullOrEmpty())
            {
                var destParametersList = new List<LogicalQueryNode>();

                foreach (var param in source.ParamsList)
                {
                    destParametersList.Add(ConvertLogicalQueryNode(param, convertingContext));
                }

                result.ParamsList = destParametersList;
            }

            result.Value = source.Value;
            result.IsQuestion = source.IsQuestion;

            FillAnnotationsModalitiesAndSections(source, result, convertingContext);

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        private static LogicalQueryNode ConvertIsOperatorLogicalQueryNode(LogicalQueryNode source, Dictionary<object, object> convertingContext)
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

            var leftNode = source.Left;
            var rightNode = source.Right;

#if DEBUG
            //_gbcLogger.Info($"leftNode = {leftNode}");
            //_gbcLogger.Info($"rightNode = {rightNode}");
#endif

            var result = new LogicalQueryNode() { Kind = KindOfLogicalQueryNode.Relation };
            convertingContext[source] = result;

            var name = rightNode.Name;

            result.Name = name;

            if (name.KindOfName == KindOfName.QuestionVar)
            {
                result.IsQuestion = true;
            }

            result.ParamsList = new List<LogicalQueryNode>() { leftNode };

            FillAnnotationsModalitiesAndSections(source, result, convertingContext);

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        private static void FillAnnotationsModalitiesAndSections(AnnotatedItem source, AnnotatedItem dest, Dictionary<object, object> convertingContext)
        {
            if (dest.QuantityQualityModalities == null)
            {
                dest.QuantityQualityModalities = new List<Value>();
            }

            if (!source.QuantityQualityModalities.IsNullOrEmpty())
            {
                foreach (var item in source.QuantityQualityModalities)
                {
                    dest.QuantityQualityModalities.Add(item);
                }
            }

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

            dest.Holder = source.Holder;

            if (!source.Annotations.IsNullOrEmpty())
            {
                dest.Annotations = new List<RuleInstance>();

                var destAnnotationsList = dest.Annotations;

                foreach (var sourceAnnotation in source.Annotations)
                {
                    var destAnnotation = Convert(sourceAnnotation, convertingContext);

                    destAnnotationsList.Add(destAnnotation);
                }
            }
        }
    }
}
