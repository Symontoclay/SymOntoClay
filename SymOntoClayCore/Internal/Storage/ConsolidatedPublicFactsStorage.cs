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

using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Storage.ActionsStoraging;
using SymOntoClay.Core.Internal.Storage.ChannelsStoraging;
using SymOntoClay.Core.Internal.Storage.ConstructorsStoraging;
using SymOntoClay.Core.Internal.Storage.FuzzyLogic;
using SymOntoClay.Core.Internal.Storage.IdleActionItemsStoraging;
using SymOntoClay.Core.Internal.Storage.InheritanceStoraging;
using SymOntoClay.Core.Internal.Storage.LogicalStoraging;
using SymOntoClay.Core.Internal.Storage.MetadataStoraging;
using SymOntoClay.Core.Internal.Storage.MethodsStoraging;
using SymOntoClay.Core.Internal.Storage.OperatorsStoraging;
using SymOntoClay.Core.Internal.Storage.RelationStoraging;
using SymOntoClay.Core.Internal.Storage.StatesStoraging;
using SymOntoClay.Core.Internal.Storage.SynonymsStoraging;
using SymOntoClay.Core.Internal.Storage.TriggersStoraging;
using SymOntoClay.Core.Internal.Storage.VarStoraging;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class ConsolidatedPublicFactsStorage: BaseComponent, IStorage
    {
        public ConsolidatedPublicFactsStorage(IMonitorLogger logger, KindOfStorage kind)
            : this(logger, kind, new ConsolidatedPublicFactsStorageSettings())
        {
        }

        public ConsolidatedPublicFactsStorage(IMonitorLogger logger, KindOfStorage kind, ConsolidatedPublicFactsStorageSettings settings)
            : base(logger)
        {
            _kind = kind;

            _logicalStorage = new ConsolidatedPublicFactsLogicalStorage(this, logger, kind, settings);
            _inheritanceStorage =  new ConsolidatedPublicFactsInheritanceStorage(this, logger, kind);
            _triggersStorage = new EmptyTriggersStorage(this, logger);
            _varStorage = new EmptyVarStorage(this, logger);
            _statesStorage = new EmptyStatesStorage(this, logger);
            _relationsStorage = new EmptyRelationsStorage(this, logger);
            _methodsStorage = new EmptyMethodsStorage(this, logger);
            _constructorsStorage = new EmptyConstructorsStorage(this, logger);
            _actionsStorage = new EmptyActionsStorage(this, logger);
            _synonymsStorage = new EmptySynonymsStorage(this, logger);
            _operatorsStorage = new EmptyOperatorsStorage(this, logger);
            _channelsStorage = new EmptyChannelsStorage(this, logger);
            _metadataStorage = new EmptyMetadataStorage(this, logger);
            _fuzzyLogicStorage = new EmptyFuzzyLogicStorage(this, logger);
            _idleActionItemsStorage = new EmptyIdleActionItemsStorage(this, logger);
        }

        private readonly object _lockObj = new object();
        private readonly List<IStorage> _storages = new List<IStorage>();
        private ConsolidatedPublicFactsLogicalStorage _logicalStorage;
        private ConsolidatedPublicFactsInheritanceStorage _inheritanceStorage;
        private EmptyTriggersStorage _triggersStorage;
        private EmptyVarStorage _varStorage;
        private EmptyStatesStorage _statesStorage;
        private EmptyRelationsStorage _relationsStorage;
        private EmptyMethodsStorage _methodsStorage;
        private EmptyConstructorsStorage _constructorsStorage;
        private EmptyActionsStorage _actionsStorage;
        private EmptySynonymsStorage _synonymsStorage;
        private EmptyOperatorsStorage _operatorsStorage;
        private EmptyChannelsStorage _channelsStorage;
        private EmptyMetadataStorage _metadataStorage;
        private EmptyFuzzyLogicStorage _fuzzyLogicStorage;
        private EmptyIdleActionItemsStorage _idleActionItemsStorage;

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        /// <inheritdoc/>
        public StrongIdentifierValue TargetClassName => null;

        /// <inheritdoc/>
        public StrongIdentifierValue InstanceName => null;

        /// <inheritdoc/>
        public IInstance Instance => null;

        /// <inheritdoc/>
        public bool IsIsolated => false;

        /// <inheritdoc/>
        public ILogicalStorage LogicalStorage => _logicalStorage;

        /// <inheritdoc/>
        public IRelationsStorage RelationsStorage => _relationsStorage;

        /// <inheritdoc/>
        public IMethodsStorage MethodsStorage => _methodsStorage;

        /// <inheritdoc/>
        public IConstructorsStorage ConstructorsStorage => _constructorsStorage;

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
        public IIdleActionItemsStorage IdleActionItemsStorage => _idleActionItemsStorage;

        /// <inheritdoc/>
        public DefaultSettingsOfCodeEntity DefaultSettingsOfCodeEntity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public List<StorageUsingOptions> CodeItemsStoragesList { get; set; }

        public void AddConsolidatedStorage(IMonitorLogger logger, IStorage storage)
        {
            lock(_lockObj)
            {
                if (_storages.Contains(storage))
                {
                    return;
                }

                _storages.Add(storage);

                _logicalStorage.AddConsolidatedStorage(logger, storage.LogicalStorage);
                _inheritanceStorage.AddConsolidatedStorage(logger, storage.InheritanceStorage);
            }
        }
        
        public void RemoveConsolidatedStorage(IMonitorLogger logger, IStorage storage)
        {
            lock (_lockObj)
            {
                if (!_storages.Contains(storage))
                {
                    return;
                }

                _storages.Remove(storage);

                _logicalStorage.RemoveConsolidatedStorage(logger, storage.LogicalStorage);
                _inheritanceStorage.RemoveConsolidatedStorage(logger, storage.InheritanceStorage);
            }
        }

        /// <inheritdoc/>
        public void AddParentStorage(IMonitorLogger logger, IStorage storage)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void RemoveParentStorage(IMonitorLogger logger, IStorage storage)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        void IStorage.CollectChainOfStorages(IMonitorLogger logger, IList<StorageUsingOptions> result, IList<IStorage> usedStorages, int level, CollectChainOfStoragesOptions options)
        {
            if (usedStorages.Contains(this))
            {
                return;
            }

            usedStorages.Add(this);

            level++;

            var item = new StorageUsingOptions()
            {
                Priority = level,
                Storage = this,
                UseFacts = true,
                UseProductions = false,
                UseInheritanceFacts = true
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
        void IStorage.CollectChainOfStorages(IMonitorLogger logger, IList<IStorage> result)
        {
            CollectChainOfStorages(logger, result);
        }

        /// <inheritdoc/>
        public IList<IStorage> GetStorages(IMonitorLogger logger)
        {
            var result = new List<IStorage>();

            CollectChainOfStorages(logger, result);

            return result;
        }

        private void CollectChainOfStorages(IMonitorLogger logger, IList<IStorage> result)
        {
            if (result.Contains(this))
            {
                return;
            }

            result.Add(this);
        }

        /// <inheritdoc/>
        public event Action OnParentStorageChanged;

#if DEBUG
        /// <inheritdoc/>
        public void DbgPrintFactsAndRules(IMonitorLogger logger)
        {
            logger.Info("82B887B0-0CBB-43C7-94FC-A993862D5FE1", "Begin");

            _logicalStorage.DbgPrintFactsAndRules(logger);

            logger.Info("452B0BC5-2F78-469C-9A6B-12C89F770A7E", "End");
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
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}HashCode = {GetHashCode()}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.PrintObjListProp(n, "Storages", _storages);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        public string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}HashCode = {GetHashCode()}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.PrintShortObjListProp(n, "Storages", _storages);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        public string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}HashCode = {GetHashCode()}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.PrintBriefObjListProp(n, "Storages", _storages);

            return sb.ToString();
        }
    }
}
