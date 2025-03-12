/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.VarStoraging
{
    public class VarStorage: BaseSpecificStorage, IVarStorage,
        IOnAddParentStorageRealStorageContextHandler, IOnRemoveParentStorageRealStorageContextHandler, IOnChangedWithKeysVarStorageHandler, IOnChangedVarHandler,
        IVarStorageSerializedEventsHandler
    {
        public VarStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
            _activeObjectContext = _mainStorageContext.ActiveObjectContext;
            _threadPool = _mainStorageContext.AsyncEventsThreadPool;
            _serializationAnchor = new SerializationAnchor();

            _parentVarStoragesList = realStorageContext.Parents.Select(p => p.VarStorage).ToList();

            foreach (var parentStorage in _parentVarStoragesList)
            {
                parentStorage.AddOnChangedWithKeysHandler(this);
            }

            realStorageContext.AddOnAddParentStorageHandler(this);
            realStorageContext.AddOnRemoveParentStorageHandler(this);
        }

        private IActiveObjectContext _activeObjectContext;
        private ICustomThreadPool _threadPool;
        private SerializationAnchor _serializationAnchor;

        private readonly object _lockObj = new object();

        /// <inheritdoc/>
        public void AddOnChangedHandler(IOnChangedVarStorageHandler handler)
        {
            lock (_onChangedHandlersLockObj)
            {
                if (_onChangedHandlers.Contains(handler))
                {
                    return;
                }

                _onChangedHandlers.Add(handler);
            }
        }

        /// <inheritdoc/>
        public void RemoveOnChangedHandler(IOnChangedVarStorageHandler handler)
        {
            lock (_onChangedHandlersLockObj)
            {
                if (_onChangedHandlers.Contains(handler))
                {
                    _onChangedHandlers.Remove(handler);
                }
            }
        }

        private void EmitOnChangedHandlers()
        {
            lock (_onChangedHandlersLockObj)
            {
                foreach (var handler in _onChangedHandlers)
                {
                    handler.Invoke();
                }
            }
        }

        private object _onChangedHandlersLockObj = new object();
        private List<IOnChangedVarStorageHandler> _onChangedHandlers = new List<IOnChangedVarStorageHandler>();

        /// <inheritdoc/>
        public void AddOnChangedWithKeysHandler(IOnChangedWithKeysVarStorageHandler handler)
        {
            lock (_onChangedWithKeysHandlersLockObj)
            {
                if (_onChangedWithKeysHandlers.Contains(handler))
                {
                    return;
                }

                _onChangedWithKeysHandlers.Add(handler);
            }
        }

        /// <inheritdoc/>
        public void RemoveOnChangedWithKeysHandler(IOnChangedWithKeysVarStorageHandler handler)
        {
            lock (_onChangedWithKeysHandlersLockObj)
            {
                if (_onChangedWithKeysHandlers.Contains(handler))
                {
                    _onChangedWithKeysHandlers.Remove(handler);
                }
            }
        }

        private void EmitOnChangedWithKeysHandlers(StrongIdentifierValue value)
        {
            lock (_onChangedWithKeysHandlersLockObj)
            {
                foreach (var handler in _onChangedWithKeysHandlers)
                {
                    handler.Invoke(value);
                }
            }
        }

        private object _onChangedWithKeysHandlersLockObj = new object();
        private List<IOnChangedWithKeysVarStorageHandler> _onChangedWithKeysHandlers = new List<IOnChangedWithKeysVarStorageHandler>();

        private List<IVarStorage> _parentVarStoragesList = new List<IVarStorage>();

        private Dictionary<TypeInfo, Dictionary<StrongIdentifierValue, List<VarInstance>>> _variablesDict = new Dictionary<TypeInfo, Dictionary<StrongIdentifierValue, List<VarInstance>>>();
        private Dictionary<StrongIdentifierValue, VarInstance> _localVariablesDict = new Dictionary<StrongIdentifierValue, VarInstance>();

        private List<VarInstance> _allVariablesList = new List<VarInstance>();

        private Dictionary<StrongIdentifierValue, Value> _systemVariables = new Dictionary<StrongIdentifierValue, Value>();
        
        /// <inheritdoc/>
        public void SetSystemValue(IMonitorLogger logger, StrongIdentifierValue varName, Value value)
        {
            lock(_lockObj)
            {
                _systemVariables[varName] = value;
            }
        }

        /// <inheritdoc/>
        public Value GetSystemValueDirectly(IMonitorLogger logger, StrongIdentifierValue varName)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return null;
                }

                if (_systemVariables.ContainsKey(varName))
                {
                    return _systemVariables[varName];
                }

                return null;
            }
        }

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, VarInstance varItem)
        {
            lock (_lockObj)
            {
                NAppendVar(logger, varItem);
            }
        }

        private void NAppendVar(IMonitorLogger logger, VarInstance varItem)
        {
            if(_allVariablesList.Contains(varItem))
            {
                return;
            }

            _allVariablesList.Add(varItem);

            if (varItem.TypeOfAccess != TypeOfAccess.Local)
            {
                AnnotatedItemHelper.CheckAndFillUpHolder(logger, varItem.CodeItem, _realStorageContext.MainStorageContext.CommonNamesStorage);
            }

            varItem.AddOnChangedHandler(this);

            var name = varItem.Name;

            if (varItem.TypeOfAccess == TypeOfAccess.Local)
            {
                _localVariablesDict[name] = varItem;

                return;
            }

            var holder = varItem.Holder;

            Dictionary<StrongIdentifierValue, List<VarInstance>> dict = null;

            if (_variablesDict.ContainsKey(holder))
            {
                dict = _variablesDict[holder];
            }
            else
            {
                dict = new Dictionary<StrongIdentifierValue, List<VarInstance>>();
                _variablesDict[holder] = dict;
            }

            if (dict.ContainsKey(name))
            {
                var targetList = dict[name];

                var itemsForRemoving = StorageHelper.RemoveSameItems(logger, targetList, varItem);

                foreach(var itemForRemoving in itemsForRemoving)
                {
                    itemForRemoving.RemoveOnChangedHandler(this);
                    _allVariablesList.Remove(itemForRemoving);
                }

                targetList.Add(varItem);
            }
            else
            {
                dict[name] = new List<VarInstance> { varItem };
            }
        }

        private static List<WeightedInheritanceResultItem<VarInstance>> _emptyVarsList = new List<WeightedInheritanceResultItem<VarInstance>>();

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<VarInstance>> GetVarDirectly(IMonitorLogger logger, StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyVarsList;
                }

                var result = new List<WeightedInheritanceResultItem<VarInstance>>();

                foreach (var weightedInheritanceItem in weightedInheritanceItems)
                {
                    var targetHolder = weightedInheritanceItem.SuperType;

                    if(_variablesDict.ContainsKey(targetHolder))
                    {
                        var targetDict = _variablesDict[targetHolder];

                        if (targetDict.ContainsKey(name))
                        {
                            var targetList = targetDict[name];

                            foreach (var targetVal in targetList)
                            {
                                result.Add(new WeightedInheritanceResultItem<VarInstance>(targetVal, weightedInheritanceItem));
                            }
                        }
                    }
                }

                if(_localVariablesDict.ContainsKey(name))
                {
                    var targetVar = _localVariablesDict[name];

                    result.Add(new WeightedInheritanceResultItem<VarInstance>(targetVar, null));
                }

                return result;
            }
        }

        /// <inheritdoc/>
        public VarInstance GetLocalVarDirectly(IMonitorLogger logger, StrongIdentifierValue name)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return null;
                }

                if (_localVariablesDict.ContainsKey(name))
                {
                    return _localVariablesDict[name];
                }

                return null;
            }
        }

        /// <inheritdoc/>
        public void SetValue(IMonitorLogger logger, StrongIdentifierValue varName, Value value)
        {
            lock (_lockObj)
            {
                if (_localVariablesDict.ContainsKey(varName))
                {
                    _localVariablesDict[varName].SetValue(logger, value);
                    return;
                }

                var varItem = new VarInstance(varName, TypeOfAccess.Local, _mainStorageContext);

                NAppendVar(logger, varItem);

                varItem.SetValue(logger, value);
            }
        }

        void IOnChangedVarHandler.Invoke(StrongIdentifierValue value)
        {
            VarItem_OnChanged(value);
        }

        private void VarItem_OnChanged(StrongIdentifierValue name)
        {
            EmitOnChanged(Logger, name);
        }

        protected void EmitOnChanged(IMonitorLogger logger, StrongIdentifierValue varName)
        {
            EmitOnChangedHandlers();

            EmitOnChangedWithKeysHandlers(varName);
        }

        void IOnChangedWithKeysVarStorageHandler.Invoke(StrongIdentifierValue value)
        {
            VarStorage_OnChangedWithKeys(value);
        }

        private void VarStorage_OnChangedWithKeys(StrongIdentifierValue varName)
        {
            EmitOnChanged(Logger, varName);
        }

        void IOnRemoveParentStorageRealStorageContextHandler.Invoke(IStorage storage)
        {
            RealStorageContext_OnRemoveParentStorage(storage);
        }

        private void RealStorageContext_OnRemoveParentStorage(IStorage storage)
        {
            LoggedFunctorWithoutResult<IVarStorageSerializedEventsHandler, IStorage>.Run(Logger, "B0E0BA4D-6F0D-4C69-BC14-0FEC022DE01C", this, storage,
                (IMonitorLogger loggerValue, IVarStorageSerializedEventsHandler instanceValue, IStorage storageValue) => {
                    instanceValue.NRealStorageContext_OnRemoveParentStorage(storageValue);
                },
                _activeObjectContext, _threadPool, _serializationAnchor);
        }

        void IVarStorageSerializedEventsHandler.NRealStorageContext_OnRemoveParentStorage(IStorage storage)
        {
            var varStorage = storage.VarStorage;
            varStorage.RemoveOnChangedWithKeysHandler(this);

            _parentVarStoragesList.Remove(varStorage);
        }

        void IOnAddParentStorageRealStorageContextHandler.Invoke(IStorage storage)
        {
            RealStorageContext_OnAddParentStorage(storage);
        }

        private void RealStorageContext_OnAddParentStorage(IStorage storage)
        {
            LoggedFunctorWithoutResult<IVarStorageSerializedEventsHandler, IStorage>.Run(Logger, "2C400FE9-AFAE-4819-AC7B-35D9DFFB687A", this, storage,
                (IMonitorLogger loggerValue, IVarStorageSerializedEventsHandler instanceValue, IStorage storageValue) => {
                    instanceValue.NRealStorageContext_OnAddParentStorage(storageValue);
                },
                _activeObjectContext, _threadPool, _serializationAnchor);
        }

        void IVarStorageSerializedEventsHandler.NRealStorageContext_OnAddParentStorage(IStorage storage)
        {
            var varStorage = storage.VarStorage;
            varStorage.AddOnChangedWithKeysHandler(this);

            _parentVarStoragesList.Add(varStorage);
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            foreach (var parentStorage in _parentVarStoragesList)
            {
                parentStorage.RemoveOnChangedWithKeysHandler(this);
            }

            foreach(var varItem in _allVariablesList)
            {
                varItem.RemoveOnChangedHandler(this);
            }

            _realStorageContext.RemoveOnAddParentStorageHandler(this);
            _realStorageContext.RemoveOnRemoveParentStorageHandler(this);

            _allVariablesList.Clear();
            _systemVariables.Clear();
            _variablesDict.Clear();
            _localVariablesDict.Clear();

            base.OnDisposed();
        }

#if DEBUG
        /// <inheritdoc/>
        public void DbgPrintVariables(IMonitorLogger logger)
        {
            lock (_lockObj)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"({GetHashCode()}) Begin {_kind} of {_mainStorageContext.Id}");

                foreach(var varItem in _allVariablesList)
                {
                    sb.AppendLine($"({varItem.GetHashCode()}){varItem.ToHumanizedString()}");
                }

                sb.AppendLine($"({GetHashCode()}) End {_kind} of {_mainStorageContext.Id}");

                logger.Info("FE4DBAF2-5939-4194-8EA9-1D52F60D8F1A", sb.ToString());
            }                
        }
#endif
    }
}
