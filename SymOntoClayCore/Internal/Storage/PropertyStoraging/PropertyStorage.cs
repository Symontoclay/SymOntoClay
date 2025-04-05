using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.Storage.VarStoraging;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SymOntoClay.Core.Internal.Storage.PropertyStoraging
{
    public class PropertyStorage: BaseSpecificStorage, IPropertyStorage,
        IOnAddParentStorageRealStorageContextHandler, IOnRemoveParentStorageRealStorageContextHandler, IOnChangedWithKeysPropertyStorageHandler, IOnChangedPropertyHandler,
        IPropertyStorageSerializedEventsHandler
    {
        public PropertyStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
            _activeObjectContext = _mainStorageContext.ActiveObjectContext;
            _threadPool = _mainStorageContext.AsyncEventsThreadPool;
            _serializationAnchor = new SerializationAnchor();

            _parentPropertyStoragesList = realStorageContext.Parents.Select(p => p.PropertyStorage).ToList();

            foreach (var parentStorage in _parentPropertyStoragesList)
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
        public void AddOnChangedHandler(IOnChangedPropertyStorageHandler handler)
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
        public void RemoveOnChangedHandler(IOnChangedPropertyStorageHandler handler)
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
        private List<IOnChangedPropertyStorageHandler> _onChangedHandlers = new List<IOnChangedPropertyStorageHandler>();

        /// <inheritdoc/>
        public void AddOnChangedWithKeysHandler(IOnChangedWithKeysPropertyStorageHandler handler)
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
        public void RemoveOnChangedWithKeysHandler(IOnChangedWithKeysPropertyStorageHandler handler)
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
        private List<IOnChangedWithKeysPropertyStorageHandler> _onChangedWithKeysHandlers = new List<IOnChangedWithKeysPropertyStorageHandler>();

        private List<IPropertyStorage> _parentPropertyStoragesList = new List<IPropertyStorage>();

        private List<PropertyInstance> _allPropertiesList = new List<PropertyInstance>();
        private Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<PropertyInstance>>> _propertiesDict = new Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<PropertyInstance>>>();

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, PropertyInstance propertyInstance)
        {
            lock (_lockObj)
            {
                NAppend(logger, propertyInstance);
            }
        }

        private void NAppend(IMonitorLogger logger, PropertyInstance propertyInstance)
        {
            if (_allPropertiesList.Contains(propertyInstance))
            {
                return;
            }

            _allPropertiesList.Add(propertyInstance);

            AnnotatedItemHelper.CheckAndFillUpHolder(logger, propertyInstance.CodeItem, _realStorageContext.MainStorageContext.CommonNamesStorage);

            propertyInstance.SetPropertyStorage(this);
            propertyInstance.AddOnChangedHandler(this);

            var name = propertyInstance.Name;
            var holder = propertyInstance.Holder;

            Dictionary<StrongIdentifierValue, List<PropertyInstance>> dict = null;

            if (_propertiesDict.ContainsKey(holder))
            {
                dict = _propertiesDict[holder];
            }
            else
            {
                dict = new Dictionary<StrongIdentifierValue, List<PropertyInstance>>();
                _propertiesDict[holder] = dict;
            }

            if (dict.ContainsKey(name))
            {
                var targetList = dict[name];

                var itemsForRemoving = StorageHelper.RemoveSameItems(logger, targetList, propertyInstance);

                foreach (var itemForRemoving in itemsForRemoving)
                {
                    itemForRemoving.RemoveOnChangedHandler(this);
                    _allPropertiesList.Remove(itemForRemoving);
                }

                targetList.Add(propertyInstance);
            }
            else
            {
                dict[name] = new List<PropertyInstance> { propertyInstance };
            }
        }

        private static List<WeightedInheritanceResultItem<PropertyInstance>> _emptyPropertiesList = new List<WeightedInheritanceResultItem<PropertyInstance>>();

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<PropertyInstance>> GetPropertyDirectly(IMonitorLogger logger, StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyPropertiesList;
                }

                var result = new List<WeightedInheritanceResultItem<PropertyInstance>>();

                foreach (var weightedInheritanceItem in weightedInheritanceItems)
                {
                    var targetHolder = weightedInheritanceItem.SuperName;

#if DEBUG
                    //Info("E73323C9-4988-46E5-9E15-67792538617D", $"targetHolder = {targetHolder.ToHumanizedString()}");
#endif

                    if (_propertiesDict.ContainsKey(targetHolder))
                    {
                        var targetDict = _propertiesDict[targetHolder];

                        if (targetDict.ContainsKey(name))
                        {
                            var targetList = targetDict[name];

                            foreach (var targetVal in targetList)
                            {
                                result.Add(new WeightedInheritanceResultItem<PropertyInstance>(targetVal, weightedInheritanceItem));
                            }
                        }
                    }
                }

                return result;
            }
        }

        void IOnChangedPropertyHandler.Invoke(StrongIdentifierValue value)
        {
            PropertyItem_OnChanged(value);
        }

        private void PropertyItem_OnChanged(StrongIdentifierValue name)
        {
            EmitOnChanged(Logger, name);
        }

        protected void EmitOnChanged(IMonitorLogger logger, StrongIdentifierValue propertyName)
        {
            EmitOnChangedHandlers();

            EmitOnChangedWithKeysHandlers(propertyName);
        }

        void IOnChangedWithKeysPropertyStorageHandler.Invoke(StrongIdentifierValue value)
        {
            PropertyStorage_OnChangedWithKeys(value);
        }

        private void PropertyStorage_OnChangedWithKeys(StrongIdentifierValue propertyName)
        {
            EmitOnChanged(Logger, propertyName);
        }

        void IOnRemoveParentStorageRealStorageContextHandler.Invoke(IStorage storage)
        {
            RealStorageContext_OnRemoveParentStorage(storage);
        }

        private void RealStorageContext_OnRemoveParentStorage(IStorage storage)
        {
            LoggedFunctorWithoutResult<IPropertyStorageSerializedEventsHandler, IStorage>.Run(Logger, "B0E0BA4D-6F0D-4C69-BC14-0FEC022DE01C", this, storage,
                (IMonitorLogger loggerValue, IPropertyStorageSerializedEventsHandler instanceValue, IStorage storageValue) => {
                    instanceValue.NRealStorageContext_OnRemoveParentStorage(storageValue);
                },
                _activeObjectContext, _threadPool, _serializationAnchor);
        }

        void IPropertyStorageSerializedEventsHandler.NRealStorageContext_OnRemoveParentStorage(IStorage storage)
        {
            var propertyStorage = storage.PropertyStorage;
            propertyStorage.RemoveOnChangedWithKeysHandler(this);

            _parentPropertyStoragesList.Remove(propertyStorage);
        }

        void IOnAddParentStorageRealStorageContextHandler.Invoke(IStorage storage)
        {
            RealStorageContext_OnAddParentStorage(storage);
        }

        private void RealStorageContext_OnAddParentStorage(IStorage storage)
        {
            LoggedFunctorWithoutResult<IPropertyStorageSerializedEventsHandler, IStorage>.Run(Logger, "2C400FE9-AFAE-4819-AC7B-35D9DFFB687A", this, storage,
                (IMonitorLogger loggerValue, IPropertyStorageSerializedEventsHandler instanceValue, IStorage storageValue) => {
                    instanceValue.NRealStorageContext_OnAddParentStorage(storageValue);
                },
                _activeObjectContext, _threadPool, _serializationAnchor);
        }

        void IPropertyStorageSerializedEventsHandler.NRealStorageContext_OnAddParentStorage(IStorage storage)
        {
            var propertyStorage = storage.PropertyStorage;
            propertyStorage.AddOnChangedWithKeysHandler(this);

            _parentPropertyStoragesList.Add(propertyStorage);
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            foreach (var parentStorage in _parentPropertyStoragesList)
            {
                parentStorage.RemoveOnChangedWithKeysHandler(this);
            }

            foreach (var propertyItem in _allPropertiesList)
            {
                propertyItem.RemoveOnChangedHandler(this);
            }

            _realStorageContext.RemoveOnAddParentStorageHandler(this);
            _realStorageContext.RemoveOnRemoveParentStorageHandler(this);

            _allPropertiesList.Clear();
            //_systemVariables.Clear();
            //_variablesDict.Clear();
            //_localVariablesDict.Clear();

            base.OnDisposed();
        }
    }
}
