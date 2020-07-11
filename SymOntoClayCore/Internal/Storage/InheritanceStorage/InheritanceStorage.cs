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
            : base(realStorageContext.Logger)
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

        private readonly Dictionary<Name, Dictionary<Name, List<InheritanceItem>>> _nonIndexedInfo = new Dictionary<Name, Dictionary<Name, List<InheritanceItem>>>();
        private readonly Dictionary<Name, Dictionary<Name, List<IndexedInheritanceItem>>> _indexedInfo = new Dictionary<Name, Dictionary<Name, List<IndexedInheritanceItem>>>();

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
                Log($"subItem = {subItem}");
                Log($"inheritanceItem = {inheritanceItem}");
                Log($"isPrimary = {isPrimary}");
#endif
                var indexedInheritanceItem = inheritanceItem.GetIndexed(_realStorageContext.EntityDictionary);

#if DEBUG
                Log($"indexedInheritanceItem = {indexedInheritanceItem}");
#endif

                var subName = indexedInheritanceItem.SubName;
                var superName = indexedInheritanceItem.SuperName;

                if (subName.IsEmpty || superName.IsEmpty)
                {
                    return;
                }

                if (_nonIndexedInfo.ContainsKey(subName))
                {
                    var dict = _nonIndexedInfo[subName];
                    var indexedDict = _indexedInfo[subName];

                    if (dict.ContainsKey(superName))
                    {
                        dict[superName].Add(inheritanceItem);
                        indexedDict[superName].Add(indexedInheritanceItem);
                    }
                    else
                    {
                        dict[superName] = new List<InheritanceItem>() { inheritanceItem };
                        indexedDict[superName] = new List<IndexedInheritanceItem>() { indexedInheritanceItem };
                    }
                }
                else
                {
                    var dict = new Dictionary<Name, List<InheritanceItem>>();
                    _nonIndexedInfo[subName] = dict;
                    dict[superName] = new List<InheritanceItem>() { inheritanceItem };

                    var indexedDict = new Dictionary<Name, List<IndexedInheritanceItem>>();
                    _indexedInfo[subName] = indexedDict;
                    indexedDict[superName] = new List<IndexedInheritanceItem>() { indexedInheritanceItem };
                }
            }

            if(isPrimary)
            {
                var inheritanceFact = CreateInheritanceFact(subItem, inheritanceItem);

#if DEBUG
                Log($"inheritanceFact = {inheritanceFact}");
                Log($"inheritanceFact = {DebugHelperForRuleInstance.ToString(inheritanceFact)}");
#endif

                _realStorageContext.LogicalStorage.Append(inheritanceFact, false);
            }

#if DEBUG
            Log("End");
#endif
        }

        private RuleInstance CreateInheritanceFact(Name subItem, InheritanceItem inheritanceItem)
        {
            var factName = NameHelper.CreateRuleOrFactName(_realStorageContext.EntityDictionary);

#if DEBUG
            Log($"factName = {factName}");
#endif

            var fact = new RuleInstance();
            fact.AppendAnnotations(inheritanceItem);
            fact.DictionaryName = _realStorageContext.EntityDictionary.Name;
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
        public IList<WeightedInheritanceResultItem<IndexedInheritanceItem>> GetItemsDirectly(Name subName)
        {
            lock (_lockObj)
            {
#if DEBUG
                Log($"superName = {subName}");
#endif

                if(subName.IsEmpty)
                {
                    return new List<WeightedInheritanceResultItem<IndexedInheritanceItem>>();
                }

#if DEBUG
                Log("Next");
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
