using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;

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

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, PropertyInstance propertyInstance)
        {
            throw new NotImplementedException("D552B92B-998B-4FFB-AB79-0D3A725C3228");
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<PropertyInstance>> GetPropertyDirectly(IMonitorLogger logger, StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            throw new NotImplementedException("262837C7-151F-44DF-BC7A-9B547CC4EAF5");
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
            throw new NotImplementedException("7E130BFF-FE19-4153-B4E2-15E878D844A8");
        }

        /// <inheritdoc/>
        public void RemoveOnChangedWithKeysHandler(IOnChangedWithKeysPropertyStorageHandler handler)
        {
            throw new NotImplementedException("BE729409-D67A-426D-B39A-C3EA5EDBF2A4");
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

            return result;
        }
    }
}
