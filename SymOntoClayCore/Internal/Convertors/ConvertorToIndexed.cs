using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace SymOntoClay.Core.Internal.Convertors
{
    public static class ConvertorToIndexed
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public static IndexedInheritanceItem ConvertInheritanceItem(InheritanceItem source, IMainStorageContext mainStorageContext)
        {
            var convertingContext = new Dictionary<object, object>();
            return ConvertInheritanceItem(source, mainStorageContext, convertingContext);
        }

        public static IndexedInheritanceItem ConvertInheritanceItem(InheritanceItem source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if(source == null)
            {
                return null;
            }

            if(convertingContext.ContainsKey(source))
            {
                return (IndexedInheritanceItem)convertingContext[source];
            }

            var result = new IndexedInheritanceItem();
            convertingContext[source] = result;
            result.OriginalInheritanceItem = source;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            var dictionary = mainStorageContext.Dictionary;

            result.Key = dictionary.GetKey(source.Id);

            result.SubName = ConvertStrongIdentifierValue(source.SubName, mainStorageContext, convertingContext);
            result.SuperName = ConvertStrongIdentifierValue(source.SuperName, mainStorageContext, convertingContext);

            result.Rank = ConvertValue(source.Rank, mainStorageContext, convertingContext);

            result.IsSystemDefined = source.IsSystemDefined;

            result.KeysOfPrimaryRecords = source.KeysOfPrimaryRecords?.Select(p => dictionary.GetKey(p)).ToList();

            result.CalculateLongHashCodes();

            return result;
        }

        public static IndexedValue ConvertValue(Value source, IMainStorageContext mainStorageContext)
        {
            var convertingContext = new Dictionary<object, object>();
            return ConvertValue(source, mainStorageContext, convertingContext);
        }

        public static IndexedValue ConvertValue(Value source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            switch(source.KindOfValue)
            {
                case KindOfValue.NullValue:
                    return ConvertNullValue(source.AsNullValue, mainStorageContext, convertingContext);

                case KindOfValue.LogicalValue:
                    return ConvertLogicalValue(source.AsLogicalValue, mainStorageContext, convertingContext);

                case KindOfValue.NumberValue:
                    return ConvertNumberValue(source.AsNumberValue, mainStorageContext, convertingContext);

                case KindOfValue.StringValue:
                    return ConvertStringValue(source.AsStringValue, mainStorageContext, convertingContext);

                case KindOfValue.StrongIdentifierValue:
                    return ConvertStrongIdentifierValue(source.AsStrongIdentifierValue, mainStorageContext, convertingContext);

                case KindOfValue.TaskValue:
                    return ConvertTaskValue(source.AsTaskValue, mainStorageContext, convertingContext);

                case KindOfValue.AnnotationValue:
                    return ConvertAnnotationValue(source.AsAnnotationValue, mainStorageContext, convertingContext);

                case KindOfValue.WaypointValue:
                    return ConvertWaypointValue(source.AsWaypointValue, mainStorageContext, convertingContext);

                case KindOfValue.InstanceValue:
                    return ConvertInstanceValue(source.AsInstanceValue, mainStorageContext, convertingContext);

                case KindOfValue.HostValue:
                    return ConvertHostValue(source.AsHostValue, mainStorageContext, convertingContext);

                case KindOfValue.PointRefValue:
                    return ConvertPointRefValue(source.AsPointRefValue, mainStorageContext, convertingContext);

                case KindOfValue.RuleInstanceValue:
                    return ConvertRuleInstanceValue(source.AsRuleInstanceValue, mainStorageContext, convertingContext);

                case KindOfValue.LogicalSearchResultValue:
                    return ConvertLogicalSearchResultValue(source.AsLogicalSearchResultValue, mainStorageContext, convertingContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(source.KindOfValue), source.KindOfValue, null);
            }
        }

        public static IndexedNullValue ConvertNullValue(NullValue source, IMainStorageContext mainStorageContext)
        {
            var convertingContext = new Dictionary<object, object>();
            return ConvertNullValue(source, mainStorageContext, convertingContext);
        }

        public static IndexedNullValue ConvertNullValue(NullValue source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (IndexedNullValue)convertingContext[source];
            }

            var result = new IndexedNullValue();
            convertingContext[source] = result;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            result.OriginalNullValue = source;

            result.CalculateLongHashCodes();

            return result;
        }

        public static IndexedLogicalValue ConvertLogicalValue(LogicalValue source, IMainStorageContext mainStorageContext)
        {
            var convertingContext = new Dictionary<object, object>();
            return ConvertLogicalValue(source, mainStorageContext, convertingContext);
        }

        public static IndexedLogicalValue ConvertLogicalValue(LogicalValue source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (IndexedLogicalValue)convertingContext[source];
            }

            var result = new IndexedLogicalValue();
            convertingContext[source] = result;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            result.OriginalLogicalValue = source;
            result.SystemValue = source.SystemValue;

            result.CalculateLongHashCodes();

            return result;
        }

        public static IndexedNumberValue ConvertNumberValue(NumberValue source, IMainStorageContext mainStorageContext)
        {
            var convertingContext = new Dictionary<object, object>();
            return ConvertNumberValue(source, mainStorageContext, convertingContext);
        }

        public static IndexedNumberValue ConvertNumberValue(NumberValue source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (IndexedNumberValue)convertingContext[source];
            }

            var result = new IndexedNumberValue();
            convertingContext[source] = result;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            result.OriginalNumberValue = source;
            result.SystemValue = source.SystemValue;

            result.CalculateLongHashCodes();

            return result;
        }

        public static IndexedStringValue ConvertStringValue(StringValue source, IMainStorageContext mainStorageContext)
        {
            var convertingContext = new Dictionary<object, object>();
            return ConvertStringValue(source, mainStorageContext, convertingContext);
        }

        public static IndexedStringValue ConvertStringValue(StringValue source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (IndexedStringValue)convertingContext[source];
            }

            var result = new IndexedStringValue();
            convertingContext[source] = result;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            result.OriginalStringValue = source;
            result.SystemValue = source.SystemValue;

            result.CalculateLongHashCodes();

            return result;
        }

        public static IndexedStrongIdentifierValue ConvertStrongIdentifierValue(StrongIdentifierValue source, IMainStorageContext mainStorageContext)
        {
            var convertingContext = new Dictionary<object, object>();
            return ConvertStrongIdentifierValue(source, mainStorageContext, convertingContext);
        }

        public static IndexedStrongIdentifierValue ConvertStrongIdentifierValue(StrongIdentifierValue source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (IndexedStrongIdentifierValue)convertingContext[source];
            }

            var result = new IndexedStrongIdentifierValue();
            convertingContext[source] = result;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            result.OriginalStrongIdentifierValue = source;

            result.DictionaryName = source.DictionaryName;
            result.IsEmpty = source.IsEmpty;
            result.KindOfName = source.KindOfName;
            result.NameValue = source.NameValue;

            if (!result.IsEmpty)
            {
                result.NameKey = mainStorageContext.Dictionary.GetKey(result.NameValue);
            }

            result.CalculateLongHashCodes();

            return result;
        }

        public static IndexedTaskValue ConvertTaskValue(TaskValue source, IMainStorageContext mainStorageContext)
        {
            var convertingContext = new Dictionary<object, object>();
            return ConvertTaskValue(source, mainStorageContext, convertingContext);
        }

        public static IndexedTaskValue ConvertTaskValue(TaskValue source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (IndexedTaskValue)convertingContext[source];
            }

            var result = new IndexedTaskValue();
            convertingContext[source] = result;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            result.OriginalTaskValue = source;

            result.TaskId = source.TaskId;
            result.TaskIdKey = mainStorageContext.Dictionary.GetKey(source.TaskId);
            result.SystemTask = source.SystemTask;

            result.CalculateLongHashCodes();

            return result;
        }

        public static IndexedAnnotationValue ConvertAnnotationValue(AnnotationValue source, IMainStorageContext mainStorageContext)
        {
            var convertingContext = new Dictionary<object, object>();
            return ConvertAnnotationValue(source, mainStorageContext, convertingContext);
        }

        public static IndexedAnnotationValue ConvertAnnotationValue(AnnotationValue source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (IndexedAnnotationValue)convertingContext[source];
            }

            var result = new IndexedAnnotationValue();
            convertingContext[source] = result;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            result.OriginalAnnotationValue = source;
            result.AnnotatedItem = source.AnnotatedItem.GetIndexedAnnotatedItem(mainStorageContext, convertingContext);

            result.CalculateLongHashCodes();

            return result;
        }

        public static IndexedWaypointValue ConvertWaypointValue(WaypointValue source, IMainStorageContext mainStorageContext)
        {
            var convertingContext = new Dictionary<object, object>();
            return ConvertWaypointValue(source, mainStorageContext, convertingContext);
        }

        public static IndexedWaypointValue ConvertWaypointValue(WaypointValue source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (IndexedWaypointValue)convertingContext[source];
            }

            var result = new IndexedWaypointValue();
            convertingContext[source] = result;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            result.OriginalWaypointValue = source;

            result.AbcoluteCoordinates = source.AbcoluteCoordinates;
            result.Name = ConvertStrongIdentifierValue(source.Name, mainStorageContext, convertingContext);

            result.CalculateLongHashCodes();

            return result;
        }

        public static IndexedInstanceValue ConvertInstanceValue(InstanceValue source, IMainStorageContext mainStorageContext)
        {
            var convertingContext = new Dictionary<object, object>();
            return ConvertInstanceValue(source, mainStorageContext, convertingContext);
        }

        public static IndexedInstanceValue ConvertInstanceValue(InstanceValue source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (IndexedInstanceValue)convertingContext[source];
            }

            var result = new IndexedInstanceValue();
            convertingContext[source] = result;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            result.OriginalInstanceValue = source;

            result.InstanceInfo = source.InstanceInfo;

            result.CalculateLongHashCodes();

            return result;
        }

        public static IndexedHostValue ConvertHostValue(HostValue source, IMainStorageContext mainStorageContext)
        {
            var convertingContext = new Dictionary<object, object>();
            return ConvertHostValue(source, mainStorageContext, convertingContext);
        }

        public static IndexedHostValue ConvertHostValue(HostValue source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (IndexedHostValue)convertingContext[source];
            }

            var result = new IndexedHostValue();
            convertingContext[source] = result;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            result.OriginalHostValue = source;

            result.CalculateLongHashCodes();

            return result;
        }

        public static IndexedPointRefValue ConvertPointRefValue(PointRefValue source, IMainStorageContext mainStorageContext)
        {
            var convertingContext = new Dictionary<object, object>();
            return ConvertPointRefValue(source, mainStorageContext, convertingContext);
        }
        
        public static IndexedPointRefValue ConvertPointRefValue(PointRefValue source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (IndexedPointRefValue)convertingContext[source];
            }

            var result = new IndexedPointRefValue();
            convertingContext[source] = result;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            result.OriginalPointRefValue = source;

            result.LeftOperand = ConvertValue(source.LeftOperand, mainStorageContext, convertingContext);
            result.RightOperand = ConvertValue(source.RightOperand, mainStorageContext, convertingContext);

            result.CalculateLongHashCodes();

            return result;
        }

        public static IndexedRuleInstanceValue ConvertRuleInstanceValue(RuleInstanceValue source, IMainStorageContext mainStorageContext)
        {
            var convertingContext = new Dictionary<object, object>();
            return ConvertRuleInstanceValue(source, mainStorageContext, convertingContext);
        }

        public static IndexedRuleInstanceValue ConvertRuleInstanceValue(RuleInstanceValue source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (IndexedRuleInstanceValue)convertingContext[source];
            }

            var result = new IndexedRuleInstanceValue();
            convertingContext[source] = result;
            source.Indexed = result;

            var ruleInstance = source.RuleInstance;

            result.RuleInstance = ruleInstance;

            var indexedRuleInstance = ruleInstance.GetIndexed(mainStorageContext, convertingContext);

            result.IndexedRuleInstance = indexedRuleInstance;

            result.CalculateLongHashCodes();

            return result;
        }

        public static IndexedLogicalSearchResultValue ConvertLogicalSearchResultValue(LogicalSearchResultValue source, IMainStorageContext mainStorageContext)
        {
            var convertingContext = new Dictionary<object, object>();
            return ConvertLogicalSearchResultValue(source, mainStorageContext, convertingContext);
        }

        public static IndexedLogicalSearchResultValue ConvertLogicalSearchResultValue(LogicalSearchResultValue source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (IndexedLogicalSearchResultValue)convertingContext[source];
            }

            var result = new IndexedLogicalSearchResultValue();
            convertingContext[source] = result;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            result.LogicalSearchResult = source.LogicalSearchResult;

            result.CalculateLongHashCodes();

            return result;
        }

        private static void FillAnnotationsModalitiesAndSections(AnnotatedItem source, IndexedAnnotatedItem dest, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (dest.QuantityQualityModalities == null)
            {
                dest.QuantityQualityModalities = new List<IndexedValue>();
            }

            if(!source.QuantityQualityModalities.IsNullOrEmpty())
            {
                foreach(var item in source.QuantityQualityModalities)
                {
                    dest.QuantityQualityModalities.Add(ConvertValue(item, mainStorageContext, convertingContext));
                }
            }

            if(dest.WhereSection == null)
            {
                dest.WhereSection = new List<IndexedValue>();
            }

            if(!source.WhereSection.IsNullOrEmpty())
            {
                foreach(var item in source.WhereSection)
                {
                    dest.WhereSection.Add(ConvertValue(item, mainStorageContext, convertingContext));
                }
            }

            dest.Holder = ConvertStrongIdentifierValue(source.Holder, mainStorageContext, convertingContext);

            var dictionary = mainStorageContext.Dictionary;

            if (!source.Annotations.IsNullOrEmpty())
            {
                dest.Annotations = new List<IndexedLogicalAnnotation>();

                var destAnnotationsList = dest.Annotations;

                foreach(var sourceAnnotation in source.Annotations)
                {
                    var ruleInstanceKey = dictionary.GetKey(sourceAnnotation.Name.NameValue);

                    var destAnnotation = new IndexedLogicalAnnotation()
                    {
                        RuleInstanceKey = ruleInstanceKey
                    };

                    destAnnotationsList.Add(destAnnotation);
                }
            }
        }

        public static IndexedRuleInstance ConvertRuleInstance(RuleInstance source, IMainStorageContext mainStorageContext)
        {
            var convertingContext = new Dictionary<object, object>();

            var result = ConvertRuleInstance(source, mainStorageContext, convertingContext);

#if DEBUG
            //_gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        public static IndexedRuleInstance ConvertRuleInstance(RuleInstance source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
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
                return (IndexedRuleInstance)convertingContext[source];
            }

