using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public static class ConvertorToIndexed
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public static IndexedInheritanceItem ConvertInheritanceItem(InheritanceItem source, IEntityDictionary entityDictionary)
        {
            var convertingContext = new Dictionary<object, object>();
            return ConvertInheritanceItem(source, entityDictionary, convertingContext);
        }

        private static IndexedInheritanceItem ConvertInheritanceItem(InheritanceItem source, IEntityDictionary entityDictionary, Dictionary<object, object> convertingContext)
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

            FillAnnotationsModalitiesAndSections(source, result, entityDictionary, convertingContext);

            result.SubName = source.SubName;
            result.SuperName = source.SuperName;

            result.Rank = ConvertValue(source.Rank, entityDictionary, convertingContext);

            result.IsSystemDefined = source.IsSystemDefined;

            return result;
        }

        private static IndexedValue ConvertValue(Value source, IEntityDictionary entityDictionary, Dictionary<object, object> convertingContext)
        {
            switch(source.KindOfValue)
            {
                case KindOfValue.NullValue:
                    return NConvertNullValue(source.AsNullValue, entityDictionary, convertingContext);

                case KindOfValue.LogicalValue:
                    return NConvertLogicalValue(source.AsLogicalValue, entityDictionary, convertingContext);

                case KindOfValue.NumberValue:
                    return NConvertNumberValue(source.AsNumberValue, entityDictionary, convertingContext);

                case KindOfValue.StringValue:
                    return NConvertStringValue(source.AsStringValue, entityDictionary, convertingContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(source.KindOfValue), source.KindOfValue, null);
            }
        }

        private static IndexedNullValue NConvertNullValue(NullValue source, IEntityDictionary entityDictionary, Dictionary<object, object> convertingContext)
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

            FillAnnotationsModalitiesAndSections(source, result, entityDictionary, convertingContext);

            result.OriginalNullValue = source;

            return result;
        }

        private static IndexedLogicalValue NConvertLogicalValue(LogicalValue source, IEntityDictionary entityDictionary, Dictionary<object, object> convertingContext)
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

            FillAnnotationsModalitiesAndSections(source, result, entityDictionary, convertingContext);

            result.OriginalLogicalValue = source;
            result.SystemValue = source.SystemValue;

            return result;
        }

        private static IndexedNumberValue NConvertNumberValue(NumberValue source, IEntityDictionary entityDictionary, Dictionary<object, object> convertingContext)
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

            FillAnnotationsModalitiesAndSections(source, result, entityDictionary, convertingContext);

            result.OriginalNumberValue = source;
            result.SystemValue = source.SystemValue;

            return result;
        }

        private static IndexedStringValue NConvertStringValue(StringValue source, IEntityDictionary entityDictionary, Dictionary<object, object> convertingContext)
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

            FillAnnotationsModalitiesAndSections(source, result, entityDictionary, convertingContext);

            result.OriginalStringValue = source;
            result.SystemValue = source.SystemValue;

            return result;
        }

        private static void FillAnnotationsModalitiesAndSections(AnnotatedItem source, IndexedAnnotatedItem dest, IEntityDictionary entityDictionary, Dictionary<object, object> convertingContext)
        {
            if(dest.QuantityQualityModalities == null)
            {
                dest.QuantityQualityModalities = new List<IndexedValue>();
            }

            if(!source.QuantityQualityModalities.IsNullOrEmpty())
            {
                foreach(var item in source.QuantityQualityModalities)
                {
                    dest.QuantityQualityModalities.Add(ConvertValue(item, entityDictionary, convertingContext));
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
                    dest.WhereSection.Add(ConvertValue(item, entityDictionary, convertingContext));
                }
            }
        }

        public static IndexedRuleInstance ConvertRuleInstance(RuleInstance source, IEntityDictionary entityDictionary)
        {
            var convertingContext = new Dictionary<object, object>();

            var result = ConvertRuleInstance(source, entityDictionary, convertingContext);

#if DEBUG
            _gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        private static IndexedRuleInstance ConvertRuleInstance(RuleInstance source, IEntityDictionary entityDictionary, Dictionary<object, object> convertingContext)
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
                return (IndexedRuleInstance)convertingContext[source];
            }

#if DEBUG
            _gbcLogger.Info("NEXT");
