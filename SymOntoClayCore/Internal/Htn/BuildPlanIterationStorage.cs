/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using SymOntoClay.Core.EventsInterfaces;
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
using SymOntoClay.Core.Internal.Storage.TasksStoraging;
using SymOntoClay.Core.Internal.Storage.TriggersStoraging;
using SymOntoClay.Core.Internal.Storage.VarStoraging;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Htn
{
    public class BuildPlanIterationStorage : BaseComponent, IStorage
    {
        private BuildPlanIterationStorage(IMonitorLogger logger)
            : base(logger)
        {
            CreateEmptyStorages(logger);
        }

        public BuildPlanIterationStorage(IMonitorLogger logger, IMainStorageContext mainStorageContext, IStorage parentStorage)
            : base(logger)
        {
            _parentStorage = parentStorage;

            CreateEmptyStorages(logger);

            _propertyStorage = new BuildPlanIterationPropertyStorage(this, logger);
        }
        
        private void CreateEmptyStorages(IMonitorLogger logger)
        {
            _logicalStorage = new EmptyLogicalStorage(this, logger);
            _inheritanceStorage = new EmptyInheritanceStorage(this, logger);
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
            _tasksStorage = new EmptyTasksStorage(this, logger);
        }

        private IStorage _parentStorage;

        private EmptyLogicalStorage _logicalStorage;
        private EmptyInheritanceStorage _inheritanceStorage;
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
        private EmptyTasksStorage _tasksStorage;
        private BuildPlanIterationPropertyStorage _propertyStorage;

        /// <inheritdoc/>
        public KindOfStorage Kind => KindOfStorage.HtnBuildPlanIterationStorage;

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
        public ITasksStorage TasksStorage => _tasksStorage;

        /// <inheritdoc/>
        public IPropertyStorage PropertyStorage => _propertyStorage;

        /// <inheritdoc/>
        public DefaultSettingsOfCodeEntity DefaultSettingsOfCodeEntity { get => _parentStorage.DefaultSettingsOfCodeEntity; set => throw new NotImplementedException("85116340-8597-4C29-95AE-682646D5B5BF"); }

        /// <inheritdoc/>
        public List<StorageUsingOptions> CodeItemsStoragesList { get; set; }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public BuildPlanIterationStorage Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public BuildPlanIterationStorage Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (BuildPlanIterationStorage)context[this];
            }

            var result = new BuildPlanIterationStorage(Logger);
            context[this] = result;

            result._parentStorage = _parentStorage;
            result._propertyStorage = _propertyStorage.Clone(context);

            return result;
        }

        /// <inheritdoc/>
        public void AddParentStorage(IMonitorLogger logger, IStorage storage)
        {
            throw new NotImplementedException("60D8BC4D-7704-4029-849D-AB8A51FFC903");
        }

        /// <inheritdoc/>
        public void RemoveParentStorage(IMonitorLogger logger, IStorage storage)
        {
            throw new NotImplementedException("B939F0D9-29D2-4CEC-8504-C6948D79B326");
        }

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

            result.Add(item);

            _parentStorage.CollectChainOfStorages(logger, result, usedStorages, level, options);
        }

        /// <inheritdoc/>
        void IStorage.CollectChainOfStorages(IMonitorLogger logger, IList<IStorage> result)
        {
            CollectChainOfStorages(logger, result);
        }

        private void CollectChainOfStorages(IMonitorLogger logger, IList<IStorage> result)
        {
            if (result.Contains(this))
            {
                return;
            }

            result.Add(this);

            _parentStorage.CollectChainOfStorages(logger, result);
        }

        /// <inheritdoc/>
        public IList<IStorage> GetStorages(IMonitorLogger logger)
        {
            var result = new List<IStorage>();

            CollectChainOfStorages(logger, result);

            return result;
        }

        void IStorage.AddOnParentStorageChangedHandler(IOnParentStorageChangedStorageHandler handler)
        {
        }

        void IStorage.RemoveOnParentStorageChangedHandler(IOnParentStorageChangedStorageHandler handler)
        {
        }

        /// <inheritdoc/>
        public string FactsAndRulesToDbgString()
        {
            return string.Empty;
        }

#if DEBUG
        /// <inheritdoc/>
        public void DbgPrintFactsAndRules(IMonitorLogger logger)
        {
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

            return sb.ToString();
        }
    }
}
