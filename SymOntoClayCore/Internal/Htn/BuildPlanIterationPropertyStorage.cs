using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.Htn
{
    public class BuildPlanIterationPropertyStorage: BaseComponent, IPropertyStorage
    {
        public BuildPlanIterationPropertyStorage(IStorage storage, IMonitorLogger logger)
            : base(logger)
        {
            _storage = storage;
        }

        private readonly IStorage _storage;

        /// <inheritdoc/>
        public KindOfStorage Kind => _storage.Kind;

        /// <inheritdoc/>
        public IStorage Storage => _storage;

        private readonly object _lockObj = new object();
        private List<PropertyInstance> _allPropertiesList = new List<PropertyInstance>();
        private Dictionary<StrongIdentifierValue, List<PropertyInstance>> _propertiesDict = new Dictionary<StrongIdentifierValue, List<PropertyInstance>>();

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

            propertyInstance.SetPropertyStorage(this);

            var name = propertyInstance.Name;

            if (_propertiesDict.ContainsKey(name))
            {
                var targetList = _propertiesDict[name];

                var itemsForRemoving = StorageHelper.RemoveSameItems(logger, targetList, propertyInstance);

                foreach (var itemForRemoving in itemsForRemoving)
                {
                    //itemForRemoving.RemoveOnChangedHandler(this);
                    _allPropertiesList.Remove(itemForRemoving);
                }

                targetList.Add(propertyInstance);
            }
            else
            {
                _propertiesDict[name] = new List<PropertyInstance> { propertyInstance };
            }
        }

        private static List<WeightedInheritanceResultItem<PropertyInstance>> _getPropertyDirectlyEmptyList = new List<WeightedInheritanceResultItem<PropertyInstance>>();
        
        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<PropertyInstance>> GetPropertyDirectly(IMonitorLogger logger, StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            //Info("54446AC9-2652-4C32-A3D8-AC856B1A74E0", $"name = {name}");
#endif

            var result = new List<WeightedInheritanceResultItem<PropertyInstance>>();

            var firstWeightedInheritanceItem = weightedInheritanceItems.First();

            if (_propertiesDict.ContainsKey(name))
            {
                var targetList = _propertiesDict[name];

                foreach (var targetVal in targetList)
                {
                    result.Add(new WeightedInheritanceResultItem<PropertyInstance>(targetVal, firstWeightedInheritanceItem));
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public void AddOnChangedHandler(IOnChangedPropertyStorageHandler handler)
        {
            throw new NotImplementedException("5AADCE05-A0A2-4D43-8D43-00BC82CDB5B2");
        }

        /// <inheritdoc/>
        public void RemoveOnChangedHandler(IOnChangedPropertyStorageHandler handler)
        {
            throw new NotImplementedException("BE964648-5934-4972-A603-9C5EB1B87F5F");
        }

        /// <inheritdoc/>
        public void AddOnChangedWithKeysHandler(IOnChangedWithKeysPropertyStorageHandler handler)
        {
            //throw new NotImplementedException("7E130BFF-FE19-4153-B4E2-15E878D844A8");
        }

        /// <inheritdoc/>
        public void RemoveOnChangedWithKeysHandler(IOnChangedWithKeysPropertyStorageHandler handler)
        {
            //throw new NotImplementedException("BE729409-D67A-426D-B39A-C3EA5EDBF2A4");
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public BuildPlanIterationPropertyStorage Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public BuildPlanIterationPropertyStorage Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (BuildPlanIterationPropertyStorage)context[this];
            }

            var result = new BuildPlanIterationPropertyStorage(Storage, Logger);
            context[this] = result;

            result._allPropertiesList = _allPropertiesList.Select(p => p.Clone(context)).ToList();

            foreach(var item in result._allPropertiesList)
            {
                item.SetPropertyStorage(result);
            }

            return result;
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _allPropertiesList.Clear();

            base.OnDisposed();
        }
    }
}
