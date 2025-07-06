using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Storage.ActionsStoraging;
using SymOntoClay.Core.Internal.Storage.ChannelsStoraging;
using SymOntoClay.Core.Internal.Storage.ConstructorsStoraging;
using SymOntoClay.Core.Internal.Storage.FuzzyLogic;
using SymOntoClay.Core.Internal.Storage.IdleActionItemsStoraging;
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

namespace SymOntoClay.Core.Internal.Htn
{
    public class BuildPlanIterationStorage : BaseComponent, IStorage
    {
        private BuildPlanIterationStorage(IMonitorLogger logger)
            : base(logger)
        {
            _logger = logger;
        }

        public BuildPlanIterationStorage(IMonitorLogger logger, IMainStorageContext mainStorageContext, IStorage parentStorage)
            : base(logger)
        {
            _logger = logger;

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
            _propertyStorage = new BuildPlanIterationPropertyStorage(this, logger);
        }

        private IMonitorLogger _logger;

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
        public DefaultSettingsOfCodeEntity DefaultSettingsOfCodeEntity { get => throw new NotImplementedException("4A2B1219-DFD2-4E7B-AC69-AB3A1882B6EA"); set => throw new NotImplementedException("85116340-8597-4C29-95AE-682646D5B5BF"); }

        /// <inheritdoc/>
        public List<StorageUsingOptions> CodeItemsStoragesList { get; set; }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public BuildPlanIterationStorage Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        public BuildPlanIterationStorage Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (BuildPlanIterationStorage)context[this];
            }

            var result = new BuildPlanIterationStorage(_logger);
            context[this] = result;

            return result;
        }
    }
}
