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

            result.SubName = ConvertStrongIdentifierValue(source.SubName, mainStorageContext, convertingContext);
            result.SuperName = ConvertStrongIdentifierValue(source.SuperName, mainStorageContext, convertingContext);

            result.Rank = ConvertValue(source.Rank, mainStorageContext, convertingContext);

            result.IsSystemDefined = source.IsSystemDefined;

            result.CalculateLongConditionalHashCode();

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

            result.CalculateLongConditionalHashCode();

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

            result.CalculateLongConditionalHashCode();

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

            result.CalculateLongConditionalHashCode();

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

            result.CalculateLongConditionalHashCode();

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

            result.CalculateLongConditionalHashCode();

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
            result.SystemTask = source.SystemTask;

            result.CalculateLongConditionalHashCode();

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

            result.CalculateLongConditionalHashCode();

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

            result.CalculateLongConditionalHashCode();

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

            result.CalculateLongConditionalHashCode();

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

            result.CalculateLongConditionalHashCode();

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

            result.CalculateLongConditionalHashCode();

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

            var result = new IndexedRuleInstance();
            convertingContext[source] = result;
            result.Origin = source;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            result.Name = ConvertStrongIdentifierValue(source.Name, mainStorageContext, convertingContext);

            result.IsRule = source.IsRule;

            result.PrimaryPart = ConvertPrimaryRulePart(source.PrimaryPart, mainStorageContext, convertingContext);

            if (!source.SecondaryParts.IsNullOrEmpty())
            {
                throw new NotImplementedException();
            }

#if DEBUG
            //_gbcLogger.Info($"result (snapshot) = {result}");
#endif

            result.CalculateLongConditionalHashCode();

            return result;
        }

        private static IndexedPrimaryRulePart ConvertPrimaryRulePart(PrimaryRulePart source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
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

            FillBaseRulePart(source, result, mainStorageContext, convertingContext);

            if(!source.SecondaryParts.IsNullOrEmpty())
            {
                throw new NotImplementedException();
            }

#if DEBUG
            //_gbcLogger.Info($"result (snapshot) = {result}");
#endif

            result.CalculateLongConditionalHashCode();

            return result;
        }

        private static void FillBaseRulePart(BaseRulePart source, IndexedBaseRulePart dest, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            dest.Parent = (IndexedRuleInstance)convertingContext[source.Parent];

            dest.Expression = ConvertLogicalQueryNode(source.Expression, mainStorageContext, convertingContext);
        }

        private static BaseIndexedLogicalQueryNode ConvertLogicalQueryNode(LogicalQueryNode source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            switch(source.Kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    switch(source.KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Is:
                            return ConvertIsOperatorLogicalQueryNode(source, mainStorageContext, convertingContext);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(source.KindOfOperator), source.KindOfOperator, null);
                    }

                case KindOfLogicalQueryNode.Concept:
                    return ConvertConceptLogicalQueryNode(source, mainStorageContext, convertingContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(source.Kind), source.Kind, null);
            }
        }

        private static IsOperatorIndexedLogicalQueryNode ConvertIsOperatorLogicalQueryNode(LogicalQueryNode source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
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
                return (IsOperatorIndexedLogicalQueryNode)convertingContext[source];
            }

#if DEBUG
            //_gbcLogger.Info("NEXT");
#endif

            var result = new IsOperatorIndexedLogicalQueryNode();
            convertingContext[source] = result;
            result.Origin = source;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

            result.Left = ConvertLogicalQueryNode(source.Left, mainStorageContext, convertingContext);
            result.Right = ConvertLogicalQueryNode(source.Right, mainStorageContext, convertingContext);

#if DEBUG
            //_gbcLogger.Info($"result (snapshot) = {result}");
#endif

            result.CalculateLongConditionalHashCode();

            return result;
        }

        private static ConceptIndexedLogicalQueryNode ConvertConceptLogicalQueryNode(LogicalQueryNode source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
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
                return (ConceptIndexedLogicalQueryNode)convertingContext[source];
            }

#if DEBUG
            //_gbcLogger.Info("NEXT");
#endif

            var result = new ConceptIndexedLogicalQueryNode();
            convertingContext[source] = result;
            result.Origin = source;

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

#if DEBUG
            //_gbcLogger.Info($"result (snapshot) = {result}");
#endif

            result.CalculateLongConditionalHashCode();

            return result;
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

            result.CalculateLongConditionalHashCode();

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

            result.CalculateLongConditionalHashCode();

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

            result.CalculateLongConditionalHashCode();

            return result;
        }
    }
}
