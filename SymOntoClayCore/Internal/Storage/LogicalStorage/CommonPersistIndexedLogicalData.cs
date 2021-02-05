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

using NLog;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.LogicalStorage
{
    public class CommonPersistIndexedLogicalData: BaseLoggedComponent
    {
        public CommonPersistIndexedLogicalData(IEntityLogger logger, IEntityDictionary entityDictionary)
            : base(logger)
        {
            _entityDictionary = entityDictionary;

            IndexedRuleInstancesDict = new Dictionary<ulong, IndexedRuleInstance>();
            AdditionalRuleInstancesDict = new Dictionary<ulong, IndexedRuleInstance>();
            IndexedRulePartsOfFactsDict = new Dictionary<ulong, IList<IndexedBaseRulePart>>();
            IndexedRulePartsWithOneRelationWithVarsDict = new Dictionary<ulong, IList<IndexedBaseRulePart>>();
            RelationsList = new List<RelationIndexedLogicalQueryNode>();
        }

        private IEntityDictionary _entityDictionary;

        //It is temporary public for construction time. It will be private after complete construction.
        public IDictionary<ulong, IndexedRuleInstance> IndexedRuleInstancesDict { get; set; }
        public IDictionary<ulong, IndexedRuleInstance> AdditionalRuleInstancesDict { get; set; }

        //It is temporary public for construction time. It will be private after complete construction.
        public IDictionary<ulong, IList<IndexedBaseRulePart>> IndexedRulePartsOfFactsDict { get; set; }
        //It is temporary public for construction time. It will be private after complete construction.
        public IDictionary<ulong, IList<IndexedBaseRulePart>> IndexedRulePartsWithOneRelationWithVarsDict { get; set; }
        public IList<RelationIndexedLogicalQueryNode> RelationsList { get; set; }

        public void NSetIndexedRuleInstanceToIndexData(IndexedRuleInstance indexedRuleInstance)
        {
#if DEBUG
            //Log($"indexedRuleInstance = {indexedRuleInstance}");
#endif

            IndexedRuleInstancesDict[indexedRuleInstance.Key] = indexedRuleInstance;

            var kind = indexedRuleInstance.Kind;

            switch (kind)
            {
                case KindOfRuleInstance.Fact:
                //case KindOfRuleInstance.Annotation:
                //case KindOfRuleInstance.EntityCondition:
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

                //case KindOfRuleInstance.EntityCondition:
                //break;

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

                //case KindOfRuleInstance.Annotation:
                //case KindOfRuleInstance.EntityCondition:
                //AdditionalRuleInstancesDict[indexedRuleInstance.Key] = indexedRuleInstance;
                //break;

                default: throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }

            //Log("End");
        }

        private void NAddIndexedRulePartToKeysOfRelationsIndex(IDictionary<ulong, IList<IndexedBaseRulePart>> indexData, IndexedBaseRulePart indexedRulePart)
        {
#if DEBUG
            //var dbgStr = DebugHelperForRuleInstance.BaseRulePartToString(indexedRulePart.OriginRulePart);
            //Log($"dbgStr = {dbgStr}");
#endif

            var relationsList = indexedRulePart.RelationsDict.SelectMany(p => p.Value).Distinct().ToList();

#if DEBUG
            //Log($"relationsList = {relationsList.WriteListToString()}");
#endif

            foreach (var relation in relationsList)
            {
                if (!RelationsList.Contains(relation))
                {
                    RelationsList.Add(relation);
                }
            }

            var keysOfRelationsList = indexedRulePart.RelationsDict.Keys.ToList();

#if DEBUG
            //Log($" = {relationsList.WriteListToString()}");
#endif

            foreach (var keyOfRelation in keysOfRelationsList)
            {
#if DEBUG
                //Log($"keyOfRelation = {keyOfRelation}");
                //Log($"_entityDictionary.GetName(keyOfRelation) = {_entityDictionary.GetName(keyOfRelation)}");
#endif

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
                    var tmpList = new List<IndexedBaseRulePart>() { indexedRulePart };
                    indexData[keyOfRelation] = tmpList;
                }
            }
        }

        public void NRemoveIndexedRuleInstanceFromIndexData(IndexedRuleInstance indexedRuleInstance)
        {
#if DEBUG
            //Log($"indexedRuleInstance = {indexedRuleInstance}");
#endif

            IndexedRuleInstancesDict.Remove(indexedRuleInstance.Key);

            var kind = indexedRuleInstance.Kind;

            switch (kind)
            {
                case KindOfRuleInstance.Fact:
                    //case KindOfRuleInstance.Annotation:
                    //case KindOfRuleInstance.EntityCondition:
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

                //case KindOfRuleInstance.EntityCondition:
                //break;

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

                //case KindOfRuleInstance.Annotation:
                //case KindOfRuleInstance.EntityCondition:
                //AdditionalRuleInstancesDict[indexedRuleInstance.Key] = indexedRuleInstance;
                //break;

                default: throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }

            //Log("End");
        }

        private void NRemoveIndexedRulePartFromKeysOfRelationsIndex(IDictionary<ulong, IList<IndexedBaseRulePart>> indexData, IndexedBaseRulePart indexedRulePart)
        {
#if DEBUG
            //var dbgStr = DebugHelperForRuleInstance.BaseRulePartToString(indexedRulePart.OriginRulePart);
            //Log($"dbgStr = {dbgStr}");
#endif

            var relationsList = indexedRulePart.RelationsDict.SelectMany(p => p.Value).Distinct().ToList();

#if DEBUG
            //Log($"relationsList = {relationsList.WriteListToString()}");
#endif

            foreach (var relation in relationsList)
            {
                if (RelationsList.Contains(relation))
                {
                    RelationsList.Remove(relation);
                }
            }

            var keysOfRelationsList = indexedRulePart.RelationsDict.Keys.ToList();

#if DEBUG
            //Log($" = {relationsList.WriteListToString()}");
#endif

            foreach (var keyOfRelation in keysOfRelationsList)
            {
#if DEBUG
                //Log($"keyOfRelation = {keyOfRelation}");
                //Log($"_entityDictionary.GetName(keyOfRelation) = {_entityDictionary.GetName(keyOfRelation)}");
#endif

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

        public IList<RelationIndexedLogicalQueryNode> GetAllRelations()
        {
            return RelationsList.ToList();
        }

        public IList<IndexedBaseRulePart> GetIndexedRulePartOfFactsByKeyOfRelation(ulong key)
        {
#if DEBUG
            //Log($"key = {key}");
            //Log($"_entityDictionary.GetName(key) = {_entityDictionary.GetName(key)}");
            //Log($"_entityDictionary = {_entityDictionary.GetDbgStr()}");
            //Log($"IndexedRulePartsOfFactsDict.Count = {IndexedRulePartsOfFactsDict.Count}");
            //foreach (var tmpItem in IndexedRulePartsOfFactsDict)
            //{
            //    Log($"tmpItem.Key = {tmpItem.Key}");
            //    foreach(var a in tmpItem.Value)
            //    {
            //        var dbgStr = DebugHelperForRuleInstance.BaseRulePartToString(a.OriginRulePart);
            //        Log($"dbgStr = {dbgStr}");
            //    }                
            //}
#endif

            if (IndexedRulePartsOfFactsDict.ContainsKey(key))
            {
                return IndexedRulePartsOfFactsDict[key];
            }

            return null;
        }

        public IList<IndexedBaseRulePart> GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(ulong key)
        {
            if (IndexedRulePartsWithOneRelationWithVarsDict.ContainsKey(key))
            {
                return IndexedRulePartsWithOneRelationWithVarsDict[key];
            }

            return null;
        }
    }
}