#if DEBUG
            //_gbcLogger.Info("NEXT");
#endif

            var dictionary = mainStorageContext.Dictionary;

            var result = new IndexedRuleInstance();
            convertingContext[source] = result;
            result.Origin = source;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            result.Kind = source.Kind;

            result.Name = ConvertStrongIdentifierValue(source.Name, mainStorageContext, convertingContext);

            result.Key = dictionary.GetKey(source.Name.NameValue);

            result.PrimaryPart = ConvertPrimaryRulePart(source.PrimaryPart, result, mainStorageContext, convertingContext);

            if (!source.SecondaryParts.IsNullOrEmpty())
            {
                foreach (var secondaryPart in source.SecondaryParts)
                {
                    result.SecondaryParts.Add(ConvertSecondaryRulePart(secondaryPart, result, mainStorageContext, convertingContext));
                }
            }

            result.KeysOfPrimaryRecords = source.KeysOfPrimaryRecords?.Select(p => dictionary.GetKey(p)).ToList();

#if DEBUG
            _gbcLogger.Info($"result (snapshot) = {result}");
#endif

            result.CalculateLongHashCodes();

            return result;
        }

        private static IndexedPrimaryRulePart ConvertPrimaryRulePart(PrimaryRulePart source, IndexedRuleInstance ruleInstance, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
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
                return (IndexedPrimaryRulePart)convertingContext[source];
            }

