/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.Storage.InheritanceStoraging
{
    public class InheritanceStorage: BaseSpecificStorage, IInheritanceStorage
    {
        public InheritanceStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
            _inheritancePublicFactsReplicator = realStorageContext.InheritancePublicFactsReplicator;
        }

        private readonly object _lockObj = new object();
        private readonly object _factsIdRegistryLockObj = new object();

        private IInheritancePublicFactsReplicator _inheritancePublicFactsReplicator;

        private readonly Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<InheritanceItem>>> _nonIndexedInfo = new Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<InheritanceItem>>>();
        private readonly Dictionary<InheritanceItem, string> _factsIdRegistry = new Dictionary<InheritanceItem, string>();

        /// <inheritdoc/>
        public void SetInheritance(IMonitorLogger logger, InheritanceItem inheritanceItem)
        {
            SetInheritance(logger, inheritanceItem, true);
        }

        /// <inheritdoc/>
        public void SetInheritance(IMonitorLogger logger, InheritanceItem inheritanceItem, bool isPrimary)
        {
            var subItem = inheritanceItem.SubName;

            lock (_lockObj)
            {
                var subName = inheritanceItem.SubName;
                var superName = inheritanceItem.SuperName;

                if (subName.IsEmpty || superName.IsEmpty)
                {
                    return;
                }

                if (_nonIndexedInfo.ContainsKey(subName))
                {
                    var dict = _nonIndexedInfo[subName];

                    if (dict.ContainsKey(superName))
                    {
                        var targetList = dict[superName];

                        StorageHelper.RemoveSameItems(logger, targetList, inheritanceItem);

                        targetList.Add(inheritanceItem);
                    }
                    else
                    {
                        dict[superName] = new List<InheritanceItem>() { inheritanceItem };
                    }
                }
                else
                {
                    var dict = new Dictionary<StrongIdentifierValue, List<InheritanceItem>>();
                    _nonIndexedInfo[subName] = dict;
                    dict[superName] = new List<InheritanceItem>() { inheritanceItem };
                }
            }

            if(isPrimary && _kind != KindOfStorage.PublicFacts && _kind != KindOfStorage.PerceptedFacts)
            {
                var inheritanceFact = CreateInheritanceFact(logger, inheritanceItem);

                lock(_factsIdRegistryLockObj)
                {
                    _factsIdRegistry[inheritanceItem] = inheritanceFact.Name.NameValue;
                }

                _realStorageContext.LogicalStorage.Append(logger, inheritanceFact, false);

                if(_kind == KindOfStorage.Global || _kind ==  KindOfStorage.Host || _kind == KindOfStorage.Categories)
                {
                    _inheritancePublicFactsReplicator.ProcessChangeInheritance(logger, inheritanceItem.SubName, inheritanceItem.SuperName);
                }
            }

        }

        /// <inheritdoc/>
        public void RemoveInheritance(IMonitorLogger logger, InheritanceItem inheritanceItem)
        {
            lock (_lockObj)
            {
                var subName = inheritanceItem.SubName;
                var superName = inheritanceItem.SuperName;

                if (subName.IsEmpty || superName.IsEmpty)
                {
                    return;
                }

                if (_nonIndexedInfo.ContainsKey(subName))
                {
                    var dict = _nonIndexedInfo[subName];
                    

                    if(dict.ContainsKey(superName))
                    {
                        var targetList = dict[superName];

                        targetList.Remove(inheritanceItem);

                        NRemoveIngeritanceFact(logger, inheritanceItem);
                    }
                }
            }
        }

        private void NRemoveIngeritanceFact(IMonitorLogger logger, InheritanceItem inheritanceItem)
        {
            lock (_factsIdRegistryLockObj)
            {
                if (_factsIdRegistry.ContainsKey(inheritanceItem))
                {
                    var factId = _factsIdRegistry[inheritanceItem];

                    _realStorageContext.LogicalStorage.RemoveById(logger, factId);

                    _factsIdRegistry.Remove(inheritanceItem);
                }
            }
        }

        private RuleInstance CreateInheritanceFact(IMonitorLogger logger, InheritanceItem inheritanceItem)
        {
            var factName = NameHelper.CreateRuleOrFactName();

            var fact = new RuleInstance();
            fact.KindOfRuleInstance = KindOfRuleInstance.Fact;
            fact.AppendAnnotations(inheritanceItem);
            fact.Name = factName;

            var primaryPart = new PrimaryRulePart();
            fact.PrimaryPart = primaryPart;
            primaryPart.Parent = fact;

            var isRelation = new LogicalQueryNode();
            isRelation.Name = NameHelper.CreateName("is");
            isRelation.Kind = KindOfLogicalQueryNode.Relation;
            isRelation.ParamsList = new List<LogicalQueryNode>();

            primaryPart.Expression = isRelation;

            var subItemNode = new LogicalQueryNode();
            isRelation.ParamsList.Add(subItemNode);
            subItemNode.Kind = KindOfLogicalQueryNode.Concept;
            subItemNode.Name = inheritanceItem.SubName;

            var superItemNode = new LogicalQueryNode();
            isRelation.ParamsList.Add(superItemNode);
            superItemNode.Kind = KindOfLogicalQueryNode.Concept;
            superItemNode.Name = inheritanceItem.SuperName;

            var rankNode = new LogicalQueryNode();
            isRelation.ParamsList.Add(rankNode);
            rankNode.Kind = KindOfLogicalQueryNode.Value;
            rankNode.Value = inheritanceItem.Rank;

            return fact;
        }

        private static List<WeightedInheritanceResultItem<InheritanceItem>> _emptyItemsList = new List<WeightedInheritanceResultItem<InheritanceItem>>();

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<InheritanceItem>> GetItemsDirectly(IMonitorLogger logger, StrongIdentifierValue subName)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyItemsList;
                }

                if (subName == null || subName.IsEmpty)
                {
                    return _emptyItemsList;
                }

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

                return _emptyItemsList;
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _nonIndexedInfo.Clear();
            _factsIdRegistry.Clear();

            base.OnDisposed();
        }
    }
}