#endif

            var result = new IndexedRuleInstance();
            convertingContext[source] = result;
            result.Origin = source;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, entityDictionary, convertingContext);

            result.Name = source.Name;

            result.IsRule = source.IsRule;

            result.PrimaryPart = ConvertPrimaryRulePart(source.PrimaryPart, entityDictionary, convertingContext);

            if (!source.SecondaryParts.IsNullOrEmpty())
            {
                throw new NotImplementedException();
            }

#if DEBUG
            _gbcLogger.Info($"result (snapshot) = {result}");
#endif

            return result;
        }

        private static IndexedPrimaryRulePart ConvertPrimaryRulePart(PrimaryRulePart source, IEntityDictionary entityDictionary, Dictionary<object, object> convertingContext)
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
                return (IndexedPrimaryRulePart)convertingContext[source];
            }

#if DEBUG
            _gbcLogger.Info("NEXT");
#endif

            var result = new IndexedPrimaryRulePart();
            convertingContext[source] = result;
            result.OriginPrimaryRulePart = source;
            source.Indexed = result;

            FillAnnotationsModalitiesAndSections(source, result, entityDictionary, convertingContext);

            FillBaseRulePart(source, result, entityDictionary, convertingContext);

            if(!source.SecondaryParts.IsNullOrEmpty())
            {
                throw new NotImplementedException();
            }

#if DEBUG
            _gbcLogger.Info($"result (snapshot) = {result}");
#endif

            return result;
        }

        private static void FillBaseRulePart(BaseRulePart source, IndexedBaseRulePart dest, IEntityDictionary entityDictionary, Dictionary<object, object> convertingContext)
        {
            dest.Parent = (IndexedRuleInstance)convertingContext[source.Parent];

            dest.Expression = ConvertLogicalQueryNode(source.Expression, entityDictionary, convertingContext);
        }

        private static BaseIndexedLogicalQueryNode ConvertLogicalQueryNode(LogicalQueryNode source, IEntityDictionary entityDictionary, Dictionary<object, object> convertingContext)
        {
            switch(source.Kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    switch(source.KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Is:
                            return ConvertIsOperatorLogicalQueryNode(source, entityDictionary, convertingContext);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(source.KindOfOperator), source.KindOfOperator, null);
                    }

                case KindOfLogicalQueryNode.Concept:
                    return ConvertConceptLogicalQueryNode(source, entityDictionary, convertingContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(source.Kind), source.Kind, null);
            }
        }

        private static IsOperatorIndexedLogicalQueryNode ConvertIsOperatorLogicalQueryNode(LogicalQueryNode source, IEntityDictionary entityDictionary, Dictionary<object, object> convertingContext)
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
                return (IsOperatorIndexedLogicalQueryNode)convertingContext[source];
            }

#if DEBUG
            _gbcLogger.Info("NEXT");
#endif

            var result = new IsOperatorIndexedLogicalQueryNode();
            convertingContext[source] = result;
            result.Origin = source;

            FillAnnotationsModalitiesAndSections(source, result, entityDictionary, convertingContext);

            result.Left = ConvertLogicalQueryNode(source.Left, entityDictionary, convertingContext);
            result.Right = ConvertLogicalQueryNode(source.Right, entityDictionary, convertingContext);

#if DEBUG
            _gbcLogger.Info($"result (snapshot) = {result}");
#endif

            return result;
        }

        private static ConceptIndexedLogicalQueryNode ConvertConceptLogicalQueryNode(LogicalQueryNode source, IEntityDictionary entityDictionary, Dictionary<object, object> convertingContext)
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
            result.Origin = source;

            FillAnnotationsModalitiesAndSections(source, result, entityDictionary, convertingContext);

#if DEBUG
            _gbcLogger.Info($"result (snapshot) = {result}");
#endif

            return result;
        }

        public static IndexedOperator ConvertOperator(Operator source, IEntityDictionary entityDictionary)
        {
            var convertingContext = new Dictionary<object, object>();

            return ConvertOperator(source, entityDictionary, convertingContext);
        }

        private static IndexedOperator ConvertOperator(Operator source, IEntityDictionary entityDictionary, Dictionary<object, object> convertingContext)
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

            FillAnnotationsModalitiesAndSections(source, result, entityDictionary, convertingContext);

            result.KindOfOperator = source.KindOfOperator;
            result.IsSystemDefined = source.IsSystemDefined;

            if(result.IsSystemDefined)
            {
                result.SystemHandler = source.SystemHandler;
            }
            else
            {
                //CompiledFunctionBody
                throw new NotImplementedException();
            }

            return result;
        }
    }
}