#if DEBUG
            //_gbcLogger.Info("NEXT");
#endif

            var result = new IndexedPrimaryRulePart();
            convertingContext[source] = result;
            result.OriginPrimaryRulePart = source;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            FillBaseRulePart(source, result, ruleInstance, mainStorageContext, convertingContext);

            if(!source.SecondaryParts.IsNullOrEmpty())
            {
                foreach(var secondaryPart in source.SecondaryParts)
                {
                    result.SecondaryParts.Add(ConvertSecondaryRulePart(secondaryPart, ruleInstance, mainStorageContext, convertingContext));
                }              
            }

#if DEBUG
            _gbcLogger.Info($"result (snapshot) = {result}");
#endif

            result.CalculateLongHashCodes();

            return result;
        }

        private static IndexedSecondaryRulePart ConvertSecondaryRulePart(SecondaryRulePart source, IndexedRuleInstance ruleInstance, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
#if DEBUG
            _gbcLogger.Info($"source = {source}");
#endif

            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (IndexedSecondaryRulePart)convertingContext[source];
            }

            var result = new IndexedSecondaryRulePart();
            convertingContext[source] = result;
            result.OriginalSecondaryRulePart = source;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            FillBaseRulePart(source, result, ruleInstance, mainStorageContext, convertingContext);

            result.PrimaryPart = ConvertPrimaryRulePart(source.PrimaryPart, ruleInstance, mainStorageContext, convertingContext);

