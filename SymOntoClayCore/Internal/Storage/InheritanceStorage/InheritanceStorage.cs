using Newtonsoft.Json;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.InheritanceStorage
{
    public class InheritanceStorage: BaseLoggedComponent, IInheritanceStorage
    {
        public InheritanceStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.MainStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
        }

        private readonly object _lockObj = new object();

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;

        private readonly Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<InheritanceItem>>> _nonIndexedInfo = new Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<InheritanceItem>>>();
        private readonly Dictionary<IndexedStrongIdentifierValue, Dictionary<IndexedStrongIdentifierValue, List<IndexedInheritanceItem>>> _indexedInfo = new Dictionary<IndexedStrongIdentifierValue, Dictionary<IndexedStrongIdentifierValue, List<IndexedInheritanceItem>>>();

        /// <inheritdoc/>
        public void SetInheritance(InheritanceItem inheritanceItem)
        {
            SetInheritance(inheritanceItem, true);
        }

        /// <inheritdoc/>
        public void SetInheritance(InheritanceItem inheritanceItem, bool isPrimary)
        {
            var subItem = inheritanceItem.SubName;

            lock (_lockObj)
            {
#if DEBUG
                //Log($"subItem = {subItem}");
                //Log($"inheritanceItem = {inheritanceItem}");
                //Log($"isPrimary = {isPrimary}");
#endif
                var indexedInheritanceItem = inheritanceItem.GetIndexed(_realStorageContext.MainStorageContext);

#if DEBUG
                //Log($"indexedInheritanceItem = {indexedInheritanceItem}");
#endif

                var subName = inheritanceItem.SubName;
                var superName = inheritanceItem.SuperName;

                var indexedSubName = indexedInheritanceItem.SubName;
                var indexedSuperName = indexedInheritanceItem.SuperName;

                if (subName.IsEmpty || superName.IsEmpty)
                {
                    return;
                }

                if (_nonIndexedInfo.ContainsKey(subName))
                {
                    var dict = _nonIndexedInfo[subName];
                    var indexedDict = _indexedInfo[indexedSubName];

                    if (dict.ContainsKey(superName))
                    {
                        var targetList = dict[superName];

#if DEBUG
                        //Log($"dict[superName].Count = {dict[superName].Count}");
                        //Log($"targetList = {targetList.WriteListToString()}");
#endif
                        var targetLongConditionalHashCode = indexedInheritanceItem.GetLongConditionalHashCode();

#if DEBUG
                        //Log($"targetLongConditionalHashCode = {targetLongConditionalHashCode}");
#endif

                        var targetIndexedList = indexedDict[indexedSuperName];

                        var indexedItemsWithTheSameLongConditionalHashCodeList = targetIndexedList.Where(p => p.GetLongConditionalHashCode() == targetLongConditionalHashCode).ToList();

                        foreach (var indexedItemWithTheSameLongConditionalHashCode in indexedItemsWithTheSameLongConditionalHashCodeList)
                        {
                            targetIndexedList.Remove(indexedItemWithTheSameLongConditionalHashCode);
                        }

                        var itemsWithTheSameLongConditionalHashCodeList = indexedItemsWithTheSameLongConditionalHashCodeList.Select(p => p.OriginalInheritanceItem).ToList();

#if DEBUG
                        //Log($"itemsWithTheSameLongConditionalHashCodeList = {itemsWithTheSameLongConditionalHashCodeList.WriteListToString()}");
#endif

                        foreach(var itemWithTheSameLongConditionalHashCode in itemsWithTheSameLongConditionalHashCodeList)
                        {
                            targetList.Remove(itemWithTheSameLongConditionalHashCode);
                        }

                        targetList.Add(inheritanceItem);

                        targetIndexedList.Add(indexedInheritanceItem);
                    }
                    else
                    {
                        dict[superName] = new List<InheritanceItem>() { inheritanceItem };
                        indexedDict[indexedSuperName] = new List<IndexedInheritanceItem>() { indexedInheritanceItem };
                    }
                }
                else
                {
                    var dict = new Dictionary<StrongIdentifierValue, List<InheritanceItem>>();
                    _nonIndexedInfo[subName] = dict;
                    dict[superName] = new List<InheritanceItem>() { inheritanceItem };

                    var indexedDict = new Dictionary<IndexedStrongIdentifierValue, List<IndexedInheritanceItem>>();
                    _indexedInfo[indexedSubName] = indexedDict;
                    indexedDict[indexedSuperName] = new List<IndexedInheritanceItem>() { indexedInheritanceItem };
                }
            }

            if(isPrimary)
            {
                var inheritanceFact = CreateInheritanceFact(subItem, inheritanceItem);

#if DEBUG
                //Log($"inheritanceFact = {inheritanceFact}");
                //Log($"inheritanceFact = {DebugHelperForRuleInstance.ToString(inheritanceFact)}");
#endif

                _realStorageContext.LogicalStorage.Append(inheritanceFact, false);
            }

#if DEBUG
            //Log("End");
#endif
        }

        private RuleInstance CreateInheritanceFact(StrongIdentifierValue subItem, InheritanceItem inheritanceItem)
        {
            var dictionary = _realStorageContext.MainStorageContext.Dictionary;

            var factName = NameHelper.CreateRuleOrFactName(dictionary);

#if DEBUG
            //Log($"factName = {factName}");
#endif

            var fact = new RuleInstance();
            fact.Kind = KindOfRuleInstance.Fact;
            fact.AppendAnnotations(inheritanceItem);
            fact.DictionaryName = dictionary.Name;
            fact.Name = factName;

            var primaryPart = new PrimaryRulePart();
            fact.PrimaryPart = primaryPart;
            primaryPart.Parent = fact;

            primaryPart.QuantityQualityModalities.Add(inheritanceItem.Rank);

            var isExpr = new LogicalQueryNode
            {
                Kind = KindOfLogicalQueryNode.BinaryOperator,
                KindOfOperator = KindOfOperatorOfLogicalQueryNode.Is
            };

            primaryPart.Expression = isExpr;

            var subItemNode = new LogicalQueryNode();
            isExpr.Left = subItemNode;
            subItemNode.Kind = KindOfLogicalQueryNode.Concept;
            subItemNode.Name = subItem;

            var superItemNode = new LogicalQueryNode();
            isExpr.Right = superItemNode;
            superItemNode.Kind = KindOfLogicalQueryNode.Concept;
            superItemNode.Name = inheritanceItem.SuperName;

            return fact;
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<IndexedInheritanceItem>> GetItemsDirectly(IndexedStrongIdentifierValue subName)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"superName = {subName}");
#endif

                if(subName.IsEmpty)
                {
                    return new List<WeightedInheritanceResultItem<IndexedInheritanceItem>>();
                }

#if DEBUG
                //Log("Next");
#endif

                if(_indexedInfo.ContainsKey(subName))
                {
                    var rawResult = _indexedInfo[subName].SelectMany(p => p.Value).ToList();

                    var result = new List<WeightedInheritanceResultItem<IndexedInheritanceItem>>();

                    foreach(var rawResultItem in rawResult)
                    {
                        result.Add(new WeightedInheritanceResultItem<IndexedInheritanceItem>(rawResultItem));
                    }

                    return result;
                }

                return new List<WeightedInheritanceResultItem<IndexedInheritanceItem>>();
            }
        }
    }
}
