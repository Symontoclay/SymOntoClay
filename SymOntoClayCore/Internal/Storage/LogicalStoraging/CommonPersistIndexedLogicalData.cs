/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.EqualityComparers;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.LogicalStoraging
{
    public class CommonPersistIndexedLogicalData: BaseLoggedComponent
    {
        public CommonPersistIndexedLogicalData()
            : this(LoggerNLogImpementation.Instance)
        {
        }

        public CommonPersistIndexedLogicalData(IEntityLogger logger)
            : base(logger)
        {
            IndexedRuleInstancesDict = new Dictionary<StrongIdentifierValue, RuleInstance>();
            AdditionalRuleInstancesDict = new Dictionary<StrongIdentifierValue, RuleInstance>();
            IndexedRulePartsOfFactsDict = new Dictionary<StrongIdentifierValue, IList<BaseRulePart>>();
            IndexedRulePartsWithOneRelationWithVarsDict = new Dictionary<StrongIdentifierValue, IList<BaseRulePart>>();
            RelationsList = new List<LogicalQueryNode>();
        }

        public IDictionary<StrongIdentifierValue, RuleInstance> IndexedRuleInstancesDict { get; set; }
        public IDictionary<StrongIdentifierValue, RuleInstance> AdditionalRuleInstancesDict { get; set; }

        public IDictionary<StrongIdentifierValue, IList<BaseRulePart>> IndexedRulePartsOfFactsDict { get; set; }
        public IDictionary<StrongIdentifierValue, IList<BaseRulePart>> IndexedRulePartsWithOneRelationWithVarsDict { get; set; }
        public IList<LogicalQueryNode> RelationsList { get; set; }
        private IDictionary<KindOfLogicalQueryNode, IList<LogicalQueryNode>> _leafsDict = new Dictionary<KindOfLogicalQueryNode, IList<LogicalQueryNode>>();

        private static LogicalQueryNodeEqualityComparer _logicalQueryNodeEqualityComparer = new LogicalQueryNodeEqualityComparer();

        public void NSetIndexedRuleInstanceToIndexData(RuleInstance indexedRuleInstance, bool registerLeafs)
        {
            IndexedRuleInstancesDict[indexedRuleInstance.Name] = indexedRuleInstance;

            var kind = indexedRuleInstance.KindOfRuleInstance;

            switch (kind)
            {
                case KindOfRuleInstance.Fact:
                    NAddIndexedRulePartToKeysOfRelationsIndex(IndexedRulePartsOfFactsDict, indexedRuleInstance.PrimaryPart);
                    break;

                case KindOfRuleInstance.Rule:
                    {
                        var part_1 = indexedRuleInstance.PrimaryPart;
                        
                        if (part_1.HasVars && part_1.IsActive && !part_1.HasQuestionVars && part_1.RelationsDict.Count == 1)
                        {
                            NAddIndexedRulePartToKeysOfRelationsIndex(IndexedRulePartsWithOneRelationWithVarsDict, part_1);
                        }

                        foreach(var part_2 in indexedRuleInstance.SecondaryParts)
                        {
                            if (part_2.HasVars && part_2.IsActive && !part_2.HasQuestionVars && part_2.RelationsDict.Count == 1)
                            {
                                NAddIndexedRulePartToKeysOfRelationsIndex(IndexedRulePartsWithOneRelationWithVarsDict, part_2);
                            }
                        }
                    }
                    break;


                case KindOfRuleInstance.Question:
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }

            switch (kind)
            {
                case KindOfRuleInstance.Fact:
                case KindOfRuleInstance.Rule:
                case KindOfRuleInstance.Question:
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }

            if(registerLeafs)
            {
                NRegisterLogicalQueryNodeLeafs(indexedRuleInstance);
            }
        }

        private void NRegisterLogicalQueryNodeLeafs(RuleInstance indexedRuleInstance)
        {
            var leafsDict = indexedRuleInstance.LeafsList.GroupBy(p => p.Kind).ToDictionary(p => p.Key, p => p.ToList());

            foreach(var leafsDictKVPItem in leafsDict)
            {
                NRegisterLogicalQueryNodeLeafs(leafsDictKVPItem.Key, leafsDictKVPItem.Value);
            }
        }

        private void NRegisterLogicalQueryNodeLeafs(KindOfLogicalQueryNode kind, List<LogicalQueryNode> leafs)
        {
            if(_leafsDict.ContainsKey(kind))
            {
                var list = _leafsDict[kind];

                foreach(var leaf in leafs)
                {
                    if(!list.Contains(leaf))
                    {
                        list.Add(leaf);
                    }
                }
            }
            else
            {
                _leafsDict[kind] = leafs.Distinct(_logicalQueryNodeEqualityComparer).ToList();
            }
        }

        private void NAddIndexedRulePartToKeysOfRelationsIndex(IDictionary<StrongIdentifierValue, IList<BaseRulePart>> indexData, BaseRulePart indexedRulePart)
        {
            var relationsList = indexedRulePart.RelationsDict.SelectMany(p => p.Value).Distinct().ToList();

            foreach (var relation in relationsList)
            {
                if (!RelationsList.Contains(relation))
                {
                    RelationsList.Add(relation);
                }
            }

            var keysOfRelationsList = indexedRulePart.RelationsDict.Keys.ToList();

            foreach (var keyOfRelation in keysOfRelationsList)
            {
                if (indexData.ContainsKey(keyOfRelation))
                {
                    var tmpList = indexData[keyOfRelation];
                    if (!tmpList.Contains(indexedRulePart))
                    {
                        tmpList.Add(indexedRulePart);
                    }
                }
                else
                {
                    var tmpList = new List<BaseRulePart>() { indexedRulePart };
                    indexData[keyOfRelation] = tmpList;
                }
            }
        }

        public void NRemoveIndexedRuleInstanceFromIndexData(RuleInstance indexedRuleInstance)
        {
            IndexedRuleInstancesDict.Remove(indexedRuleInstance.Name);

            var kind = indexedRuleInstance.KindOfRuleInstance;

            switch (kind)
            {
                case KindOfRuleInstance.Fact:
                    NRemoveIndexedRulePartFromKeysOfRelationsIndex(IndexedRulePartsOfFactsDict, indexedRuleInstance.PrimaryPart);
                    break;

                case KindOfRuleInstance.Rule:
                    {
                        var part_1 = indexedRuleInstance.PrimaryPart;

                        if (part_1.HasVars && part_1.IsActive && !part_1.HasQuestionVars && part_1.RelationsDict.Count == 1)
                        {
                            NRemoveIndexedRulePartFromKeysOfRelationsIndex(IndexedRulePartsWithOneRelationWithVarsDict, part_1);
                        }

                        foreach(var part_2 in indexedRuleInstance.SecondaryParts)
                        {
                            if (part_2.HasVars && part_2.IsActive && !part_2.HasQuestionVars && part_2.RelationsDict.Count == 1)
                            {
                                NRemoveIndexedRulePartFromKeysOfRelationsIndex(IndexedRulePartsWithOneRelationWithVarsDict, part_2);
                            }
                        }
                    }
                    break;


                case KindOfRuleInstance.Question:
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }

            switch (kind)
            {
                case KindOfRuleInstance.Fact:
                case KindOfRuleInstance.Rule:
                case KindOfRuleInstance.Question:
                    break;


                default: throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }

        }

        private void NRemoveIndexedRulePartFromKeysOfRelationsIndex(IDictionary<StrongIdentifierValue, IList<BaseRulePart>> indexData, BaseRulePart indexedRulePart)
        {
            var relationsList = indexedRulePart.RelationsDict.SelectMany(p => p.Value).Distinct().ToList();

            foreach (var relation in relationsList)
            {
                if (RelationsList.Contains(relation))
                {
                    RelationsList.Remove(relation);
                }
            }

            var keysOfRelationsList = indexedRulePart.RelationsDict.Keys.ToList();

            foreach (var keyOfRelation in keysOfRelationsList)
            {
                if (indexData.ContainsKey(keyOfRelation))
                {
                    var tmpList = indexData[keyOfRelation];
                    if (tmpList.Contains(indexedRulePart))
                    {
                        tmpList.Remove(indexedRulePart);
                    }
                }
            }
        }

        public IList<LogicalQueryNode> GetAllRelations()
        {
            return RelationsList.ToList();
        }

        public IList<BaseRulePart> GetIndexedRulePartOfFactsByKeyOfRelation(StrongIdentifierValue name)
        {
            if (IndexedRulePartsOfFactsDict.ContainsKey(name))
            {
                return IndexedRulePartsOfFactsDict[name];
            }

            return null;
        }

        public IList<BaseRulePart> GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(StrongIdentifierValue name)
        {
            if (IndexedRulePartsWithOneRelationWithVarsDict.ContainsKey(name))
            {
                return IndexedRulePartsWithOneRelationWithVarsDict[name];
            }

            return null;
        }

        public IReadOnlyList<LogicalQueryNode> GetLogicalQueryNodes(IList<LogicalQueryNode> exceptList, ReplacingNotResultsStrategy replacingNotResultsStrategy, IList<KindOfLogicalQueryNode> targetKindsOfItems)
        {
            switch(replacingNotResultsStrategy)
            {
                case ReplacingNotResultsStrategy.AllKindOfItems:
                    return _leafsDict.Values.SelectMany(p => p).Where(p => !exceptList.Contains(p))/*.Distinct(_logicalQueryNodeEqualityComparer)*/.ToList();

                case ReplacingNotResultsStrategy.PresentKindOfItems:
                    {
                        var result = new List<LogicalQueryNode>();

                        foreach(var targetKind in targetKindsOfItems)
                        {
                            if(_leafsDict.ContainsKey(targetKind))
                            {
                                result.AddRange(_leafsDict[targetKind].Where(p => !exceptList.Contains(p)));
                            }
                        }

                        return result;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(replacingNotResultsStrategy), replacingNotResultsStrategy, null);
            }

        }
    }
}
