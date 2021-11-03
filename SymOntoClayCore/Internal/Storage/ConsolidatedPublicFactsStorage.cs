using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Storage.InheritanceStorage;
using SymOntoClay.Core.Internal.Storage.LogicalStorage;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class ConsolidatedPublicFactsStorage: BaseComponent, IStorage
    {
        public ConsolidatedPublicFactsStorage(IEntityLogger logger)
            : base(logger)
        {
            _logicalStorage = new WorldPublicFactsLogicalStorage(this, logger);
            _inheritanceStorage =  new WorldPublicFactsInheritanceStorage(this, logger);
        }

        private WorldPublicFactsLogicalStorage _logicalStorage;
        private WorldPublicFactsInheritanceStorage _inheritanceStorage;

        /// <inheritdoc/>
        public KindOfStorage Kind => KindOfStorage.WorldPublicFacts;

        /// <inheritdoc/>
        public ILogicalStorage LogicalStorage => _logicalStorage;

        /// <inheritdoc/>
        public IMethodsStorage MethodsStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        public IActionsStorage ActionsStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        public ITriggersStorage TriggersStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        public IInheritanceStorage InheritanceStorage => _inheritanceStorage;

        /// <inheritdoc/>
        public ISynonymsStorage SynonymsStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        public IOperatorsStorage OperatorsStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        public IChannelsStorage ChannelsStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        public IMetadataStorage MetadataStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        public IVarStorage VarStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        public IFuzzyLogicStorage FuzzyLogicStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        public DefaultSettingsOfCodeEntity DefaultSettingsOfCodeEntity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void AddPublicFactsStorageOfOtherGameComponent(IStorage storage)
        {
            _logicalStorage.AddPublicFactsStorageOfOtherGameComponent(storage.LogicalStorage);
            _inheritanceStorage.AddPublicFactsStorageOfOtherGameComponent(storage.InheritanceStorage);
        }

        public void RemovePublicFactsStorageOfOtherGameComponent(IStorage storage)
        {
            _logicalStorage.RemovePublicFactsStorageOfOtherGameComponent(storage.LogicalStorage);
            _inheritanceStorage.RemovePublicFactsStorageOfOtherGameComponent(storage.InheritanceStorage);
        }

        /// <inheritdoc/>
        public void AddParentStorage(IStorage storage)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void RemoveParentStorage(IStorage storage)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        void IStorage.CollectChainOfStorages(IList<StorageUsingOptions> result, IList<IStorage> usedStorages, int level, CollectChainOfStoragesOptions options)
        {
            if (usedStorages.Contains(this))
            {
                return;
            }

            usedStorages.Add(this);

            //if(result.Any(p => p.Storage == this))
            //{
            //    return;
            //}

            level++;

            var item = new StorageUsingOptions()
            {
                Priority = level,
                Storage = this,
                UseFacts = true,
                UseProductions = false,
                UseAdditionalInstances = false
            };

            if (options != null)
            {
                if (options.UseFacts.HasValue)
                {
                    item.UseFacts = options.UseFacts.Value;
                }
            }

            result.Add(item);
        }

        /// <inheritdoc/>
        void IStorage.CollectChainOfStorages(IList<IStorage> result)
        {
            CollectChainOfStorages(result);
        }

        /// <inheritdoc/>
        public IList<IStorage> GetStorages()
        {
            var result = new List<IStorage>();

            CollectChainOfStorages(result);

            return result;
        }

        private void CollectChainOfStorages(IList<IStorage> result)
        {
            if (result.Contains(this))
            {
                return;
            }

#if DEBUG
            //Log($"_kind = {_kind}");
#endif

            result.Add(this);
        }

#if DEBUG
        /// <inheritdoc/>
        public void DbgPrintFactsAndRules()
        {
            Log("Begin");

            _logicalStorage.DbgPrintFactsAndRules();
            //_inheritanceStorage.DbgPrintInheritances();

            Log("End");
        }
#endif

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _logicalStorage.Dispose();
            _inheritanceStorage.Dispose();

            base.OnDisposed();
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string PropertiesToString(uint n)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string PropertiesToShortString(uint n)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string PropertiesToBriefString(uint n)
        {
            throw new NotImplementedException();
        }
    }
}
