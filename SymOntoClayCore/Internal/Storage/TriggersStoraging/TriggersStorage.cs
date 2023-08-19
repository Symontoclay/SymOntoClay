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
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Storage.TriggersStoraging
{
    public class TriggersStorage: BaseSpecificStorage, ITriggersStorage
    {
        public TriggersStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
            _parentTriggersStoragesList = realStorageContext.Parents.Select(p => p.TriggersStorage).ToList();

            foreach (var parentStorage in _parentTriggersStoragesList)
            {
                parentStorage.OnNamedTriggerInstanceChangedWithKeys += TriggersStorage_OnNamedTriggerInstanceChangedWithKeys;
            }

            realStorageContext.OnAddParentStorage += RealStorageContext_OnAddParentStorage;
            realStorageContext.OnRemoveParentStorage += RealStorageContext_OnRemoveParentStorage;
        }

        private readonly object _lockObj = new object();

        private List<ITriggersStorage> _parentTriggersStoragesList = new List<ITriggersStorage>();

        private Dictionary<KindOfSystemEventOfInlineTrigger, Dictionary<StrongIdentifierValue, List<InlineTrigger>>> _systemEventsInfoDict = new Dictionary<KindOfSystemEventOfInlineTrigger, Dictionary<StrongIdentifierValue, List<InlineTrigger>>>();

        private Dictionary<StrongIdentifierValue, List<InlineTrigger>> _logicConditionalsDict = new Dictionary<StrongIdentifierValue, List<InlineTrigger>>();
        private Dictionary<StrongIdentifierValue, List<InlineTrigger>> _addFactsDict = new Dictionary<StrongIdentifierValue, List<InlineTrigger>>();

        private List<INamedTriggerInstance> _namedTriggerInstancesList = new List<INamedTriggerInstance>();
        private Dictionary<StrongIdentifierValue, List<INamedTriggerInstance>> _namedTriggerInstancesDict = new Dictionary<StrongIdentifierValue, List<INamedTriggerInstance>>();

        /// <inheritdoc/>
        public void Append(InlineTrigger inlineTrigger)
        {
            AnnotatedItemHelper.CheckAndFillUpHolder(inlineTrigger, _realStorageContext.MainStorageContext.CommonNamesStorage);

            inlineTrigger.CheckDirty();

            var kind = inlineTrigger.KindOfInlineTrigger;

            switch (kind)
            {
                case KindOfInlineTrigger.SystemEvent:
                    AppendSystemEvent(inlineTrigger);
                    break;

                case KindOfInlineTrigger.LogicConditional:
                    AppendLogicConditional(inlineTrigger);
                    break;

                case KindOfInlineTrigger.AddFact:
                    AppendAddFact(inlineTrigger);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private void AppendAddFact(InlineTrigger inlineTrigger)
        {
            lock (_lockObj)
            {
                ProcessAppendTrigger(inlineTrigger, _addFactsDict);
            }
        }

        private void AppendLogicConditional(InlineTrigger inlineTrigger)
        {
            lock (_lockObj)
            {
                ProcessAppendTrigger(inlineTrigger, _logicConditionalsDict);
            }
        }

        private void ProcessAppendTrigger(InlineTrigger inlineTrigger, Dictionary<StrongIdentifierValue, List<InlineTrigger>> dictForStoraging)
        {
            inlineTrigger.CheckDirty();

            var holder = inlineTrigger.Holder;

            if (dictForStoraging.ContainsKey(holder))
            {
                var targetList = dictForStoraging[holder];

                StorageHelper.RemoveSameItems(targetList, inlineTrigger);

                targetList.Add(inlineTrigger);
            }
            else
            {
                dictForStoraging[holder] = new List<InlineTrigger>() { inlineTrigger };
            }
        }

        private void AppendSystemEvent(InlineTrigger inlineTrigger)
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

                        StorageHelper.RemoveSameItems(targetList, inlineTrigger);

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
        public IList<WeightedInheritanceResultItem<InlineTrigger>> GetSystemEventsTriggersDirectly(KindOfSystemEventOfInlineTrigger kindOfSystemEvent, IList<WeightedInheritanceItem> weightedInheritanceItems)
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
                        var targetHolder = weightedInheritanceItem.SuperName;

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
        public IList<WeightedInheritanceResultItem<InlineTrigger>> GetLogicConditionalTriggersDirectly(IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyTriggersList;
                }

                return GetTriggersDirectly(weightedInheritanceItems, _logicConditionalsDict);
            }
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<InlineTrigger>> GetAddFactTriggersDirectly(IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyTriggersList;
                }

                return GetTriggersDirectly(weightedInheritanceItems, _addFactsDict);
            }                
        }

        private IList<WeightedInheritanceResultItem<InlineTrigger>> GetTriggersDirectly(IList<WeightedInheritanceItem> weightedInheritanceItems, Dictionary<StrongIdentifierValue, List<InlineTrigger>> dictForStoraging)
        {
            var result = new List<WeightedInheritanceResultItem<InlineTrigger>>();

            foreach (var weightedInheritanceItem in weightedInheritanceItems)
            {
                var targetHolder = weightedInheritanceItem.SuperName;

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
        public void Append(INamedTriggerInstance namedTriggerInstance)
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

                namedTriggerInstance.OnChanged += NamedTriggerInstance_OnChanged;

                foreach(var name in namesList)
                {
                    if(_namedTriggerInstancesDict.ContainsKey(name))
                    {
                        var targetList = _namedTriggerInstancesDict[name];

                        var itemsForRemoving = StorageHelper.RemoveSameItems(targetList, namedTriggerInstance);

                        foreach (var itemForRemoving in itemsForRemoving)
                        {
                            itemForRemoving.OnChanged -= NamedTriggerInstance_OnChanged;
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
        public void Remove(INamedTriggerInstance namedTriggerInstance)
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

                namedTriggerInstance.OnChanged -= NamedTriggerInstance_OnChanged;

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
        public IList<INamedTriggerInstance> GetNamedTriggerInstancesDirectly(StrongIdentifierValue name)
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
        public event Action OnNamedTriggerInstanceChanged;

        /// <inheritdoc/>
        public event Action<IList<StrongIdentifierValue>> OnNamedTriggerInstanceChangedWithKeys;

        private void NamedTriggerInstance_OnChanged(IList<StrongIdentifierValue> namesList)
        {
            EmitOnChanged(namesList);
        }

        protected void EmitOnChanged(IList<StrongIdentifierValue> namesList)
        {
            Task.Run(() => {
                try
                {
                    OnNamedTriggerInstanceChanged?.Invoke();
                }
                catch (Exception e)
                {
                    Error("E2154301-E9FC-4720-B6D9-DC6B1ACF6FFA", e);
                }                
            });

            Task.Run(() => {
                try
                {
                    OnNamedTriggerInstanceChangedWithKeys?.Invoke(namesList);
                }
                catch (Exception e)
                {
                    Error("5F87DF78-676E-4725-A534-A38299DCF1A5", e);
                }                
            });
        }

        private void TriggersStorage_OnNamedTriggerInstanceChangedWithKeys(IList<StrongIdentifierValue> namesList)
        {
            EmitOnChanged(namesList);
        }

        private void RealStorageContext_OnRemoveParentStorage(IStorage storage)
        {
            var triggersStorage = storage.TriggersStorage;
            triggersStorage.OnNamedTriggerInstanceChangedWithKeys -= TriggersStorage_OnNamedTriggerInstanceChangedWithKeys;

            _parentTriggersStoragesList.Remove(triggersStorage);
        }

        private void RealStorageContext_OnAddParentStorage(IStorage storage)
        {
            var triggersStorage = storage.TriggersStorage;
            triggersStorage.OnNamedTriggerInstanceChangedWithKeys += TriggersStorage_OnNamedTriggerInstanceChangedWithKeys;

            _parentTriggersStoragesList.Add(triggersStorage);
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            foreach (var parentStorage in _parentTriggersStoragesList)
            {
                parentStorage.OnNamedTriggerInstanceChangedWithKeys -= TriggersStorage_OnNamedTriggerInstanceChangedWithKeys;
            }

            foreach (var item in _namedTriggerInstancesList)
            {
                item.OnChanged += NamedTriggerInstance_OnChanged;
            }

            _systemEventsInfoDict.Clear();
            _logicConditionalsDict.Clear();

            base.OnDisposed();
        }
    }
}
