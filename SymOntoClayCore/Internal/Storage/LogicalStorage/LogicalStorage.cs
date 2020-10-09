/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.LogicalStorage
{
    public class LogicalStorage: BaseLoggedComponent, ILogicalStorage
    {
        public LogicalStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.MainStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;

            _ruleInstancesList = new List<RuleInstance>();
            _ruleInstancesDict = new Dictionary<ulong, RuleInstance>();
            _ruleInstancesDictByHashCode = new Dictionary<ulong, RuleInstance>();
            _commonPersistIndexedLogicalData = new CommonPersistIndexedLogicalData(realStorageContext.MainStorageContext.Logger, realStorageContext.MainStorageContext.Dictionary);
        }

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;

        private readonly object _lockObj = new object();

        private List<RuleInstance> _ruleInstancesList;
        private Dictionary<ulong, RuleInstance> _ruleInstancesDict;
        private Dictionary<ulong, RuleInstance> _ruleInstancesDictByHashCode;
        private readonly CommonPersistIndexedLogicalData _commonPersistIndexedLogicalData;

        /// <inheritdoc/>
        public void Append(RuleInstance ruleInstance)
        {
            Append(ruleInstance, true);
        }

        /// <inheritdoc/>
        public void Append(RuleInstance ruleInstance, bool isPrimary)
        {
            lock (_lockObj)
            {
                var annotationsList = ruleInstance.GetAllAnnotations();

#if DEBUG
                //Log($"annotationsList = {annotationsList.WriteListToString()}");
#endif

                foreach(var annotationRuleInstance in annotationsList)
                {
                    NAppend(annotationRuleInstance, false);
                }

                NAppend(ruleInstance, isPrimary);
            }
        }

        private void NAppend(RuleInstance ruleInstance, bool isPrimary)
        {
#if DEBUG
            //Log($"ruleInstance = {ruleInstance}");
            //Log($"isPrimary = {isPrimary}");
            //Log($"ruleInstance = {DebugHelperForRuleInstance.ToString(ruleInstance)}");
#endif

            if (_ruleInstancesList.Contains(ruleInstance))
            {
                return;
            }

            //ruleInstance = ruleInstance.Clone();

#if DEBUG
            //Log($"ruleInstance (after) = {ruleInstance}");
#endif

            var indexedRuleInstance = ruleInstance.GetIndexed(_realStorageContext.MainStorageContext);

#if DEBUG
            //Log($"indexedRuleInstance = {indexedRuleInstance}");
#endif

            var ruleInstanceKey = indexedRuleInstance.Key;

#if DEBUG
            //Log($"ruleInstanceKey = {ruleInstanceKey}");
#endif

            if (_ruleInstancesDict.ContainsKey(ruleInstanceKey))
            {
                return;
            }

            var longHashCode = indexedRuleInstance.GetLongHashCode();

#if DEBUG
            //Log($"longHashCode = {longHashCode}");
#endif

            if(_ruleInstancesDictByHashCode.ContainsKey(longHashCode))
            {
                return;
            }

            _ruleInstancesList.Add(ruleInstance);
            _ruleInstancesDict[ruleInstanceKey] = ruleInstance;
            _ruleInstancesDictByHashCode[longHashCode] = ruleInstance;

            _commonPersistIndexedLogicalData.NSetIndexedRuleInstanceToIndexData(indexedRuleInstance);

            if(isPrimary)
            {
                var inheritanceRelationsList = ruleInstance.GetInheritanceRelations();
                var ruleInstanceName = ruleInstance.Name.NameValue;

                if (inheritanceRelationsList.Any())
                {
                    var inheritanceStorage = _realStorageContext.InheritanceStorage;

                    foreach (var inheritanceRelation in inheritanceRelationsList)
                    {
#if DEBUG
                        //Log($"inheritanceRelation = {inheritanceRelation}");
#endif
                        var inheritanceItem = new InheritanceItem();

                        inheritanceItem.SuperName = inheritanceRelation.Name;
                        inheritanceItem.SubName = inheritanceRelation.ParamsList.Single().Name;
                        inheritanceItem.Rank = new LogicalValue(1);
                        inheritanceItem.KeysOfPrimaryRecords.Add(ruleInstanceName);
#if DEBUG
                        //Log($"inheritanceItem = {inheritanceItem}");
#endif                       

                        inheritanceStorage.SetInheritance(inheritanceItem, false);
                    }
                }
            }
            
            EmitOnChanged();

#if IMAGINE_WORKING
            //Log("End");
#else
            throw new NotImplementedException();
#endif
        }

        /// <inheritdoc/>
        public event Action OnChanged;

        protected void EmitOnChanged()
        {
            OnChanged?.Invoke();
        }

        /// <inheritdoc/>
        public IList<RelationIndexedLogicalQueryNode> GetAllRelations()
        {
            lock (_lockObj)
            {
                return _commonPersistIndexedLogicalData.GetAllRelations();
            }
        }

        /// <inheritdoc/>
        public IList<IndexedBaseRulePart> GetIndexedRulePartOfFactsByKeyOfRelation(ulong key)
        {
            lock (_lockObj)
            {
#if DEBUG
                //LogInstance.Log($"key = {key}");
#endif

                return _commonPersistIndexedLogicalData.GetIndexedRulePartOfFactsByKeyOfRelation(key);
            }
        }

        /// <inheritdoc/>
        public IList<IndexedBaseRulePart> GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(ulong key)
        {
            lock (_lockObj)
            {
                return _commonPersistIndexedLogicalData.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(key);
            }
        }
    }
}
