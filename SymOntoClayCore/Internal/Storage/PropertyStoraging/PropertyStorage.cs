using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.Storage.PropertyStoraging
{
    public class PropertyStorage: BaseSpecificStorage, IPropertyStorage
    {
        public PropertyStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
            _parentPropertyStoragesList = realStorageContext.Parents.Select(p => p.PropertyStorage).ToList();

            foreach (var parentStorage in _parentPropertyStoragesList)
            {
                parentStorage.OnChangedWithKeys += PropertyStorage_OnChangedWithKeys;
            }

            realStorageContext.OnAddParentStorage += RealStorageContext_OnAddParentStorage;
            realStorageContext.OnRemoveParentStorage += RealStorageContext_OnRemoveParentStorage;
        }

        private readonly object _lockObj = new object();

        /// <inheritdoc/>
        public event Action OnChanged;

        /// <inheritdoc/>
        public event Action<StrongIdentifierValue> OnChangedWithKeys;

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
                    itemForRemoving.OnChanged -= PropertyItem_OnChanged;
                    _allPropertiesList.Remove(itemForRemoving);
                }

                targetList.Add(propertyInstance);
            }
            else
            {
                dict[name] = new List<PropertyInstance> { propertyInstance };
            }
        }

        private void PropertyItem_OnChanged(StrongIdentifierValue name)
        {
            EmitOnChanged(Logger, name);
        }

        protected void EmitOnChanged(IMonitorLogger logger, StrongIdentifierValue propertyName)
        {
            OnChanged?.Invoke();

            OnChangedWithKeys?.Invoke(propertyName);
        }

        private void PropertyStorage_OnChangedWithKeys(StrongIdentifierValue propertyName)
        {
            EmitOnChanged(Logger, propertyName);
        }

        private void RealStorageContext_OnRemoveParentStorage(IStorage storage)
        {
            var propertyStorage = storage.PropertyStorage;
            propertyStorage.OnChangedWithKeys -= PropertyStorage_OnChangedWithKeys;

            _parentPropertyStoragesList.Remove(propertyStorage);
        }

        private void RealStorageContext_OnAddParentStorage(IStorage storage)
        {
            var propertyStorage = storage.PropertyStorage;
            propertyStorage.OnChangedWithKeys += PropertyStorage_OnChangedWithKeys;

            _parentPropertyStoragesList.Add(propertyStorage);
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            foreach (var parentStorage in _parentPropertyStoragesList)
            {
                parentStorage.OnChangedWithKeys -= PropertyStorage_OnChangedWithKeys;
            }

            foreach (var propertyItem in _allPropertiesList)
            {
                propertyItem.OnChanged -= PropertyItem_OnChanged;
            }

            _allPropertiesList.Clear();
            //_systemVariables.Clear();
            //_variablesDict.Clear();
            //_localVariablesDict.Clear();

            base.OnDisposed();
        }
    }
}
