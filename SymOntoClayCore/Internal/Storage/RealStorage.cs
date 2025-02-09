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
using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
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
using SymOntoClay.Core.Internal.Storage.PropertyStoraging;
using SymOntoClay.Core.Internal.Storage.RelationStoraging;
using SymOntoClay.Core.Internal.Storage.StatesStoraging;
using SymOntoClay.Core.Internal.Storage.SynonymsStoraging;
using SymOntoClay.Core.Internal.Storage.TasksStoraging;
using SymOntoClay.Core.Internal.Storage.TriggersStoraging;
using SymOntoClay.Core.Internal.Storage.VarStoraging;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class RealStorage : BaseLoggedComponent, IStorage
    {
        public RealStorage(KindOfStorage kind, RealStorageSettings settings)
            : base(settings.MainStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = new RealStorageContext();
            _realStorageContext.KindOfGC = settings.KindOfGC;
            _realStorageContext.EnableOnAddingFactEvent = settings.EnableOnAddingFactEvent;
            _realStorageContext.Storage = this;
            _realStorageContext.Logger = settings.MainStorageContext.Logger;
            _realStorageContext.ParentCodeExecutionContext = settings.ParentCodeExecutionContext;
            _realStorageContext.MainStorageContext = settings.MainStorageContext;
            _realStorageContext.InheritancePublicFactsReplicator = settings.InheritancePublicFactsReplicator;
            _realStorageContext.Disabled = !settings.Enabled;

            IsIsolated = settings.IsIsolated;

            DefaultSettingsOfCodeEntity = settings.DefaultSettingsOfCodeEntity;

            var parents = settings.ParentsStorages;

            if(parents == null)
            {
                _realStorageContext.Parents = new List<IStorage>();
            }
            else
            {
                _realStorageContext.Parents = parents;
            }

            var logicalStorage = new LogicalStorage(_kind, _realStorageContext);

            _realStorageContext.LogicalStorage = logicalStorage;
            _logicalStorage = logicalStorage;

            var relationsStorage = new RelationsStorage(_kind, _realStorageContext);

            _realStorageContext.RelationsStorage = relationsStorage;
            _relationsStorage = relationsStorage;

            var methodsStorage = new MethodsStorage(_kind, _realStorageContext);

            _realStorageContext.MethodsStorage = methodsStorage;
            _methodsStorage = methodsStorage;

            var constructorsStorage = new ConstructorsStorage(_kind, _realStorageContext);

            _realStorageContext.ConstructorsStorage = constructorsStorage;
            _constructorsStorage = constructorsStorage;

            var actionsStorage = new ActionsStorage(_kind, _realStorageContext);

            _realStorageContext.ActionsStorage = actionsStorage;
            _actionsStorage = actionsStorage;

            var statesStorage = new StatesStorage(_kind, _realStorageContext);

            _realStorageContext.StatesStorage = statesStorage;
            _statesStorage = statesStorage;

            var triggersStorage = new TriggersStorage(_kind, _realStorageContext);

            _realStorageContext.TriggersStorage = triggersStorage;
            _triggersStorage = triggersStorage;

            var inheritanceStorage = new InheritanceStorage(_kind, _realStorageContext);

            _realStorageContext.InheritanceStorage = inheritanceStorage;
            _inheritanceStorage = inheritanceStorage;

            var synonymsStorage = new SynonymsStorage(_kind, _realStorageContext);

            _realStorageContext.SynonymsStorage = synonymsStorage;
            _synonymsStorage = synonymsStorage;

            var operatorsStorage = new OperatorsStorage(_kind, _realStorageContext);

            _realStorageContext.OperatorsStorage = operatorsStorage;
            _operatorsStorage = operatorsStorage;

            var channelsStorage = new ChannelsStorage(_kind, _realStorageContext);

            _realStorageContext.ChannelsStorage = channelsStorage;
            _channelsStorage = channelsStorage;

            var metadataStorage = new MetadataStorage(_kind, _realStorageContext);

            _realStorageContext.MetadataStorage = metadataStorage;
            _metadataStorage = metadataStorage;

            var varStorage = new VarStorage(_kind, _realStorageContext);

            _realStorageContext.VarStorage = varStorage;
            _varStorage = varStorage;

            var fuzzyLogicStorage = new FuzzyLogicStorage(_kind, _realStorageContext);

            _realStorageContext.FuzzyLogicStorage = fuzzyLogicStorage;
            _fuzzyLogicStorage = fuzzyLogicStorage;

            var idleActionItemsStorage = new IdleActionItemsStorage(_kind, _realStorageContext);

            _realStorageContext.IdleActionItemsStorage = idleActionItemsStorage;
            _idleActionItemsStorage = idleActionItemsStorage;

            var tasksStorage = new TasksStorage(_kind, _realStorageContext);

            _realStorageContext.TasksStorage = tasksStorage;
            _tasksStorage = tasksStorage;

            var propertyStorage = new PropertyStorage(_kind, _realStorageContext);

            _realStorageContext.PropertyStorage = propertyStorage;
            _propertyStorage = propertyStorage;
        }

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        /// <inheritdoc/>
        public virtual StrongIdentifierValue TargetClassName => null;

        /// <inheritdoc/>
        public virtual StrongIdentifierValue InstanceName => null;

        /// <inheritdoc/>
        public virtual IInstance Instance => null;

        /// <inheritdoc/>
        public bool IsIsolated { get; private set; }

        private readonly RealStorageContext _realStorageContext;
        private readonly object _lockObj = new object();

        private readonly LogicalStorage _logicalStorage;
        private readonly RelationsStorage _relationsStorage;
        private readonly MethodsStorage _methodsStorage;
        private readonly ConstructorsStorage _constructorsStorage;
        private readonly ActionsStorage _actionsStorage;
        private readonly StatesStorage _statesStorage;
        private readonly TriggersStorage _triggersStorage;
        private readonly InheritanceStorage _inheritanceStorage;
        private readonly SynonymsStorage _synonymsStorage;
        private readonly OperatorsStorage _operatorsStorage;
        private readonly ChannelsStorage _channelsStorage;
        private readonly MetadataStorage _metadataStorage;
        private readonly VarStorage _varStorage;
        private readonly FuzzyLogicStorage _fuzzyLogicStorage;
        private readonly IdleActionItemsStorage _idleActionItemsStorage;
        private readonly ITasksStorage _tasksStorage;
        private readonly PropertyStorage _propertyStorage;

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
        public IStatesStorage StatesStorage => _statesStorage;

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
        public ITasksStorage TasksStorage => _tasksStorage;

        /// <inheritdoc/>
        public IPropertyStorage PropertyStorage => _propertyStorage;

        public bool Enabled { get => !_realStorageContext.Disabled; set => _realStorageContext.Disabled = !value; }

        /// <inheritdoc/>
        public void AddParentStorage(IMonitorLogger logger, IStorage storage)
        {
            lock (_lockObj)
            {
                var parentsList = _realStorageContext.Parents;

                if(parentsList.Contains(storage))
                {
                    return;
                }
                
                parentsList.Add(storage);

                CodeItemsStoragesList = null;

                storage.OnParentStorageChanged += OnParentStorageChangedHandler;

                _realStorageContext.EmitOnAddParentStorage(logger, storage);         

                OnParentStorageChanged?.Invoke();
            }
        }

        /// <inheritdoc/>
        public void RemoveParentStorage(IMonitorLogger logger, IStorage storage)
        {
            lock (_lockObj)
            {
                var parentsList = _realStorageContext.Parents;

                if (parentsList.Contains(storage))
                {
                    parentsList.Remove(storage);

                    CodeItemsStoragesList = null;

                    storage.OnParentStorageChanged -= OnParentStorageChangedHandler;

                    _realStorageContext.EmitOnRemoveParentStorage(logger, storage);
                    OnParentStorageChanged?.Invoke();
                }
            }
        }

        private void OnParentStorageChangedHandler()
        {
            CodeItemsStoragesList = null;

            OnParentStorageChanged?.Invoke();
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
                UseProductions = true,
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

            lock(_lockObj)
            {
                var parentsList = _realStorageContext.Parents;

                if (parentsList.Any())
                {
                    foreach (var parent in parentsList)
                    {
                        parent.CollectChainOfStorages(logger, result, usedStorages, level, options);
                    }
                }
            }
        }

        /// <inheritdoc/>
        void IStorage.CollectChainOfStorages(IMonitorLogger logger, IList<IStorage> result)
        {
            CollectChainOfStorages(logger, result);
        }

        private void CollectChainOfStorages(IMonitorLogger logger, IList<IStorage> result)
        {
            if(result.Contains(this))
            {
                return;
            }

            result.Add(this);

            lock (_lockObj)
            {
                var parentsList = _realStorageContext.Parents;

                if (parentsList.Any())
                {
                    foreach (var parent in parentsList)
                    {
                        parent.CollectChainOfStorages(logger, result);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public IList<IStorage> GetStorages(IMonitorLogger logger)
        {
            var result = new List<IStorage>();

            CollectChainOfStorages(logger, result);

            return result;
        }

        /// <inheritdoc/>
        public event Action OnParentStorageChanged;

        /// <inheritdoc/>
        public DefaultSettingsOfCodeEntity DefaultSettingsOfCodeEntity { get; set; }

        /// <inheritdoc/>
        public List<StorageUsingOptions> CodeItemsStoragesList { get; set; }

        private bool _isDisposed;

        /// <inheritdoc/>
        public bool IsDisposed
        {
            get
            {
                lock (_lockObj)
                {
                    return _isDisposed;
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_lockObj)
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;
            }

            OnDisposed();
        }

        protected virtual void OnDisposed()
        {
            _realStorageContext.Dispose();
        }

#if DEBUG
        /// <inheritdoc/>
        public void DbgPrintFactsAndRules(IMonitorLogger logger)
        {
            LogicalStorage.DbgPrintFactsAndRules(logger);
        }
#endif

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
            sb.AppendLine($"{spaces}{nameof(Kind)} = {_kind}");
            sb.AppendLine($"{spaces}Owner = {_realStorageContext.MainStorageContext.Id}");
            sb.AppendLine($"{spaces}{nameof(IsIsolated)} = {IsIsolated}");
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
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}HashCode = {GetHashCode()}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {_kind}");
            sb.AppendLine($"{spaces}Owner = {_realStorageContext.MainStorageContext.Id}");
            sb.AppendLine($"{spaces}{nameof(IsIsolated)} = {IsIsolated}");
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
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}HashCode = {GetHashCode()}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {_kind}");
            sb.AppendLine($"{spaces}Owner = {_realStorageContext.MainStorageContext.Id}");
            sb.AppendLine($"{spaces}{nameof(IsIsolated)} = {IsIsolated}");
            return sb.ToString();
        }
    }
}