#if DEBUG
            _gbcLogger.Info($"result (snapshot) = {result}");
#endif

            result.CalculateLongHashCodes();

            return result;
        }

        private static void FillBaseRulePart(BaseRulePart source, IndexedBaseRulePart dest, IndexedRuleInstance ruleInstance, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            var contextOfConvertingExpressionNode = new ContextOfConvertingExpressionNode();

            dest.Parent = (IndexedRuleInstance)convertingContext[source.Parent];

            dest.Expression = ConvertLogicalQueryNode(source.Expression, dest, ruleInstance, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);

            dest.RelationsDict = contextOfConvertingExpressionNode.RelationsList.GroupBy(p => p.Key).ToDictionary(p => p.Key, p => (IList<RelationIndexedLogicalQueryNode>)p.ToList());

            dest.IsActive = source.IsActive;
            dest.HasQuestionVars = contextOfConvertingExpressionNode.HasQuestionVars;
            dest.HasVars = contextOfConvertingExpressionNode.HasVars;
        }

        private static BaseIndexedLogicalQueryNode ConvertLogicalQueryNode(LogicalQueryNode source, IndexedBaseRulePart rulePart, IndexedRuleInstance ruleInstance, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext, ContextOfConvertingExpressionNode contextOfConvertingExpressionNode)
        {
            switch(source.Kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    switch(source.KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Is:
                            return ConvertIsOperatorLogicalQueryNode(source, rulePart, ruleInstance, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);

                        case KindOfOperatorOfLogicalQueryNode.And:
                            return ConvertAndOperatorIndexedLogicalQueryNode(source, rulePart, ruleInstance, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);

                        case KindOfOperatorOfLogicalQueryNode.Or:
                            return ConvertOrOperatorIndexedLogicalQueryNode(source, rulePart, ruleInstance, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(source.KindOfOperator), source.KindOfOperator, null);
                    }

                case KindOfLogicalQueryNode.UnaryOperator:
                    switch (source.KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Not:
                            return ConvertNotOperatorIndexedLogicalQueryNode(source, rulePart, ruleInstance, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(source.KindOfOperator), source.KindOfOperator, null);
                    }

                case KindOfLogicalQueryNode.Concept:
                    return ConvertConceptLogicalQueryNode(source, rulePart, ruleInstance, mainStorageContext, convertingContext);

                case KindOfLogicalQueryNode.Entity:
                    return ConvertEntityIndexedLogicalQueryNode(source, rulePart, ruleInstance, mainStorageContext, convertingContext);

                case KindOfLogicalQueryNode.Relation:
                    return ConvertRelationIndexedLogicalQueryNode(source, rulePart, ruleInstance, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);

                case KindOfLogicalQueryNode.LogicalVar:
                    return ConvertLogicalVarIndexedLogicalQueryNode(source, rulePart, ruleInstance, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);

                case KindOfLogicalQueryNode.QuestionVar:
                    return ConvertQuestionVarIndexedLogicalQueryNode(source, rulePart, ruleInstance, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);

                default:
                    throw new ArgumentOutOfRangeException(nameof(source.Kind), source.Kind, null);
            }
        }

        private static AndOperatorIndexedLogicalQueryNode ConvertAndOperatorIndexedLogicalQueryNode(LogicalQueryNode source, IndexedBaseRulePart rulePart, IndexedRuleInstance ruleInstance, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext, ContextOfConvertingExpressionNode contextOfConvertingExpressionNode)
        {
#if DEBUG
            _gbcLogger.Info($"source = {source}");
#endif

            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (AndOperatorIndexedLogicalQueryNode)convertingContext[source];
            }

            var result = new AndOperatorIndexedLogicalQueryNode();
            convertingContext[source] = result;

            FillBaseIndexedLogicalQueryNode(result, source, rulePart, ruleInstance, mainStorageContext, convertingContext);

            result.Left = ConvertLogicalQueryNode(source.Left, rulePart, ruleInstance, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);
            result.Right = ConvertLogicalQueryNode(source.Right, rulePart, ruleInstance, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);

