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
using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SymOntoClay.Core.Internal.Storage.TriggersStoraging
{
    public class TriggersStorage: BaseSpecificStorage, ITriggersStorage,
        IOnAddParentStorageRealStorageContextHandler, IOnRemoveParentStorageRealStorageContextHandler,
        IOnChangedNamedTriggerInstanceHandler, IOnNamedTriggerInstanceChangedWithKeysTriggersStorageHandler,
        ITriggersStorageSerializedEventsHandler
    {
        public TriggersStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
            _activeObjectContext = _mainStorageContext.ActiveObjectContext;
            _threadPool = _mainStorageContext.AsyncEventsThreadPool;
            _serializationAnchor = new SerializationAnchor();

            _parentTriggersStoragesList = realStorageContext.Parents.Select(p => p.TriggersStorage).ToList();

            foreach (var parentStorage in _parentTriggersStoragesList)
            {
                parentStorage.AddOnNamedTriggerInstanceChangedWithKeysHandler(this);
            }

            realStorageContext.AddOnAddParentStorageHandler(this);
            realStorageContext.AddOnRemoveParentStorageHandler(this);
        }

        private IActiveObjectContext _activeObjectContext;
        private ICustomThreadPool _threadPool;
        private SerializationAnchor _serializationAnchor;

        private readonly object _lockObj = new object();

        private List<ITriggersStorage> _parentTriggersStoragesList = new List<ITriggersStorage>();

        private Dictionary<KindOfSystemEventOfInlineTrigger, Dictionary<StrongIdentifierValue, List<InlineTrigger>>> _systemEventsInfoDict = new Dictionary<KindOfSystemEventOfInlineTrigger, Dictionary<StrongIdentifierValue, List<InlineTrigger>>>();

        private Dictionary<StrongIdentifierValue, List<InlineTrigger>> _logicConditionalsDict = new Dictionary<StrongIdentifierValue, List<InlineTrigger>>();
        private Dictionary<StrongIdentifierValue, List<InlineTrigger>> _addFactsDict = new Dictionary<StrongIdentifierValue, List<InlineTrigger>>();

        private List<INamedTriggerInstance> _namedTriggerInstancesList = new List<INamedTriggerInstance>();
        private Dictionary<StrongIdentifierValue, List<INamedTriggerInstance>> _namedTriggerInstancesDict = new Dictionary<StrongIdentifierValue, List<INamedTriggerInstance>>();

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, InlineTrigger inlineTrigger)
        {
            AnnotatedItemHelper.CheckAndFillUpHolder(logger, inlineTrigger, _realStorageContext.MainStorageContext.CommonNamesStorage);

            inlineTrigger.CheckDirty();

            var kind = inlineTrigger.KindOfInlineTrigger;

            switch (kind)
            {
                case KindOfInlineTrigger.SystemEvent:
                    AppendSystemEvent(logger, inlineTrigger);
                    break;

                case KindOfInlineTrigger.LogicConditional:
                    AppendLogicConditional(logger, inlineTrigger);
                    break;

                case KindOfInlineTrigger.AddFact:
                    AppendAddFact(logger, inlineTrigger);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private void AppendAddFact(IMonitorLogger logger, InlineTrigger inlineTrigger)
        {
            lock (_lockObj)
            {
                ProcessAppendTrigger(logger, inlineTrigger, _addFactsDict);
            }
        }

        private void AppendLogicConditional(IMonitorLogger logger, InlineTrigger inlineTrigger)
        {
            lock (_lockObj)
            {
                ProcessAppendTrigger(logger, inlineTrigger, _logicConditionalsDict);
            }
        }

        private void ProcessAppendTrigger(IMonitorLogger logger, InlineTrigger inlineTrigger, Dictionary<StrongIdentifierValue, List<InlineTrigger>> dictForStoraging)
        {
            inlineTrigger.CheckDirty();

            var holder = inlineTrigger.Holder;

            if (dictForStoraging.ContainsKey(holder))
            {
                var targetList = dictForStoraging[holder];

                StorageHelper.RemoveSameItems(logger, targetList, inlineTrigger);

                targetList.Add(inlineTrigger);
            }
            else
            {
                dictForStoraging[holder] = new List<InlineTrigger>() { inlineTrigger };
            }
        }

        private void AppendSystemEvent(IMonitorLogger logger, InlineTrigger inlineTrigger)
        {
            lock (_lockObj)
            {
                var kindOfSystemEvent = inlineTrigger.KindOfSystemEvent;

                var holder = inlineTrigger.Holder;

                if (_systemEventsInfoDict.ContainsKey(kindOfSystemEvent))
                {
                    var dict = _systemEventsInfoDict[kindOfSystemEvent];

                    if (dict.ContainsKey(holder))
                    {
                        var targetList = dict[holder];

                        StorageHelper.RemoveSameItems(logger, targetList, inlineTrigger);

                        targetList.Add(inlineTrigger);
                    }
                    else
                    {
                        dict[holder] = new List<InlineTrigger>() { inlineTrigger };
                    }
                }
                else
                {
                    _systemEventsInfoDict[kindOfSystemEvent] = new Dictionary<StrongIdentifierValue, List<InlineTrigger>>() { { holder, new List<InlineTrigger>() { inlineTrigger } } };
                }
            }
        }

        private static List<WeightedInheritanceResultItem<InlineTrigger>> _emptyTriggersList = new List<WeightedInheritanceResultItem<InlineTrigger>>();

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<InlineTrigger>> GetSystemEventsTriggersDirectly(IMonitorLogger logger, KindOfSystemEventOfInlineTrigger kindOfSystemEvent, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyTriggersList;
                }

                if (_systemEventsInfoDict.ContainsKey(kindOfSystemEvent))
                {
                    var dict = _systemEventsInfoDict[kindOfSystemEvent];

                    var result = new List<WeightedInheritanceResultItem<InlineTrigger>>();

                    foreach (var weightedInheritanceItem in weightedInheritanceItems)
                    {
                        var targetHolder = weightedInheritanceItem.SuperType;

                        if (dict.ContainsKey(targetHolder))
                        {
                            var targetList = dict[targetHolder];

                            foreach (var targetVal in targetList)
                            {
                                result.Add(new WeightedInheritanceResultItem<InlineTrigger>(targetVal, weightedInheritanceItem));
                            }
                        }
                    }

                    return result;
                }

                return _emptyTriggersList;
            }
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<InlineTrigger>> GetLogicConditionalTriggersDirectly(IMonitorLogger logger, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyTriggersList;
                }

                return GetTriggersDirectly(logger, weightedInheritanceItems, _logicConditionalsDict);
            }
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<InlineTrigger>> GetAddFactTriggersDirectly(IMonitorLogger logger, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyTriggersList;
                }

                return GetTriggersDirectly(logger, weightedInheritanceItems, _addFactsDict);
            }                
        }

        private IList<WeightedInheritanceResultItem<InlineTrigger>> GetTriggersDirectly(IMonitorLogger logger, IList<WeightedInheritanceItem> weightedInheritanceItems, Dictionary<StrongIdentifierValue, List<InlineTrigger>> dictForStoraging)
        {
            var result = new List<WeightedInheritanceResultItem<InlineTrigger>>();

            foreach (var weightedInheritanceItem in weightedInheritanceItems)
            {
                var targetHolder = weightedInheritanceItem.SuperType.Name;

                if (dictForStoraging.ContainsKey(targetHolder))
                {
                    var targetList = dictForStoraging[targetHolder];

                    foreach (var targetVal in targetList)
                    {
                        result.Add(new WeightedInheritanceResultItem<InlineTrigger>(targetVal, weightedInheritanceItem));
                    }
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, INamedTriggerInstance namedTriggerInstance)
        {
            lock (_lockObj)
            {
                var namesList = namedTriggerInstance.NamesList;

                if (namesList.IsNullOrEmpty())
                {
                    return;
                }

                if(_namedTriggerInstancesList.Contains(namedTriggerInstance))
                {
                    return;
                }

                _namedTriggerInstancesList.Add(namedTriggerInstance);

                namedTriggerInstance.AddOnChangedHandler(this);

                foreach (var name in namesList)
                {
                    if(_namedTriggerInstancesDict.ContainsKey(name))
                    {
                        var targetList = _namedTriggerInstancesDict[name];

                        var itemsForRemoving = StorageHelper.RemoveSameItems(logger, targetList, namedTriggerInstance);

                        foreach (var itemForRemoving in itemsForRemoving)
                        {
                            itemForRemoving.RemoveOnChangedHandler(this);
                            _namedTriggerInstancesList.Remove(itemForRemoving);
                        }

                        targetList.Add(namedTriggerInstance);
                    }
                    else
                    {
                        _namedTriggerInstancesDict[name] = new List<INamedTriggerInstance> { namedTriggerInstance };
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void Remove(IMonitorLogger logger, INamedTriggerInstance namedTriggerInstance)
        {
            lock (_lockObj)
            {
                var namesList = namedTriggerInstance.NamesList;

                if (namesList.IsNullOrEmpty())
                {
                    return;
                }

                if (!_namedTriggerInstancesList.Contains(namedTriggerInstance))
                {
                    return;
                }

                _namedTriggerInstancesList.Remove(namedTriggerInstance);

                namedTriggerInstance.RemoveOnChangedHandler(this);

                foreach (var name in namesList)
                {
                    var targetList = _namedTriggerInstancesDict[name];

                    targetList.Remove(namedTriggerInstance);

                    if(!targetList.Any())
                    {
                        _namedTriggerInstancesDict.Remove(name);
                    }
                }
            }
        }

        private static List<INamedTriggerInstance> _emptyNamedTriggerInstancesList = new List<INamedTriggerInstance>();

        /// <inheritdoc/>
        public IList<INamedTriggerInstance> GetNamedTriggerInstancesDirectly(IMonitorLogger logger, StrongIdentifierValue name)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyNamedTriggerInstancesList;
                }

                if (_namedTriggerInstancesDict.ContainsKey(name))
                {
                    return _namedTriggerInstancesDict[name];
                }

                return null;
            }
        }

        /// <inheritdoc/>
        public void AddOnNamedTriggerInstanceChangedHandler(IOnNamedTriggerInstanceChangedTriggersStorageHandler handler)
        {
            lock (_onNamedTriggerInstanceChangedHandlersLockObj)
            {
                if (_onNamedTriggerInstanceChangedHandlers.Contains(handler))
                {
                    return;
                }

                _onNamedTriggerInstanceChangedHandlers.Add(handler);
            }
        }

        /// <inheritdoc/>
        public void RemoveOnNamedTriggerInstanceChangedHandler(IOnNamedTriggerInstanceChangedTriggersStorageHandler handler)
        {
            lock (_onNamedTriggerInstanceChangedHandlersLockObj)
            {
                if (_onNamedTriggerInstanceChangedHandlers.Contains(handler))
                {
                    _onNamedTriggerInstanceChangedHandlers.Remove(handler);
                }
            }
        }

        private void EmitOnNamedTriggerInstanceChangedHandlers()
        {
            lock (_onNamedTriggerInstanceChangedHandlersLockObj)
            {
                foreach (var handler in _onNamedTriggerInstanceChangedHandlers)
                {
                    handler.Invoke();
                }
            }
        }

        private object _onNamedTriggerInstanceChangedHandlersLockObj = new object();
        private List<IOnNamedTriggerInstanceChangedTriggersStorageHandler> _onNamedTriggerInstanceChangedHandlers = new List<IOnNamedTriggerInstanceChangedTriggersStorageHandler>();

        /// <inheritdoc/>
        public void AddOnNamedTriggerInstanceChangedWithKeysHandler(IOnNamedTriggerInstanceChangedWithKeysTriggersStorageHandler handler)
        {
            lock (_onNamedTriggerInstanceChangedWithKeysHandlersLockObj)
            {
                if (_onNamedTriggerInstanceChangedWithKeysHandlers.Contains(handler))
                {
                    return;
                }

                _onNamedTriggerInstanceChangedWithKeysHandlers.Add(handler);
            }
        }

        /// <inheritdoc/>
        public void RemoveOnNamedTriggerInstanceChangedWithKeysHandler(IOnNamedTriggerInstanceChangedWithKeysTriggersStorageHandler handler)
        {
            lock (_onNamedTriggerInstanceChangedWithKeysHandlersLockObj)
            {
                if (_onNamedTriggerInstanceChangedWithKeysHandlers.Contains(handler))
                {
                    _onNamedTriggerInstanceChangedWithKeysHandlers.Remove(handler);
                }
            }
        }

        private void EmitOnNamedTriggerInstanceChangedWithKeysHandlers(IList<StrongIdentifierValue> value)
        {
            lock (_onNamedTriggerInstanceChangedWithKeysHandlersLockObj)
            {
                foreach (var handler in _onNamedTriggerInstanceChangedWithKeysHandlers)
                {
                    handler.Invoke(value);
                }
            }
        }

        private object _onNamedTriggerInstanceChangedWithKeysHandlersLockObj = new object();
        private List<IOnNamedTriggerInstanceChangedWithKeysTriggersStorageHandler> _onNamedTriggerInstanceChangedWithKeysHandlers = new List<IOnNamedTriggerInstanceChangedWithKeysTriggersStorageHandler>();

        void IOnChangedNamedTriggerInstanceHandler.Invoke(IList<StrongIdentifierValue> value)
        {
            NamedTriggerInstance_OnChanged(value);
        }

        private void NamedTriggerInstance_OnChanged(IList<StrongIdentifierValue> namesList)
        {
            EmitOnChanged(Logger, namesList);
        }

        protected void EmitOnChanged(IMonitorLogger logger, IList<StrongIdentifierValue> namesList)
        {
            EmitOnNamedTriggerInstanceChangedHandlers();

            EmitOnNamedTriggerInstanceChangedWithKeysHandlers(namesList);
        }

        void IOnNamedTriggerInstanceChangedWithKeysTriggersStorageHandler.Invoke(IList<StrongIdentifierValue> value)
        {
            EmitOnChanged(Logger, value);
        }

        void IOnRemoveParentStorageRealStorageContextHandler.Invoke(IStorage storage)
        {
            RealStorageContext_OnRemoveParentStorage(storage);
        }

        private void RealStorageContext_OnRemoveParentStorage(IStorage storage)
        {
            LoggedFunctorWithoutResult<ITriggersStorageSerializedEventsHandler, IStorage>.Run(Logger, "CE9E5E18-3D8B-47B7-874C-FD1E6F985354", this, storage,
                (IMonitorLogger loggerValue, ITriggersStorageSerializedEventsHandler instanceValue, IStorage storageValue) => {
                    instanceValue.NRealStorageContext_OnRemoveParentStorage(storageValue);
                },
                _activeObjectContext, _threadPool, _serializationAnchor);
        }

        void ITriggersStorageSerializedEventsHandler.NRealStorageContext_OnRemoveParentStorage(IStorage storage)
        {
            var triggersStorage = storage.TriggersStorage;
            triggersStorage.RemoveOnNamedTriggerInstanceChangedWithKeysHandler(this);

            _parentTriggersStoragesList.Remove(triggersStorage);
        }

        void IOnAddParentStorageRealStorageContextHandler.Invoke(IStorage storage)
        {
            RealStorageContext_OnAddParentStorage(storage);
        }

        private void RealStorageContext_OnAddParentStorage(IStorage storage)
        {
            LoggedFunctorWithoutResult<ITriggersStorageSerializedEventsHandler, IStorage>.Run(Logger, "403E1946-20F0-40DD-B587-C17B408BA842", this, storage,
                (IMonitorLogger loggerValue, ITriggersStorageSerializedEventsHandler instanceValue, IStorage storageValue) => {
                    instanceValue.NRealStorageContext_OnAddParentStorage(storageValue);
                },
                _activeObjectContext, _threadPool, _serializationAnchor);
        }

        void ITriggersStorageSerializedEventsHandler.NRealStorageContext_OnAddParentStorage(IStorage storage)
        {
            var triggersStorage = storage.TriggersStorage;
            triggersStorage.AddOnNamedTriggerInstanceChangedWithKeysHandler(this);

            _parentTriggersStoragesList.Add(triggersStorage);
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            foreach (var parentStorage in _parentTriggersStoragesList)
            {
                parentStorage.RemoveOnNamedTriggerInstanceChangedWithKeysHandler(this);
            }

            foreach (var item in _namedTriggerInstancesList)
            {
                item.RemoveOnChangedHandler(this);
            }

            _realStorageContext.RemoveOnAddParentStorageHandler(this);
            _realStorageContext.RemoveOnRemoveParentStorageHandler(this);

            _systemEventsInfoDict.Clear();
            _logicConditionalsDict.Clear();

            _onNamedTriggerInstanceChangedHandlers.Clear();
            _onNamedTriggerInstanceChangedWithKeysHandlers.Clear();

            _serializationAnchor.Dispose();

            base.OnDisposed();
        }
    }
}
