/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
            _inheritancePublicFactsReplicator = realStorageContext.InheritancePublicFactsReplicator;
        }

        private readonly object _lockObj = new object();
        private readonly object _factsIdRegistryLockObj = new object();

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;
        private IInheritancePublicFactsReplicator _inheritancePublicFactsReplicator;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;

        private readonly Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<InheritanceItem>>> _nonIndexedInfo = new Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<InheritanceItem>>>();
        //private readonly Dictionary<ulong, Dictionary<ulong, List<IndexedInheritanceItem>>> _indexedInfo = new Dictionary<ulong, Dictionary<ulong, List<IndexedInheritanceItem>>>();
        private readonly Dictionary<InheritanceItem, string> _factsIdRegistry = new Dictionary<InheritanceItem, string>();

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
                //Log($"kind = {_kind}");
                //Log($"subItem = {subItem}; superName = {inheritanceItem.SuperName}");
                //Log($"inheritanceItem = {inheritanceItem}");
                //Log($"isPrimary = {isPrimary}");
#endif
                //var indexedInheritanceItem = inheritanceItem.GetIndexed(_realStorageContext.MainStorageContext);

#if DEBUG
                //Log($"indexedInheritanceItem = {indexedInheritanceItem}");
#endif

                var subName = inheritanceItem.SubName;
                var superName = inheritanceItem.SuperName;

                if (subName.IsEmpty || superName.IsEmpty)
                {
                    return;
                }

                //var subNameNormalizedNameValue = subName.NormalizedNameValue;
                //var superNameNormalizedNameValue = superName.NormalizedNameValue;

#if DEBUG
                //Log($"subNameNormalizedNameValue = {subNameNormalizedNameValue}");
                //Log($"superNameNormalizedNameValue = {superNameNormalizedNameValue}");
#endif

                if (_nonIndexedInfo.ContainsKey(subName))
                {
                    var dict = _nonIndexedInfo[subName];
                    //var indexedDict = _indexedInfo[subNameKey];

                    if (dict.ContainsKey(superName))
                    {
                        var targetList = dict[superName];

#if DEBUG
                        //Log($"dict[superName].Count = {dict[superName].Count}");
                        //Log($"targetList = {targetList.WriteListToString()}");
#endif
                        var targetLongConditionalHashCode = inheritanceItem.GetLongConditionalHashCode();

#if DEBUG
                        //Log($"targetLongConditionalHashCode = {targetLongConditionalHashCode}");
#endif

                        //var targetIndexedList = indexedDict[superNameKey];

                        var itemsWithTheSameLongConditionalHashCodeList = targetList.Where(p => p.GetLongConditionalHashCode() == targetLongConditionalHashCode).ToList();

                        foreach (var itemWithTheSameLongConditionalHashCode in itemsWithTheSameLongConditionalHashCodeList)
                        {
                            targetList.Remove(itemWithTheSameLongConditionalHashCode);
                            NRemoveIngeritanceFact(itemWithTheSameLongConditionalHashCode);
                        }

                        //var itemsWithTheSameLongConditionalHashCodeList = indexedItemsWithTheSameLongConditionalHashCodeList.Select(p => p.OriginalInheritanceItem).ToList();

#if DEBUG
                        //Log($"itemsWithTheSameLongConditionalHashCodeList = {itemsWithTheSameLongConditionalHashCodeList.WriteListToString()}");
#endif

                        //foreach(var itemWithTheSameLongConditionalHashCode in itemsWithTheSameLongConditionalHashCodeList)
                        //{
                        //    targetList.Remove(itemWithTheSameLongConditionalHashCode);
                        //    NRemoveIngeritanceFact(itemWithTheSameLongConditionalHashCode);
                        //}

                        targetList.Add(inheritanceItem);

                        //targetIndexedList.Add(indexedInheritanceItem);
                    }
                    else
                    {
                        dict[superName] = new List<InheritanceItem>() { inheritanceItem };
                        //indexedDict[superNameKey] = new List<IndexedInheritanceItem>() { indexedInheritanceItem };
                    }
                }
                else
                {
                    var dict = new Dictionary<StrongIdentifierValue, List<InheritanceItem>>();
                    _nonIndexedInfo[subName] = dict;
                    dict[superName] = new List<InheritanceItem>() { inheritanceItem };

                    //var indexedDict = new Dictionary<ulong, List<IndexedInheritanceItem>>();
                    //_indexedInfo[subNameKey] = indexedDict;
                    //indexedDict[superNameKey] = new List<IndexedInheritanceItem>() { indexedInheritanceItem };
                }
            }

            if(isPrimary && _kind != KindOfStorage.PublicFacts && _kind != KindOfStorage.PerceptedFacts)
            {
                var inheritanceFact = CreateInheritanceFact(inheritanceItem);

#if DEBUG
                //Log($"inheritanceFact = {inheritanceFact}");
                //Log($"inheritanceFact = {DebugHelperForRuleInstance.ToString(inheritanceFact)}");
#endif
                lock(_factsIdRegistryLockObj)
                {
                    _factsIdRegistry[inheritanceItem] = inheritanceFact.Name.NameValue;
                }

                _realStorageContext.LogicalStorage.Append(inheritanceFact, false);

                if(_kind == KindOfStorage.Global || _kind ==  KindOfStorage.Host)
                {
                    _inheritancePublicFactsReplicator.ProcessChangeInheritance(inheritanceItem.SubName, inheritanceItem.SuperName);
                }
            }