#if DEBUG
            _gbcLogger.Info($"result = {result}");
#endif

            result.CalculateLongHashCodes();

            return result;
        }
        
        private static OrOperatorIndexedLogicalQueryNode ConvertOrOperatorIndexedLogicalQueryNode(LogicalQueryNode source, IndexedBaseRulePart rulePart, IndexedRuleInstance ruleInstance, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext, ContextOfConvertingExpressionNode contextOfConvertingExpressionNode)
        {
#if DEBUG
            _gbcLogger.Info($"source = {source}");
#endif

            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (OrOperatorIndexedLogicalQueryNode)convertingContext[source];
            }

            var result = new OrOperatorIndexedLogicalQueryNode();
            convertingContext[source] = result;

            FillBaseIndexedLogicalQueryNode(result, source, rulePart, ruleInstance, mainStorageContext, convertingContext);

            result.Left = ConvertLogicalQueryNode(source.Left, rulePart, ruleInstance, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);
            result.Right = ConvertLogicalQueryNode(source.Right, rulePart, ruleInstance, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);

#if DEBUG
            _gbcLogger.Info($"result = {result}");
#endif
            result.CalculateLongHashCodes();

            return result;
        }

        private static BaseIndexedLogicalQueryNode ConvertIsOperatorLogicalQueryNode(LogicalQueryNode source, IndexedBaseRulePart rulePart, IndexedRuleInstance ruleInstance, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext, ContextOfConvertingExpressionNode contextOfConvertingExpressionNode)
        {
#if DEBUG
            _gbcLogger.Info($"source = {source}");
#endif

            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (BaseIndexedLogicalQueryNode)convertingContext[source];
            }

            var leftNode = source.Left;
            var rightNode = source.Right;

#if DEBUG
            _gbcLogger.Info($"leftNode = {leftNode}");
            _gbcLogger.Info($"rightNode = {rightNode}");
#endif

            var result = new RelationIndexedLogicalQueryNode();
            convertingContext[source] = result;

            FillBaseIndexedLogicalQueryNode(result, source, rulePart, ruleInstance, mainStorageContext, convertingContext);

            var name = rightNode.Name;

            result.Key = mainStorageContext.Dictionary.GetKey(name.NameValue);

            if(name.KindOfName == KindOfName.QuestionVar)
            {
                result.IsQuestion = true;
            }

            var sourceParamsList = new List<LogicalQueryNode>() { leftNode };

            FillRelationParams(result, sourceParamsList, rulePart, ruleInstance, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);

            //1;
            //result.Params = new List<BaseIndexedLogicalQueryNode>() { ConvertLogicalQueryNode(leftNode, rulePart, ruleInstance, mainStorageContext, convertingContext, contextOfConvertingExpressionNode) };

