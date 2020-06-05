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

            FillAnnotationsModalitiesAndSections(source, result, entityDictionary, convertingContext);

            result.SubNames = NameHelpers.CreateSimpleNames(source.SubName, entityDictionary);
            result.SuperNames = NameHelpers.CreateSimpleNames(source.SuperName, entityDictionary);

            result.Rank = ConvertValue(source.Rank, entityDictionary, convertingContext);

            result.IsSystemDefined = source.IsSystemDefined;

            return result;
        }

        private static IndexedValue ConvertValue(Value source, IEntityDictionary entityDictionary, Dictionary<object, object> convertingContext)
        {
            switch(source.Kind)
            {
                case KindOfValue.LogicalValue:
                    return NConvertLogicalValue(source.AsLogicalValue, entityDictionary, convertingContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(source.Kind), source.Kind, null);
            }
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

            FillAnnotationsModalitiesAndSections(source, result, entityDictionary, convertingContext);

            result.OriginalLogicalValue = source;
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

            FillAnnotationsModalitiesAndSections(source, result, entityDictionary, convertingContext);

            result.Names = NameHelpers.CreateSimpleNames(source.Name, entityDictionary);

            result.IsRule = source.IsRule;

            result.PrimaryPart = ConvertPrimaryRulePart(source.PrimaryPart, entityDictionary, convertingContext);

#if DEBUG
            _gbcLogger.Info($"result (snapshot) = {result}");
#endif

            throw new NotImplementedException();
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

            FillAnnotationsModalitiesAndSections(source, result, entityDictionary, convertingContext);

            FillBaseRulePart(source, result, entityDictionary, convertingContext);

#if DEBUG
            _gbcLogger.Info($"result (snapshot) = {result}");
#endif

            throw new NotImplementedException();
        }

        private static void FillBaseRulePart(BaseRulePart source, IndexedBaseRulePart dest, IEntityDictionary entityDictionary, Dictionary<object, object> convertingContext)
        {
            dest.Parent = (IndexedRuleInstance)convertingContext[source.Parent];

            dest.Expression = ConvertLogicalQueryNode();

        //public BaseIndexedLogicalQueryNode Expression { get; set; }

            //public bool IsActive { get; set; }
            //public bool HasVars { get; set; }
            //public bool HasQuestionVars { get; set; }

            throw new NotImplementedException();
        }
    }
}