#if DEBUG
            //Log("End");
#endif
        }

        /// <inheritdoc/>
        public void RemoveInheritance(InheritanceItem inheritanceItem)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"kind = {_kind}");
                //Log($"subItem = {subItem}; superName = {inheritanceItem.SuperName}");
                //Log($"inheritanceItem = {inheritanceItem}");
                //Log($"isPrimary = {isPrimary}");
#endif
                //var indexedInheritanceItem = inheritanceItem.GetIndexed(_realStorageContext.MainStorageContext);

#if DEBUG
                //Log($"indexedInheritanceItem = {indexedInheritanceItem}");
#endif

                var subName = inheritanceItem.SubName;
                var superName = inheritanceItem.SuperName;

                if (subName.IsEmpty || superName.IsEmpty)
                {
                    return;
                }

                //var subNameNormalizedNameValue = subName.NormalizedNameValue;
                //var superNameNormalizedNameValue = superName.NormalizedNameValue;

#if DEBUG
                //Log($"subNameNormalizedNameValue = {subNameNormalizedNameValue}");
                //Log($"superNameNormalizedNameValue = {superNameNormalizedNameValue}");
#endif

                if (_nonIndexedInfo.ContainsKey(subName))
                {
                    var dict = _nonIndexedInfo[subName];
                    

                    if(dict.ContainsKey(superName))
                    {
                        //var indexedDict = _indexedInfo[subNameKey];

                        var targetList = dict[superName];

                        targetList.Remove(inheritanceItem);

                        //var targetIndexedList = indexedDict[superNameKey];

                        //targetIndexedList.Remove(indexedInheritanceItem);

                        NRemoveIngeritanceFact(inheritanceItem);
                    }
                }
            }
        }

        private void NRemoveIngeritanceFact(InheritanceItem inheritanceItem)
        {
            lock (_factsIdRegistryLockObj)
            {
                if (_factsIdRegistry.ContainsKey(inheritanceItem))
                {
                    var factId = _factsIdRegistry[inheritanceItem];

                    _realStorageContext.LogicalStorage.RemoveById(factId);

                    _factsIdRegistry.Remove(inheritanceItem);
                }
            }
        }

        private RuleInstance CreateInheritanceFact(InheritanceItem inheritanceItem)
        {
            var factName = NameHelper.CreateRuleOrFactName();

#if DEBUG
            //Log($"factName = {factName}");
#endif

            var fact = new RuleInstance();
            fact.Kind = KindOfRuleInstance.Fact;
            fact.AppendAnnotations(inheritanceItem);
            fact.Name = factName;
            //fact.KeysOfPrimaryRecords.Add(inheritanceItem.Id);

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
            subItemNode.Name = inheritanceItem.SubName;

            var superItemNode = new LogicalQueryNode();
            isExpr.Right = superItemNode;
            superItemNode.Kind = KindOfLogicalQueryNode.Concept;
            superItemNode.Name = inheritanceItem.SuperName;

            return fact;
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<InheritanceItem>> GetItemsDirectly(StrongIdentifierValue subName)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"superName = {subNameKey}");
#endif

                if(subName.IsEmpty)
                {
                    return new List<WeightedInheritanceResultItem<InheritanceItem>>();
                }

#if DEBUG
                //Log("Next");
#endif

                if(_nonIndexedInfo.ContainsKey(subName))
                {
                    var rawResult = _nonIndexedInfo[subName].SelectMany(p => p.Value).ToList();

                    var result = new List<WeightedInheritanceResultItem<InheritanceItem>>();

                    foreach(var rawResultItem in rawResult)
                    {
                        result.Add(new WeightedInheritanceResultItem<InheritanceItem>(rawResultItem));
                    }

                    return result;
                }

                return new List<WeightedInheritanceResultItem<InheritanceItem>>();
            }
        }
    }
}