#if DEBUG
            _gbcLogger.Info($"result = {result}");
#endif

            result.CalculateLongHashCodes();

            return result;
        }

        private static NotOperatorIndexedLogicalQueryNode ConvertNotOperatorIndexedLogicalQueryNode(LogicalQueryNode source, IndexedBaseRulePart rulePart, IndexedRuleInstance ruleInstance, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext, ContextOfConvertingExpressionNode contextOfConvertingExpressionNode)
        {
#if DEBUG
            _gbcLogger.Info($"source = {source}");
#endif

            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (NotOperatorIndexedLogicalQueryNode)convertingContext[source];
            }

            var result = new NotOperatorIndexedLogicalQueryNode();
            convertingContext[source] = result;

            FillBaseIndexedLogicalQueryNode(result, source, rulePart, ruleInstance, mainStorageContext, convertingContext);

            result.Left = ConvertLogicalQueryNode(source.Left, rulePart, ruleInstance, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);

#if DEBUG
            _gbcLogger.Info($"result = {result}");
#endif
            result.CalculateLongHashCodes();

            return result;
        }

        private static RelationIndexedLogicalQueryNode ConvertRelationIndexedLogicalQueryNode(LogicalQueryNode source, IndexedBaseRulePart rulePart, IndexedRuleInstance ruleInstance, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext, ContextOfConvertingExpressionNode contextOfConvertingExpressionNode)
        {
#if DEBUG
            _gbcLogger.Info($"source = {source}");
#endif

            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (RelationIndexedLogicalQueryNode)convertingContext[source];
            }

            var result = new RelationIndexedLogicalQueryNode();
            convertingContext[source] = result;

            FillBaseIndexedLogicalQueryNode(result, source, rulePart, ruleInstance, mainStorageContext, convertingContext);

            var name = source.Name;

            result.Key = mainStorageContext.Dictionary.GetKey(name.NameValue);

            if (name.KindOfName == KindOfName.QuestionVar)
            {
                result.IsQuestion = true;
            }

            FillRelationParams(result, source.ParamsList, rulePart, ruleInstance, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);

#if DEBUG
            _gbcLogger.Info($"result = {result}");
