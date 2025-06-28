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

using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Common;
using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Instances.InternalRunners;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.Instances
{
    public abstract class BaseInstance : BaseComponent, IInstance,
        IObjectToString, IObjectToShortString, IObjectToBriefString,
        IOnFinishedExecutionCoordinatorHandler
    {
        protected BaseInstance(CodeItem codeItem, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext, 
            IExecutionCoordinator parentExecutionCoordinator, IStorageFactory storageFactory, List<VarInstance> varList)
            : base(context.Logger)
        {
            _codeItem = codeItem;

            Name = codeItem.Name;
            _context = context;
            _parentStorage = parentStorage;

            _activeObjectContext = context.ActiveObjectContext;
            _threadPool = context.AsyncEventsThreadPool;
            _serializationAnchor = new SerializationAnchor();

            var dataResolversFactory = context.DataResolversFactory;

            _triggersResolver = dataResolversFactory.GetTriggersResolver();
            _constructorsResolver = dataResolversFactory.GetConstructorsResolver();
            _inheritanceResolver = dataResolversFactory.GetInheritanceResolver();

            _globalTriggersStorage = context.Storage.GlobalStorage.TriggersStorage;

            _executionCoordinator = new ExecutionCoordinator(this);
            _executionCoordinator.AddOnFinishedHandler(this);

            _baseInstanceParentExecutionCoordinatorOnFinishedHandler = new BaseInstanceParentExecutionCoordinatorOnFinishedHandler(Logger, _executionCoordinator, _activeObjectContext, _threadPool, _serializationAnchor);

            _parentExecutionCoordinator = parentExecutionCoordinator;

            var isIsolated = codeItem.ParentCodeEntity == null;

            var localCodeExecutionContext = new LocalCodeExecutionContext(parentCodeExecutionContext)
            {
                IsIsolated = isIsolated,
                Instance = this
            };

            var localStorageSettings = RealStorageSettingsHelper.Create(context, parentStorage);

            localStorageSettings.IsIsolated = isIsolated;

            _storage = storageFactory.CreateStorage(localStorageSettings);

            localCodeExecutionContext.Storage = _storage;
            localCodeExecutionContext.Holder = Name;

            _localCodeExecutionContext = localCodeExecutionContext;
            RebuildSuperClassesStorages(Logger);

            if (!varList.IsNullOrEmpty())
            {
                var varStorage = _storage.VarStorage;

                foreach(var varItem in varList)
                {
                    varStorage.Append(Logger, varItem);
                }
            }
        }

        /// <inheritdoc/>
        public abstract KindOfInstance KindOfInstance { get; }

        protected readonly CodeItem _codeItem;

        /// <inheritdoc/>
        public virtual CodeItem CodeItem => _codeItem;

        /// <inheritdoc/>
        public StrongIdentifierValue Name { get; private set; }

        /// <inheritdoc/>
        public virtual float Priority => 0f;

        private IActiveObjectContext _activeObjectContext;
        private SerializationAnchor _serializationAnchor;
        protected ICustomThreadPool _threadPool;

        protected readonly IEngineContext _context;

        /// <inheritdoc/>
        public IEngineContext EngineContext => _context;

        private readonly ITriggersStorage _globalTriggersStorage;
        private readonly IStorage _parentStorage;
        protected readonly IStorage _storage;
        protected readonly ILocalCodeExecutionContext _localCodeExecutionContext;
        private readonly IExecutionCoordinator _parentExecutionCoordinator;

        private readonly TriggersResolver _triggersResolver;
        private readonly ConstructorsResolver _constructorsResolver;
        private readonly InheritanceResolver _inheritanceResolver;

        private InstanceState _instanceState = InstanceState.Created;
        private List<LogicConditionalTriggerInstance> _logicConditionalTriggersList = new List<LogicConditionalTriggerInstance>();
        private List<AddingFactNonConditionalTriggerInstance> _addingFactNonConditionalTriggerInstancesList = new List<AddingFactNonConditionalTriggerInstance>();
        private List<AddingFactConditionalTriggerInstance> _addingFactConditionalTriggerInstancesList = new List<AddingFactConditionalTriggerInstance>();

        protected IExecutionCoordinator _executionCoordinator;

        private BaseInstanceParentExecutionCoordinatorOnFinishedHandler _baseInstanceParentExecutionCoordinatorOnFinishedHandler;

        private List<IInstance> _childInstances = new List<IInstance>();
        private IInstance _parentInstance;
        private readonly object _childInstancesLockObj = new object();
      
        private readonly Dictionary<StrongIdentifierValue, IStorage> _superClassesStorages = new Dictionary<StrongIdentifierValue, IStorage>();
        private readonly object _superClassesStoragesLockObj = new object();

        /// <inheritdoc/>
        public IExecutionCoordinator ExecutionCoordinator => _executionCoordinator;

        /// <inheritdoc/>
        public ILocalCodeExecutionContext LocalCodeExecutionContext => _localCodeExecutionContext;

        /// <inheritdoc/>
        public void CancelExecution(IMonitorLogger logger, string messagePointId, ReasonOfChangeStatus reasonOfChangeStatus, Changer changer = null, string callMethodId = "")
        {
            CancelExecution(logger, messagePointId, reasonOfChangeStatus, changer == null ? null : new List<Changer> { changer }, callMethodId);
        }

        /// <inheritdoc/>
        public void CancelExecution(IMonitorLogger logger, string messagePointId, ReasonOfChangeStatus reasonOfChangeStatus, List<Changer> changers, string callMethodId = "")
        {
            logger.CancelInstanceExecution(messagePointId, Name.ToHumanizedLabel(), reasonOfChangeStatus, changers, callMethodId);

            _executionCoordinator.SetExecutionStatus(logger, "64A3F029-7F5D-4DB7-9ECC-83DD59A2973C", ActionExecutionStatus.Canceled);
        }
        
        public virtual void Init(IMonitorLogger logger)
        {
            if (_parentExecutionCoordinator != null)
            {
                if(_parentExecutionCoordinator.IsFinished)
                {
                    _executionCoordinator.SetExecutionStatus(logger, "710212D6-2705-4C71-A44A-780ECAA3B9C5", ActionExecutionStatus.WeakCanceled);
                    _instanceState = InstanceState.Initialized;
                    return;
                }

                _parentExecutionCoordinator.AddOnFinishedHandler(_baseInstanceParentExecutionCoordinatorOnFinishedHandler);
            }

            _instanceState = InstanceState.Initializing;

            _executionCoordinator.SetExecutionStatus(logger, "5ED62258-1580-4877-A8A0-1DDCBF7FF05A", ActionExecutionStatus.Executing);

            ApplyCodeDirectives(logger);

            RunPreConstructors(logger);
            RunConstructors(logger);

            RunInitialTriggers(logger);

            RunMutuallyExclusiveStatesSets(logger);

            RunExplicitStates(logger);

            RunActivatorsOfStates(logger);
            RunDeactivatorsOfStates(logger);

            var targetAddingFactTriggersList = _triggersResolver.ResolveAddFactTriggersList(logger, Name, _localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

            if(targetAddingFactTriggersList.Any())
            {
                foreach(var targetTrigger in targetAddingFactTriggersList)
                {
                    if(targetTrigger.SetCondition == null)
                    {
                        var triggerInstance = new AddingFactNonConditionalTriggerInstance(targetTrigger, this, _context, _storage, _localCodeExecutionContext);
                        triggerInstance.Init();
                        _addingFactNonConditionalTriggerInstancesList.Add(triggerInstance);
                    }
                    else
                    {
                        var triggerInstance = new AddingFactConditionalTriggerInstance(targetTrigger, this, _context, _storage, _localCodeExecutionContext);
                        triggerInstance.Init();
                        _addingFactConditionalTriggerInstancesList.Add(triggerInstance);
                    }
                }
            }

            var targetLogicConditionalTriggersList = _triggersResolver.ResolveLogicConditionalTriggersList(logger, Name, _localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

            if (targetLogicConditionalTriggersList.Any())
            {
                foreach (var targetTrigger in targetLogicConditionalTriggersList)
                {
                    var triggerInstance = new LogicConditionalTriggerInstance(targetTrigger, this, _context, _storage, _localCodeExecutionContext);
                    _logicConditionalTriggersList.Add(triggerInstance);

                    _globalTriggersStorage.Append(logger, triggerInstance);

                    triggerInstance.Init(logger);
                }
            }

            _instanceState = InstanceState.Initialized;            
        }

        private void RebuildSuperClassesStorages(IMonitorLogger logger)
        {
            var superClassesList = _inheritanceResolver.GetSuperClassesKeysList(logger, Name, _localCodeExecutionContext);

            lock(_superClassesStoragesLockObj)
            {
                if (superClassesList.Any())
                {
                    var existingKeys = _superClassesStorages.Keys.ToList();

                    var keysForAdding = superClassesList.Except(existingKeys);

                    var keysForRemoving = existingKeys.Except(superClassesList);

                    if(keysForAdding.Any())
                    {

                        foreach (var key in keysForAdding)
                        {
                            var localStorageSettings = RealStorageSettingsHelper.Create(_context, _parentStorage);

                            var storage = new SuperClassStorage(localStorageSettings, key, this);

                            _superClassesStorages[key] = storage;

                            _storage.AddParentStorage(logger, storage);
                        }
                    }

                    foreach(var key in keysForRemoving)
                    {
                        throw new NotImplementedException("F2C86013-C785-40B1-96B0-C93C48EE3FBE");
                    }
                }
                else
                {
                    if(_superClassesStorages.Count > 0)
                    {
                        throw new NotImplementedException("B2129DE0-A245-49CC-865A-A9DBEE77E2B2");
                    }                    
                }
            }
        }

        /// <inheritdoc/>
        public virtual IList<IInstance> GetTopIndependentInstances(IMonitorLogger logger)
        {
            throw new NotImplementedException("7B2BBE1F-CABE-4DF8-9497-ECF8BA9C2026");
        }

        /// <inheritdoc/>
        public virtual bool ActivateIdleAction(IMonitorLogger logger)
        {
            throw new NotImplementedException("57E4373C-1824-4034-824A-C2990B10E22B");
        }

        /// <inheritdoc/>
        public void AddChildInstance(IMonitorLogger logger, IInstance instance)
        {
            lock(_childInstancesLockObj)
            {
                if (_childInstances.Contains(instance))
                {
                    return;
                }

                _childInstances.Add(instance);

                instance.SetParent(logger, this);
            }
        }

        /// <inheritdoc/>
        public void RemoveChildInstance(IMonitorLogger logger, IInstance instance)
        {
            lock (_childInstancesLockObj)
            {
                if (!_childInstances.Contains(instance))
                {
                    return;
                }

                _childInstances.Remove(instance);

                instance.ResetParent(logger, this);
            }
        }

        /// <inheritdoc/>
        public void SetParent(IMonitorLogger logger, IInstance instance)
        {
            lock (_childInstancesLockObj)
            {
                if (_parentInstance == instance)
                {
                    return;
                }

                _parentInstance = instance;

                instance.AddChildInstance(logger, this);
            }
        }

        /// <inheritdoc/>
        public void ResetParent(IMonitorLogger logger, IInstance instance)
        {
            lock (_childInstancesLockObj)
            {
                if (_parentInstance != instance)
                {
                    return;
                }

                _parentInstance = null;

                instance.RemoveChildInstance(logger, this);
            }
        }

        protected virtual void ApplyCodeDirectives(IMonitorLogger logger)
        {
        }

        protected virtual void RunInitialTriggers(IMonitorLogger logger)
        {
            RunLifecycleTriggers(logger, KindOfSystemEventOfInlineTrigger.Enter);
        }

        protected virtual void RunPreConstructors(IMonitorLogger logger)
        {
            var preConstructors = _constructorsResolver.ResolvePreConstructors(logger, Name, _localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

            if (preConstructors.Any())
            {
                var superClassesStoragesDict = _constructorsResolver.GetSuperClassStoragesDict(logger, _localCodeExecutionContext.Storage, this);

                var processInitialInfoList = new List<ProcessInitialInfo>();

                foreach (var preConstructor in preConstructors)
                {
                    var targetHolder = preConstructor.Holder;

                    var targetStorage = superClassesStoragesDict[targetHolder];

                    var localCodeExecutionContext = new LocalCodeExecutionContext(_localCodeExecutionContext);
                    localCodeExecutionContext.Storage = targetStorage;
                    localCodeExecutionContext.Holder = targetHolder;
                    localCodeExecutionContext.Instance = this;
                    localCodeExecutionContext.Owner = targetHolder;
                    localCodeExecutionContext.OwnerStorage= targetStorage;                    
                    localCodeExecutionContext.Kind = KindOfLocalCodeExecutionContext.PreConstructor;

                    var processInitialInfo = new ProcessInitialInfo();
                    processInitialInfo.CompiledFunctionBody = preConstructor.CompiledFunctionBody;
                    processInitialInfo.LocalContext = localCodeExecutionContext;
                    processInitialInfo.Metadata = preConstructor;
                    processInitialInfo.Instance = this;
                    processInitialInfo.ExecutionCoordinator = _executionCoordinator;

                    processInitialInfoList.Add(processInitialInfo);
                }

                var threadExecutor = _context.CodeExecutor.ExecuteBatchSync(logger, processInitialInfoList);

            }
        }

        protected virtual void RunConstructors(IMonitorLogger logger)
        {
            var constructorsResolvingResultList = _constructorsResolver.ResolveListWithSelfAndDirectInheritance(logger, Name, _localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

            if (constructorsResolvingResultList.Any())
            {
                var superClassesStoragesDict = _constructorsResolver.GetSuperClassStoragesDict(logger, _localCodeExecutionContext.Storage, this);

                var processInitialInfoList = new List<ProcessInitialInfo>();

                foreach (var constructorResolvingResult in constructorsResolvingResultList)
                {
                    var constructor = constructorResolvingResult.Constructor;

                    var targetHolder = constructor.Holder;

                    var targetStorage = superClassesStoragesDict[targetHolder];

                    var localCodeExecutionContext = new LocalCodeExecutionContext(_localCodeExecutionContext);

                    var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
                    localCodeExecutionContext.Storage = new LocalStorage(localStorageSettings);
                    localCodeExecutionContext.Holder = Name;
                    localCodeExecutionContext.Instance = this;
                    localCodeExecutionContext.Owner = targetHolder;
                    localCodeExecutionContext.OwnerStorage = targetStorage;

                    var processInitialInfo = new ProcessInitialInfo();
                    processInitialInfo.CompiledFunctionBody = constructor.CompiledFunctionBody;
                    processInitialInfo.LocalContext = localCodeExecutionContext;
                    processInitialInfo.Metadata = constructor;
                    processInitialInfo.Instance = this;
                    processInitialInfo.ExecutionCoordinator = _executionCoordinator;

                    processInitialInfoList.Add(processInitialInfo);
                }

                var threadExecutor = _context.CodeExecutor.ExecuteBatchSync(logger, processInitialInfoList);

            }
        }

        protected void RunLifecycleTriggers(IMonitorLogger logger, KindOfSystemEventOfInlineTrigger kindOfSystemEvent)
        {
            RunLifecycleTriggers(logger, kindOfSystemEvent, Name);
        }

        protected void RunLifecycleTriggers(IMonitorLogger logger, KindOfSystemEventOfInlineTrigger kindOfSystemEvent, IExecutionCoordinator executionCoordinator, bool normalOrder = true)
        {
            RunLifecycleTriggers(logger, kindOfSystemEvent, Name, executionCoordinator, normalOrder);
        }

        protected void RunLifecycleTriggers(IMonitorLogger logger, KindOfSystemEventOfInlineTrigger kindOfSystemEvent, StrongIdentifierValue holder)
        {
            RunLifecycleTriggers(logger, kindOfSystemEvent, holder, _executionCoordinator);
        }

        protected void RunLifecycleTriggers(IMonitorLogger logger, KindOfSystemEventOfInlineTrigger kindOfSystemEvent, StrongIdentifierValue holder,
            IExecutionCoordinator executionCoordinator, bool normalOrder = true)
        {
#if DEBUG
            logger.Info("025CA7CC-B1BA-4BD5-802A-B6F94E138F55", $"kindOfSystemEvent = {kindOfSystemEvent}");
            logger.Info("40735ECF-A7D4-4F87-9756-F90A6FCE2856", $"holder = {holder}");
            logger.Info("B8A1FBCD-50B3-4CD0-A817-07323E59E929", $"Name = {Name}");
#endif

#if DEBUG
            var tmpRunner = new BaseLifecycleTriggersRunner(Logger, _context, this, holder, _localCodeExecutionContext, executionCoordinator, _storage, kindOfSystemEvent, normalOrder, true);
            tmpRunner.RunAsync(logger);
#endif

            /*var targetSystemEventsTriggersList = _triggersResolver.ResolveSystemEventsTriggersList(logger, kindOfSystemEvent, holder, _localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

            if (targetSystemEventsTriggersList.Any())
            {
                if(normalOrder)
                {
                    targetSystemEventsTriggersList.Reverse();
                }

                logger.RunLifecycleTrigger("D3922E27-5527-44D0-BDB6-0F64632555E0", Name?.ToHumanizedLabel(), holder?.ToHumanizedLabel(), kindOfSystemEvent);

                var processInitialInfoList = new List<ProcessInitialInfo>();

                foreach (var targetTrigger in targetSystemEventsTriggersList)
                {
#if DEBUG
                    logger.Info("F67C5A2A-2397-403A-BE32-175BB091FF37", $"targetTrigger.KindOfInlineTrigger = {targetTrigger.KindOfInlineTrigger}");
                    logger.Info("1153D67C-BC9E-4DA9-9AA8-484E26472D27", $"targetTrigger.KindOfSystemEvent = {targetTrigger.KindOfSystemEvent}");
                    logger.Info("651D3594-EB5A-4E8F-8D33-A0B70A9E9E5F", $"{nameof(targetTrigger)}.ToHumanizedLabel() = {targetTrigger.ToHumanizedLabel()}");
                    logger.Info("60696ED7-12B3-4298-B6BC-D1AD220324A0", $"{nameof(targetTrigger)}.ToHumanizedString() = {targetTrigger.ToHumanizedString()}");
                    logger.Info("F940CFA1-FC58-46A4-824B-C425D313FDC4", $"{nameof(targetTrigger)}.ToLabel(logger) = {targetTrigger.ToLabel(logger)}");
#endif
                    var localCodeExecutionContext = new LocalCodeExecutionContext(_localCodeExecutionContext);

                    var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
                    localCodeExecutionContext.Storage = new LocalStorage(localStorageSettings);
                    localCodeExecutionContext.Holder = holder;
                    localCodeExecutionContext.Instance = this;

                    var processInitialInfo = new ProcessInitialInfo();
                    processInitialInfo.CompiledFunctionBody = targetTrigger.SetCompiledFunctionBody;
                    processInitialInfo.LocalContext = localCodeExecutionContext;
                    processInitialInfo.Metadata = targetTrigger;
                    processInitialInfo.Instance = this;
                    processInitialInfo.ExecutionCoordinator = executionCoordinator;

                    processInitialInfoList.Add(processInitialInfo);
                }

                var threadExecutor = _context.CodeExecutor.ExecuteBatchAsync(logger, processInitialInfoList, logger.Id);
            }*/
        }

        protected virtual void RunMutuallyExclusiveStatesSets(IMonitorLogger logger)
        {
        }

        protected virtual void RunExplicitStates(IMonitorLogger logger)
        {
        }

        protected virtual void RunActivatorsOfStates(IMonitorLogger logger)
        {
        }

        protected virtual void RunDeactivatorsOfStates(IMonitorLogger logger)
        {
        }

        protected virtual void RunFinalizationTriggers(IMonitorLogger logger)
        {
            var finalizationExecutionCoordinator = new ExecutionCoordinator(this);
            finalizationExecutionCoordinator.SetExecutionStatus(logger, "86865B32-9839-4442-8EEF-5522E6DAADB4", ActionExecutionStatus.Executing);

            RunLifecycleTriggers(logger, KindOfSystemEventOfInlineTrigger.Leave, finalizationExecutionCoordinator, false);
        }

        void IOnFinishedExecutionCoordinatorHandler.Invoke()
        {
            LoggedFunctorWithoutResult<BaseInstance>.Run(Logger, "F79DC0DC-545C-4664-8530-71CBC0B20E77", this,
                (IMonitorLogger loggerValue, BaseInstance instanceValue) => {
                    instanceValue.ExecutionCoordinator_OnFinished();
                },
                _activeObjectContext, _threadPool, _serializationAnchor);
        }

        public virtual void ExecutionCoordinator_OnFinished()
        {
            if (_parentInstance != null)
            {
                _parentInstance.RemoveChildInstance(Logger, this);
                _parentInstance = null;
            }

            RunFinalizationTriggers(Logger);

            if(_childInstances.Any())
            {
                var changer = new Changer(KindOfChanger.Instance, Name?.ToHumanizedLabel());

                foreach(var childInstance in _childInstances.ToList())
                {
                    childInstance.CancelExecution(Logger, "D39D5646-20AC-43FB-95A5-90125E0F2DFB", ReasonOfChangeStatus.ByParentInstance, changer);
                }
            }

            Dispose();
        }

        /// <inheritdoc/>
        public virtual IExecutable GetExecutable(IMonitorLogger logger, KindOfFunctionParameters kindOfParameters, IDictionary<StrongIdentifierValue, Value> namedParameters, IList<Value> positionedParameters)
        {
            throw new NotImplementedException("51ED45A6-37FF-42A8-AEFE-6835C71D404F");
        }

        /// <inheritdoc/>
        public virtual void SetPropertyValue(IMonitorLogger logger, StrongIdentifierValue propertyName, Value value)
        {
            throw new NotImplementedException("BAC477C7-7E75-4636-9860-645189734004");
        }

        /// <inheritdoc/>
        public virtual void SetVarValue(IMonitorLogger logger, StrongIdentifierValue varName, Value value)
        {
            throw new NotImplementedException("D22BB4AA-41CE-4A77-B1DD-138E9C74229F");
        }

        /// <inheritdoc/>
        public virtual Value GetPropertyValue(IMonitorLogger logger, StrongIdentifierValue propertyName)
        {
            throw new NotImplementedException("F7A9893C-4C76-4920-8EEF-744B43CFB7B6");
        }

        /// <inheritdoc/>
        public virtual Value GetVarValue(IMonitorLogger logger, StrongIdentifierValue varName)
        {
            throw new NotImplementedException("4BB83DBC-6514-4EDE-9B77-F023715DE7B8");
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            if(_parentExecutionCoordinator != null)
            {
                _parentExecutionCoordinator.RemoveOnFinishedHandler(_baseInstanceParentExecutionCoordinatorOnFinishedHandler);
            }

            _serializationAnchor.Dispose();

            foreach (var triggerInstance in _logicConditionalTriggersList)
            {
                _globalTriggersStorage.Remove(Logger, triggerInstance);

                triggerInstance.Dispose();
            }

            _logicConditionalTriggersList.Clear();

            foreach (var triggerInstance in _addingFactNonConditionalTriggerInstancesList)
            {
                triggerInstance.Dispose();
            }

            _addingFactNonConditionalTriggerInstancesList.Clear();

            foreach (var triggerInstance in _addingFactConditionalTriggerInstancesList)
            {
                triggerInstance.Dispose();
            }

            _addingFactConditionalTriggerInstancesList.Clear();

            foreach (var childInstance in _childInstances)
            {
                childInstance.Dispose();
            }

            _childInstances.Clear();

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
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfInstance)} = {KindOfInstance}");
            sb.PrintObjProp(n, nameof(Name), Name);
            sb.AppendLine($"{spaces}{nameof(_instanceState)} = {_instanceState}");

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
            return PropertiesToShortString(n);
        }

        protected virtual string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfInstance)} = {KindOfInstance}");
            sb.PrintShortObjProp(n, nameof(Name), Name);
            sb.AppendLine($"{spaces}{nameof(_instanceState)} = {_instanceState}");

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
            return PropertiesToBriefString(n);
        }

        protected virtual string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfInstance)} = {KindOfInstance}");
            sb.PrintBriefObjProp(n, nameof(Name), Name);
            sb.AppendLine($"{spaces}{nameof(_instanceState)} = {_instanceState}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToHumanizedString(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            return ToHumanizedString(DebugHelperOptions.FromHumanizedOptions(options));
        }

        /// <inheritdoc/>
        public string ToHumanizedString(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public string ToHumanizedLabel(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            return ToHumanizedLabel(DebugHelperOptions.FromHumanizedOptions(options));
        }

        /// <inheritdoc/>
        public string ToHumanizedLabel(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public string ToHumanizedString(IMonitorLogger logger)
        {
            return NToHumanizedString();
        }

        private string NToHumanizedString()
        {
            return $"ref: {Name.NameValue}";
        }

        /// <inheritdoc/>
        public MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = NToHumanizedString()
            };
        }
    }
}
