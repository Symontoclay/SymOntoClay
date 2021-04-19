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
using NLog;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Storage.LogicalStorage
{
    public class LogicalStorage: BaseLoggedComponent, ILogicalStorage
    {
#if DEBUG
        //private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public LogicalStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.MainStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
            _mainStorageContext = realStorageContext.MainStorageContext;
            _parentLogicalStoragesList = realStorageContext.Parents.Select(p => p.LogicalStorage).ToList();

            _ruleInstancesList = new List<RuleInstance>();
            _ruleInstancesDict = new Dictionary<StrongIdentifierValue, RuleInstance>();
            _ruleInstancesDictByHashCode = new Dictionary<ulong, RuleInstance>();
            _ruleInstancesDictById = new Dictionary<string, RuleInstance>();
            _commonPersistIndexedLogicalData = new CommonPersistIndexedLogicalData(realStorageContext.MainStorageContext.Logger);

            foreach(var parentStorage in _parentLogicalStoragesList)
            {
                parentStorage.OnChangedWithKeys += LogicalStorage_OnChangedWithKeys;
            }

            realStorageContext.OnAddParentStorage += RealStorageContext_OnAddParentStorage;
            realStorageContext.OnRemoveParentStorage += RealStorageContext_OnRemoveParentStorage;
        }

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;
        private readonly IMainStorageContext _mainStorageContext;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;

        private readonly object _lockObj = new object();

        private List<RuleInstance> _ruleInstancesList;
        private Dictionary<StrongIdentifierValue, RuleInstance> _ruleInstancesDict;
        private Dictionary<ulong, RuleInstance> _ruleInstancesDictByHashCode;
        private Dictionary<string, RuleInstance> _ruleInstancesDictById;
        private readonly CommonPersistIndexedLogicalData _commonPersistIndexedLogicalData;
        private List<ILogicalStorage> _parentLogicalStoragesList = new List<ILogicalStorage>();

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
                ruleInstance.CheckDirty();

                var annotationsList = ruleInstance.GetAllAnnotations();

#if DEBUG
                //_gbcLogger.Info($"ruleInstance = {ruleInstance}");
                ////_gbcLogger.Info($"ruleInstance = {DebugHelperForRuleInstance.ToString(ruleInstance)}");
                //Log($"annotationsList = {annotationsList.WriteListToString()}");
#endif

                foreach (var annotationRuleInstance in annotationsList)
                {
                    NAppend(annotationRuleInstance, false);
                }

                NAppend(ruleInstance, isPrimary);

                //var indexedRuleInstance = ruleInstance.GetIndexed(_mainStorageContext);

                var usedKeysList = ruleInstance.Normalized.UsedKeysList.Concat(annotationsList.SelectMany(p => p.UsedKeysList)).Distinct().ToList();

                EmitOnChanged(usedKeysList);
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

            var ruleInstanceId = ruleInstance.Name.NameValue;

            if (_ruleInstancesDictById.ContainsKey(ruleInstanceId))
            {
                return;
            }

            //ruleInstance = ruleInstance.Clone();

#if DEBUG
            //Log($"ruleInstance (after) = {ruleInstance}");
#endif

            //var indexedRuleInstance = ruleInstance.GetIndexed(_realStorageContext.MainStorageContext);

#if DEBUG
            //Log($"indexedRuleInstance = {indexedRuleInstance}");
#endif

            var ruleInstanceName = ruleInstance.Name;

#if DEBUG
            //Log($"ruleInstanceName = {ruleInstanceName}");
#endif

            if (_ruleInstancesDict.ContainsKey(ruleInstanceName))
            {
                return;
            }

            var longHashCode = ruleInstance.GetLongHashCode();

#if DEBUG
            //Log($"longHashCode = {longHashCode}");
#endif

            if(_ruleInstancesDictByHashCode.ContainsKey(longHashCode))
            {
                return;
            }

            _ruleInstancesList.Add(ruleInstance);
            _ruleInstancesDict[ruleInstanceName] = ruleInstance;
            _ruleInstancesDictByHashCode[longHashCode] = ruleInstance;
            _ruleInstancesDictById[ruleInstanceId] = ruleInstance;

#if DEBUG
            //Log($"ruleInstance = {DebugHelperForRuleInstance.ToString(ruleInstance)}");
            //Log($"ruleInstance.Normalized = {DebugHelperForRuleInstance.ToString(ruleInstance.Normalized)}");
#endif            

            _commonPersistIndexedLogicalData.NSetIndexedRuleInstanceToIndexData(ruleInstance.Normalized);

            if(isPrimary && _kind != KindOfStorage.PublicFacts && _kind != KindOfStorage.PerceptedFacts)
            {
#if DEBUG
                //Log($"_kind = {_kind}");
#endif

                var inheritanceRelationsList = ruleInstance.GetInheritanceRelations();
                //var ruleInstanceName = ruleInstance.Name.NameValue;

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
            
#if IMAGINE_WORKING
            //Log("End");
#else
            throw new NotImplementedException();
#endif
        }

        /// <inheritdoc/>
        public void Remove(RuleInstance ruleInstance)
        {
            lock (_lockObj)
            {
                NRemove(ruleInstance);
            }
        }

        /// <inheritdoc/>
        public void RemoveById(string id)
        {
            lock (_lockObj)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return;
                }

                if (_ruleInstancesDictById.ContainsKey(id))
                {
                    NRemove(_ruleInstancesDictById[id]);
                }
            }
        }

        private void NRemove(RuleInstance ruleInstance)
        {
            if(!_ruleInstancesList.Contains(ruleInstance))
            {
                return;
            }

            _ruleInstancesList.Remove(ruleInstance);

            var ruleInstanceName = ruleInstance.Name;

            _ruleInstancesDict.Remove(ruleInstanceName);

            var longHashCode = ruleInstance.GetLongHashCode();

            _ruleInstancesDictByHashCode.Remove(longHashCode);

            var ruleInstanceId = ruleInstance.Name.NameValue;

            _ruleInstancesDictById.Remove(ruleInstanceId);

            _commonPersistIndexedLogicalData.NRemoveIndexedRuleInstanceFromIndexData(ruleInstance);

            EmitOnChanged(ruleInstance.UsedKeysList);
        }

        /// <inheritdoc/>
        public event Action OnChanged;

        /// <inheritdoc/>
        public event Action<IList<StrongIdentifierValue>> OnChangedWithKeys;

        protected void EmitOnChanged(IList<StrongIdentifierValue> usedKeysList)
        {
#if DEBUG
            //Log($"usedKeysList = {JsonConvert.SerializeObject(usedKeysList)}");
#endif

            Task.Run(() => {
                OnChanged?.Invoke();
            });

            Task.Run(() => {
                OnChangedWithKeys?.Invoke(usedKeysList);
            });
        }

        private void LogicalStorage_OnChangedWithKeys(IList<StrongIdentifierValue> changedKeysList)
        {
#if DEBUG
            //Log($"changedKeysList = {JsonConvert.SerializeObject(changedKeysList)}");
#endif

            EmitOnChanged(changedKeysList);
        }

        private void RealStorageContext_OnRemoveParentStorage(IStorage storage)
        {
            var logicalStroage = storage.LogicalStorage;
            logicalStroage.OnChangedWithKeys -= LogicalStorage_OnChangedWithKeys;

            _parentLogicalStoragesList.Remove(logicalStroage);
        }

        private void RealStorageContext_OnAddParentStorage(IStorage storage)
        {
            var logicalStroage = storage.LogicalStorage;
            logicalStroage.OnChangedWithKeys += LogicalStorage_OnChangedWithKeys;

            _parentLogicalStoragesList.Add(logicalStroage);
        }

        /// <inheritdoc/>
        public IList<LogicalQueryNode> GetAllRelations()
        {
            lock (_lockObj)
            {
                return _commonPersistIndexedLogicalData.GetAllRelations();
            }
        }

        /// <inheritdoc/>
        public IList<BaseRulePart> GetIndexedRulePartOfFactsByKeyOfRelation(StrongIdentifierValue name)
        {
            lock (_lockObj)
            {
#if DEBUG
                //LogInstance.Log($"key = {key}");
#endif

                return _commonPersistIndexedLogicalData.GetIndexedRulePartOfFactsByKeyOfRelation(name);
            }
        }

        /// <inheritdoc/>
        public IList<BaseRulePart> GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(StrongIdentifierValue name)
        {
            lock (_lockObj)
            {
                return _commonPersistIndexedLogicalData.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(name);
            }
        }

#if DEBUG
        /// <inheritdoc/>
        public void DbgPrintFactsAndRules()
        {
            lock (_lockObj)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Begin {_kind} of {_mainStorageContext.Id}");

                foreach (var ruleInstance in _ruleInstancesList)
                {
                    sb.AppendLine(DebugHelperForRuleInstance.ToString(ruleInstance));
                }

                sb.AppendLine($"End {_kind} of {_mainStorageContext.Id}");

                Log(sb.ToString());
            }
        }
#endif
    }
}