#endif

            result.CalculateLongHashCodes();

            return result;
        }

        private static void FillRelationParams(RelationIndexedLogicalQueryNode dest, IList<LogicalQueryNode> sourceParamsList, IndexedBaseRulePart rulePart, IndexedRuleInstance ruleInstance, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext, ContextOfConvertingExpressionNode contextOfConvertingExpressionNode)
        {
            dest.CountParams = sourceParamsList.Count;

            var parametersList = new List<BaseIndexedLogicalQueryNode>();
            var varsInfoList = new List<QueryExecutingCardAboutVar>();
            var knownInfoList = new List<QueryExecutingCardAboutKnownInfo>();
            var i = 0;

            var dictionary = mainStorageContext.Dictionary;

            foreach (var param in sourceParamsList)
            {
#if DEBUG
                _gbcLogger.Info($"param = {param}");
#endif

                var resultParam = ConvertLogicalQueryNode(param, rulePart, ruleInstance, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);

#if DEBUG
                _gbcLogger.Info($"resultParam = {resultParam}");
#endif

                parametersList.Add(resultParam);
                var kindOfParam = param.Kind;
                switch (kindOfParam)
                {
                    case KindOfLogicalQueryNode.Concept:
                        {
                            var originParam = param;
                            var knownInfo = new QueryExecutingCardAboutKnownInfo();
                            knownInfo.Kind = kindOfParam;
                            knownInfo.Expression = resultParam;
                            knownInfo.Position = i;
                            knownInfo.Key = dictionary.GetKey(originParam.Name.NameValue);
                            knownInfoList.Add(knownInfo);
                        }
                        break;

                    case KindOfLogicalQueryNode.Entity:
                        {
                            var originParam = param;
                            var knownInfo = new QueryExecutingCardAboutKnownInfo();
                            knownInfo.Kind = kindOfParam;
                            knownInfo.Expression = resultParam;
                            knownInfo.Position = i;
                            knownInfo.Key = dictionary.GetKey(originParam.Name.NameValue);
                            knownInfoList.Add(knownInfo);
                        }
                        break;

                    //case KindOfLogicalQueryNode.EntityRef:
                    //    {
                    //        var originParam = param.AsEntityRef;
                    //        var knownInfo = new QueryExecutingCardAboutKnownInfo();
                    //        knownInfo.Kind = kindOfParam;
                    //        knownInfo.Expression = param;
                    //        knownInfo.Position = i;
                    //        knownInfo.Key = originParam.Key;
                    //        knownInfoList.Add(knownInfo);
                    //    }
                    //    break;

                    //case KindOfLogicalQueryNode.EntityCondition:
                    //    {
                    //        var originParam = param.AsEntityCondition;
                    //        var knownInfo = new QueryExecutingCardAboutKnownInfo();
                    //        knownInfo.Kind = kindOfParam;
                    //        knownInfo.Expression = param;
                    //        knownInfo.Position = i;
                    //        knownInfo.Key = originParam.Key;
                    //        knownInfoList.Add(knownInfo);
                    //    }
                    //    break;

                    case KindOfLogicalQueryNode.LogicalVar:
                        {
                            var originParam = param;
                            var varInfo = new QueryExecutingCardAboutVar();
                            varInfo.KeyOfVar = dictionary.GetKey(originParam.Name.NameValue);
                            varInfo.Position = i;
                            varsInfoList.Add(varInfo);
                        }
                        break;

                    case KindOfLogicalQueryNode.QuestionVar:
                        {
                            var originParam = param;
                            var varInfo = new QueryExecutingCardAboutVar();
                            varInfo.KeyOfVar = dictionary.GetKey(originParam.Name.NameValue);
                            varInfo.Position = i;
                            varsInfoList.Add(varInfo);
                        }
                        break;

                    //case KindOfLogicalQueryNode.Value:
                    //    {
                    //        var originParam = param.AsValue;
                    //        var knownInfo = new QueryExecutingCardAboutKnownInfo();
                    //        knownInfo.Kind = kindOfParam;
                    //        knownInfo.Expression = param;
                    //        knownInfo.Position = i;
                    //        knownInfo.Value = originParam.Value;
                    //        knownInfoList.Add(knownInfo);
                    //    }
                    //    break;

                    //case KindOfLogicalQueryNode.FuzzyLogicValue:
                    //    {
                    //        var originParam = param.AsFuzzyLogicValue;
                    //        var knownInfo = new QueryExecutingCardAboutKnownInfo();
                    //        knownInfo.Kind = kindOfParam;
                    //        knownInfo.Expression = param;
                    //        knownInfo.Position = i;
                    //        knownInfo.Value = originParam.Value;
                    //        knownInfoList.Add(knownInfo);
                    //    }
                    //    break;

                    //case KindOfLogicalQueryNode.Fact:
                    //    {
                    //        var originParam = param.AsFact;
                    //        var knownInfo = new QueryExecutingCardAboutKnownInfo();
                    //        knownInfo.Kind = kindOfParam;
                    //        knownInfo.Expression = param;
                    //        knownInfo.Position = i;
                    //        knownInfo.Key = originParam.Key;
                    //        knownInfoList.Add(knownInfo);
                    //    }
                    //    break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfParam), kindOfParam, null);
                }
                i++;
            }
            dest.Params = parametersList;
            dest.VarsInfoList = varsInfoList;
            dest.KnownInfoList = knownInfoList;

            contextOfConvertingExpressionNode.RelationsList.Add(dest);
        }

        private static LogicalVarIndexedLogicalQueryNode ConvertLogicalVarIndexedLogicalQueryNode(LogicalQueryNode source, IndexedBaseRulePart rulePart, IndexedRuleInstance ruleInstance, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext, ContextOfConvertingExpressionNode contextOfConvertingExpressionNode)
        {
#if DEBUG
            _gbcLogger.Info($"source = {source}");
#endif

            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (LogicalVarIndexedLogicalQueryNode)convertingContext[source];
            }

            var result = new LogicalVarIndexedLogicalQueryNode();
            convertingContext[source] = result;

            FillBaseIndexedLogicalQueryNode(result, source, rulePart, ruleInstance, mainStorageContext, convertingContext);

            result.Key = mainStorageContext.Dictionary.GetKey(source.Name.NameValue);

#if DEBUG
            _gbcLogger.Info($"result (snapshot) = {result}");
#endif

            contextOfConvertingExpressionNode.HasVars = true;

            result.CalculateLongHashCodes();

            return result;
        }

        private static QuestionVarIndexedLogicalQueryNode ConvertQuestionVarIndexedLogicalQueryNode(LogicalQueryNode source, IndexedBaseRulePart rulePart, IndexedRuleInstance ruleInstance, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext, ContextOfConvertingExpressionNode contextOfConvertingExpressionNode)
        {
#if DEBUG
            _gbcLogger.Info($"source = {source}");
#endif

            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (QuestionVarIndexedLogicalQueryNode)convertingContext[source];
            }

            var result = new QuestionVarIndexedLogicalQueryNode();

            convertingContext[source] = result;

            FillBaseIndexedLogicalQueryNode(result, source, rulePart, ruleInstance, mainStorageContext, convertingContext);

            result.Key = mainStorageContext.Dictionary.GetKey(source.Name.NameValue);

