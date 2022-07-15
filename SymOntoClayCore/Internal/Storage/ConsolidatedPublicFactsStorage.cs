/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Storage.ActionsStorage;
using SymOntoClay.Core.Internal.Storage.ChannelsStorage;
using SymOntoClay.Core.Internal.Storage.FuzzyLogic;
using SymOntoClay.Core.Internal.Storage.InheritanceStorage;
using SymOntoClay.Core.Internal.Storage.LogicalStorage;
using SymOntoClay.Core.Internal.Storage.MetadataStorage;
using SymOntoClay.Core.Internal.Storage.MethodsStorage;
using SymOntoClay.Core.Internal.Storage.OperatorsStorage;
using SymOntoClay.Core.Internal.Storage.RelationStoraging;
using SymOntoClay.Core.Internal.Storage.StatesStorage;
using SymOntoClay.Core.Internal.Storage.SynonymsStorage;
using SymOntoClay.Core.Internal.Storage.TriggersStorage;
using SymOntoClay.Core.Internal.Storage.VarStorage;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class ConsolidatedPublicFactsStorage: BaseComponent, IStorage
    {
        public ConsolidatedPublicFactsStorage(IEntityLogger logger)
            : this(logger, new ConsolidatedPublicFactsStorageSettings())
        {
        }

        public ConsolidatedPublicFactsStorage(IEntityLogger logger, ConsolidatedPublicFactsStorageSettings settings)
            : base(logger)
        {
            _logicalStorage = new ConsolidatedPublicFactsLogicalStorage(this, logger, settings);
            _inheritanceStorage =  new ConsolidatedPublicFactsInheritanceStorage(this, logger);
            _triggersStorage = new EmptyTriggersStorage(this, logger);
            _varStorage = new EmptyVarStorage(this, logger);
            _statesStorage = new EmptyStatesStorage(this, logger);
            _relationsStorage = new EmptyRelationsStorage(this, logger);
            _methodsStorage = new EmptyMethodsStorage(this, logger);
            _actionsStorage = new EmptyActionsStorage(this, logger);
            _synonymsStorage = new EmptySynonymsStorage(this, logger);
            _operatorsStorage = new EmptyOperatorsStorage(this, logger);
            _channelsStorage = new EmptyChannelsStorage(this, logger);
            _metadataStorage = new EmptyMetadataStorage(this, logger);
            _fuzzyLogicStorage = new EmptyFuzzyLogicStorage(this, logger);
        }

        private ConsolidatedPublicFactsLogicalStorage _logicalStorage;
        private ConsolidatedPublicFactsInheritanceStorage _inheritanceStorage;
        private EmptyTriggersStorage _triggersStorage;
        private EmptyVarStorage _varStorage;
        private EmptyStatesStorage _statesStorage;
        private EmptyRelationsStorage _relationsStorage;
        private EmptyMethodsStorage _methodsStorage;
        private EmptyActionsStorage _actionsStorage;
        private EmptySynonymsStorage _synonymsStorage;
        private EmptyOperatorsStorage _operatorsStorage;
        private EmptyChannelsStorage _channelsStorage;
        private EmptyMetadataStorage _metadataStorage;
        private EmptyFuzzyLogicStorage _fuzzyLogicStorage;

        /// <inheritdoc/>
        public KindOfStorage Kind => KindOfStorage.WorldPublicFacts;

        /// <inheritdoc/>
        public ILogicalStorage LogicalStorage => _logicalStorage;

        /// <inheritdoc/>
        public IRelationsStorage RelationsStorage => _relationsStorage;

        /// <inheritdoc/>
        public IMethodsStorage MethodsStorage => _methodsStorage;

        /// <inheritdoc/>
        public IActionsStorage ActionsStorage => _actionsStorage;

        /// <inheritdoc/>
        IStatesStorage IStorage.StatesStorage => _statesStorage;

        /// <inheritdoc/>
        public ITriggersStorage TriggersStorage => _triggersStorage;

        /// <inheritdoc/>
        public IInheritanceStorage InheritanceStorage => _inheritanceStorage;

        /// <inheritdoc/>
        public ISynonymsStorage SynonymsStorage => _synonymsStorage;

        /// <inheritdoc/>
        public IOperatorsStorage OperatorsStorage => _operatorsStorage;

        /// <inheritdoc/>
        public IChannelsStorage ChannelsStorage => _channelsStorage;

        /// <inheritdoc/>
        public IMetadataStorage MetadataStorage => _metadataStorage;

        /// <inheritdoc/>
        public IVarStorage VarStorage => _varStorage;

        /// <inheritdoc/>
        public IFuzzyLogicStorage FuzzyLogicStorage => _fuzzyLogicStorage;

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
                UseInheritanceFacts = false
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
            _triggersStorage.Dispose();
            _varStorage.Dispose();
            _statesStorage.Dispose();
            _relationsStorage.Dispose();
            _methodsStorage.Dispose();
            _actionsStorage.Dispose();
            _synonymsStorage.Dispose();
            _operatorsStorage.Dispose();
            _channelsStorage.Dispose();
            _metadataStorage.Dispose();
            _fuzzyLogicStorage.Dispose();

            base.OnDisposed();
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return string.Empty;
        }

        /// <inheritdoc/>
        public string PropertiesToString(uint n)
        {
            return string.Empty;
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return string.Empty;
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return string.Empty;
        }

        /// <inheritdoc/>
        public string PropertiesToShortString(uint n)
        {
            return string.Empty;
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return string.Empty;
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return string.Empty;
        }

        /// <inheritdoc/>
        public string PropertiesToBriefString(uint n)
        {
            return string.Empty;
        }
    }
}