#if DEBUG
            _gbcLogger.Info($"result (snapshot) = {result}");
#endif

            result.CalculateLongHashCodes();

            return result;
        }

        private static ConceptIndexedLogicalQueryNode ConvertConceptLogicalQueryNode(LogicalQueryNode source, IndexedBaseRulePart rulePart, IndexedRuleInstance ruleInstance, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
#if DEBUG
            _gbcLogger.Info($"source = {source}");
#endif

            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (ConceptIndexedLogicalQueryNode)convertingContext[source];
            }

#if DEBUG
            _gbcLogger.Info("NEXT");
#endif

            var result = new ConceptIndexedLogicalQueryNode();
            convertingContext[source] = result;

            FillBaseIndexedLogicalQueryNode(result, source, rulePart, ruleInstance, mainStorageContext, convertingContext);

            result.Key = mainStorageContext.Dictionary.GetKey(source.Name.NameValue);

#if DEBUG
            _gbcLogger.Info($"result (snapshot) = {result}");
#endif

            result.CalculateLongHashCodes();

            return result;
        }

        private static EntityIndexedLogicalQueryNode ConvertEntityIndexedLogicalQueryNode(LogicalQueryNode source, IndexedBaseRulePart rulePart, IndexedRuleInstance ruleInstance, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
#if DEBUG
            _gbcLogger.Info($"source = {source}");
#endif

            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (EntityIndexedLogicalQueryNode)convertingContext[source];
            }

#if DEBUG
            _gbcLogger.Info("NEXT");
#endif

            var result = new EntityIndexedLogicalQueryNode();
            convertingContext[source] = result;

            FillBaseIndexedLogicalQueryNode(result, source, rulePart, ruleInstance, mainStorageContext, convertingContext);

            result.Key = mainStorageContext.Dictionary.GetKey(source.Name.NameValue);

#if DEBUG
            _gbcLogger.Info($"result (snapshot) = {result}");
#endif

            result.CalculateLongHashCodes();

            return result;
        }

        private static void FillBaseIndexedLogicalQueryNode(BaseIndexedLogicalQueryNode dest, LogicalQueryNode source, IndexedBaseRulePart rulePart, IndexedRuleInstance ruleInstance, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            dest.Origin = source;
            dest.RuleInstance = ruleInstance;
            dest.RulePart = rulePart;

            FillAnnotationsModalitiesAndSections(source, dest, mainStorageContext, convertingContext);
        }

        public static IndexedOperator ConvertOperator(Operator source, IMainStorageContext mainStorageContext)
        {
            var convertingContext = new Dictionary<object, object>();
            return ConvertOperator(source, mainStorageContext, convertingContext);
        }

        public static IndexedOperator ConvertOperator(Operator source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (IndexedOperator)convertingContext[source];
            }

            var result = new IndexedOperator();
            convertingContext[source] = result;
            result.OriginalOperator = source;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            result.KindOfOperator = source.KindOfOperator;
            result.IsSystemDefined = source.IsSystemDefined;

            if(result.IsSystemDefined)
            {
                result.SystemHandler = source.SystemHandler;
            }
            else
            {
                result.CompiledFunctionBody = mainStorageContext.Compiler.Compile(source.Statements);
            }

            result.CalculateLongHashCodes();

            return result;
        }

        public static IndexedChannel ConvertChannel(Channel source, IMainStorageContext mainStorageContext)
        {
            var convertingContext = new Dictionary<object, object>();
            return ConvertChannel(source, mainStorageContext, convertingContext);
        }

        public static IndexedChannel ConvertChannel(Channel source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (IndexedChannel)convertingContext[source];
            }

            var result = new IndexedChannel();
            convertingContext[source] = result;
            result.OriginalChannel = source;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            result.Name = ConvertStrongIdentifierValue(source.Name, mainStorageContext, convertingContext);
            result.Handler = source.Handler;

            result.CalculateLongHashCodes();

            return result;
        }

        public static IndexedInlineTrigger ConvertInlineTrigger(InlineTrigger source, IMainStorageContext mainStorageContext)
        {
            var convertingContext = new Dictionary<object, object>();
            return ConvertInlineTrigger(source, mainStorageContext, convertingContext);
        }

        public static IndexedInlineTrigger ConvertInlineTrigger(InlineTrigger source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (IndexedInlineTrigger)convertingContext[source];
            }

            var result = new IndexedInlineTrigger();
            convertingContext[source] = result;
            result.OriginalInlineTrigger = source;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            result.Kind = source.Kind;
            result.KindOfSystemEvent = source.KindOfSystemEvent;

            result.CompiledFunctionBody = mainStorageContext.Compiler.Compile(source.Statements);

            result.CalculateLongHashCodes();

            return result;
        }
    }
}
