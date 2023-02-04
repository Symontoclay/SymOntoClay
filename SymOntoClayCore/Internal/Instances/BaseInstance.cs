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

using Newtonsoft.Json.Linq;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Instances
{
    public abstract class BaseInstance : BaseComponent, IInstance, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        protected BaseInstance(CodeItem codeItem, IEngineContext context, IStorage parentStorage, LocalCodeExecutionContext parentCodeExecutionContext, IStorageFactory storageFactory, List<Var> varList)
            : base(context.Logger)
        {
#if DEBUG
            //Log($"parentStorage = {parentStorage}");
#endif

            _codeItem = codeItem;

            Name = codeItem.Name;
            _context = context;

            var dataResolversFactory = context.DataResolversFactory;

            _triggersResolver = dataResolversFactory.GetTriggersResolver();
            _constructorsResolver = dataResolversFactory.GetConstructorsResolver();
            _inheritanceResolver = dataResolversFactory.GetInheritanceResolver();

            _globalTriggersStorage = context.Storage.GlobalStorage.TriggersStorage;

            _executionCoordinator = new ExecutionCoordinator(this);
            _executionCoordinator.OnFinished += ExecutionCoordinator_OnFinished;

            _localCodeExecutionContext = new LocalCodeExecutionContext(parentCodeExecutionContext);
            var localStorageSettings = RealStorageSettingsHelper.Create(context, parentStorage);
            _storage = storageFactory.CreateStorage(localStorageSettings);

#if DEBUG
            Log($"_storage.Kind = {_storage.Kind}");
#endif

            _localCodeExecutionContext.Storage = _storage;
            _localCodeExecutionContext.Holder = Name;

#if DEBUG
            //Log($"_localCodeExecutionContext = {_localCodeExecutionContext}");
#endif

            RebuildSuperClassesStorages();

            if (!varList.IsNullOrEmpty())
            {
                var varStorage = _storage.VarStorage;

                foreach(var varItem in varList)
                {
                    varStorage.Append(varItem);
                }
            }
        }

        /// <inheritdoc/>
        public abstract KindOfInstance KindOfInstance { get; }

        protected readonly CodeItem _codeItem;

        /// <inheritdoc/>
        public StrongIdentifierValue Name { get; private set; }

        /// <inheritdoc/>
        public virtual float Priority => 0f;

        protected readonly IEngineContext _context;
        private readonly ITriggersStorage _globalTriggersStorage;
        protected readonly IStorage _storage;
        protected readonly LocalCodeExecutionContext _localCodeExecutionContext;

        private readonly TriggersResolver _triggersResolver;
        private readonly ConstructorsResolver _constructorsResolver;
        private readonly InheritanceResolver _inheritanceResolver;

        private InstanceState _instanceState = InstanceState.Created;
        private List<LogicConditionalTriggerInstance> _logicConditionalTriggersList = new List<LogicConditionalTriggerInstance>();
        private List<AddingFactNonConditionalTriggerInstance> _addingFactNonConditionalTriggerInstancesList = new List<AddingFactNonConditionalTriggerInstance>();
        private List<AddingFactConditionalTriggerInstance> _addingFactConditionalTriggerInstancesList = new List<AddingFactConditionalTriggerInstance>();

        protected IExecutionCoordinator _executionCoordinator;

        private List<IInstance> _childInstances = new List<IInstance>();
        private IInstance _parentInstance;
        private readonly object _childInstancesLockObj = new object();
      
        private readonly Dictionary<StrongIdentifierValue, IStorage> _superClassesStorages = new Dictionary<StrongIdentifierValue, IStorage>();
        private readonly object _superClassesStoragesLockObj = new object();

        /// <inheritdoc/>
        public IExecutionCoordinator ExecutionCoordinator => _executionCoordinator;

        /// <inheritdoc/>
        public void CancelExecution()
        {
            _executionCoordinator.ExecutionStatus = ActionExecutionStatus.Canceled;
        }

        public virtual void Init()
        {
            _instanceState = InstanceState.Initializing;

            _executionCoordinator.ExecutionStatus = ActionExecutionStatus.Executing;

            ApplyCodeDirectives();

            RunConstructors();

            RunInitialTriggers();

            RunMutuallyExclusiveStatesSets();

            RunExplicitStates();

            RunActivatorsOfStates();
            RunDeactivatorsOfStates();

#if DEBUG
            //Log($"Name = {Name}");
#endif

            var targetAddingFactTriggersList = _triggersResolver.ResolveAddFactTriggersList(Name, _localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

#if DEBUG

            //Log($"targetAddingFactTriggersList.Count = {targetAddingFactTriggersList.Count}");
            //Log($"targetAddingFactTriggersList = {targetAddingFactTriggersList.WriteListToString()}");
#endif

            if(targetAddingFactTriggersList.Any())
            {
                foreach(var targetTrigger in targetAddingFactTriggersList)
                {
#if DEBUG
                    //Log($"targetTrigger = {targetTrigger}");
#endif

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

            var targetLogicConditionalTriggersList = _triggersResolver.ResolveLogicConditionalTriggersList(Name, _localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

#if DEBUG
            //Log($"targetLogicConditionalTriggersList.Count = {targetLogicConditionalTriggersList.Count}");
            //Log($"targetLogicConditionalTriggersList = {targetLogicConditionalTriggersList.WriteListToString()}");
#endif

            if (targetLogicConditionalTriggersList.Any())
            {
                foreach (var targetTrigger in targetLogicConditionalTriggersList)
                {
#if DEBUG
                    //Log($"targetTrigger = {targetTrigger}");
#endif

                    var triggerInstance = new LogicConditionalTriggerInstance(targetTrigger, this, _context, _storage, _localCodeExecutionContext);
                    _logicConditionalTriggersList.Add(triggerInstance);

                    _globalTriggersStorage.Append(triggerInstance);

                    Task.Run(() => { triggerInstance.Init(); });                    
                }
            }

            _instanceState = InstanceState.Initialized;            
        }

        private void RebuildSuperClassesStorages()
        {
            var superClassesList = _inheritanceResolver.GetSuperClassesKeysList(Name, _localCodeExecutionContext);

#if DEBUG
            //Log($"superClassesList = {superClassesList.WriteListToString()}");
#endif

            lock(_superClassesStoragesLockObj)
            {
                if (superClassesList.Any())
                {
                    var existingKeys = _superClassesStorages.Keys.ToList();

#if DEBUG
                    //Log($"existingKeys = {existingKeys.WriteListToString()}");
#endif

                    var keysForAdding = superClassesList.Except(existingKeys);

#if DEBUG
                    Log($"keysForAdding = {keysForAdding.WriteListToString()}");
#endif

                    var keysForRemoving = existingKeys.Except(superClassesList);

#if DEBUG
                    Log($"keysForRemoving = {keysForRemoving.WriteListToString()}");
#endif
                    if(keysForAdding.Any())
                    {
                        //var parentStorage = _storage;

                        foreach (var key in keysForAdding)
                        {
                            var localStorageSettings = RealStorageSettingsHelper.Create(_context);

                            var storage = new SuperClassStorage(localStorageSettings, key);

                            _superClassesStorages[key] = storage;

                            _storage.AddParentStorage(storage);
                        }
                    }

                    foreach(var key in keysForRemoving)
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }

#if DEBUG
                var storagesList = _inheritanceResolver.GetStoragesList(_storage, KindOfStoragesList.CodeItems);
                foreach (var tmpStorage in storagesList)
                {
                    Log($"tmpStorage.Storage.Kind = {tmpStorage.Storage.Kind}");
                }
#endif
            }
        }

        /// <inheritdoc/>
        public virtual IList<IInstance> GetTopIndependentInstances()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual bool ActivateIdleAction()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void AddChildInstance(IInstance instance)
        {
#if DEBUG
            //Log($"instance = {instance}");
            //Log($"this = {this}");
#endif

            lock(_childInstancesLockObj)
            {
                if (_childInstances.Contains(instance))
                {
                    return;
                }

                _childInstances.Add(instance);

                instance.SetParent(this);
            }
        }

        /// <inheritdoc/>
        public void RemoveChildInstance(IInstance instance)
        {
#if DEBUG
            //Log($"instance = {instance}");
            //Log($"this = {this}");
#endif

            lock (_childInstancesLockObj)
            {
                if (!_childInstances.Contains(instance))
                {
                    return;
                }

                _childInstances.Remove(instance);

                instance.ResetParent(this);
            }
        }

        /// <inheritdoc/>
        public void SetParent(IInstance instance)
        {
#if DEBUG
            //Log($"instance = {instance}");
            //Log($"this = {this}");
#endif

            lock (_childInstancesLockObj)
            {
                if (_parentInstance == instance)
                {
                    return;
                }

                _parentInstance = instance;

                instance.AddChildInstance(this);
            }
        }

        /// <inheritdoc/>
        public void ResetParent(IInstance instance)
        {
#if DEBUG
            //Log($"instance = {instance}");
            //Log($"this = {this}");
#endif

            lock (_childInstancesLockObj)
            {
                if (_parentInstance != instance)
                {
                    return;
                }

                _parentInstance = null;

                instance.RemoveChildInstance(this);
            }
        }

        protected virtual void ApplyCodeDirectives()
        {
        }

        protected virtual void RunInitialTriggers()
        {
            RunLifecycleTriggers(KindOfSystemEventOfInlineTrigger.Enter);
        }

        protected virtual void RunConstructors()
        {
            var constructors = _constructorsResolver.Resolve(Name, _localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

            if(constructors.Any())
            {
                var processInitialInfoList = new List<ProcessInitialInfo>();

                constructors.Reverse();

                foreach (var constructor in constructors)
                {
                    var localCodeExecutionContext = new LocalCodeExecutionContext(_localCodeExecutionContext);

                    var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
                    localCodeExecutionContext.Storage = new LocalStorage(localStorageSettings);

                    localCodeExecutionContext.Holder = Name;

                    var processInitialInfo = new ProcessInitialInfo();
                    processInitialInfo.CompiledFunctionBody = constructor.CompiledFunctionBody;
                    processInitialInfo.LocalContext = localCodeExecutionContext;
                    processInitialInfo.Metadata = constructor;
                    processInitialInfo.Instance = this;
                    processInitialInfo.ExecutionCoordinator = _executionCoordinator;

                    processInitialInfoList.Add(processInitialInfo);
                }

#if DEBUG
                //Log($"processInitialInfoList = {processInitialInfoList.WriteListToString()}");
#endif

                var taskValue = _context.CodeExecutor.ExecuteBatchSync(processInitialInfoList);

#if DEBUG
                //Log($"taskValue = {taskValue}");
#endif
            }
        }

        protected void RunLifecycleTriggers(KindOfSystemEventOfInlineTrigger kindOfSystemEvent)
        {
            RunLifecycleTriggers(kindOfSystemEvent, Name);
        }

        protected void RunLifecycleTriggers(KindOfSystemEventOfInlineTrigger kindOfSystemEvent, IExecutionCoordinator executionCoordinator, bool normalOrder = true)
        {
            RunLifecycleTriggers(kindOfSystemEvent, Name, executionCoordinator, normalOrder);
        }

        protected void RunLifecycleTriggers(KindOfSystemEventOfInlineTrigger kindOfSystemEvent, StrongIdentifierValue holder)
        {
            RunLifecycleTriggers(kindOfSystemEvent, holder, _executionCoordinator);
        }

        protected void RunLifecycleTriggers(KindOfSystemEventOfInlineTrigger kindOfSystemEvent, StrongIdentifierValue holder,
            IExecutionCoordinator executionCoordinator, bool normalOrder = true)
        {
            var targetSystemEventsTriggersList = _triggersResolver.ResolveSystemEventsTriggersList(kindOfSystemEvent, holder, _localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

#if DEBUG
            //Log($"kindOfSystemEvent = {kindOfSystemEvent}");
            //if(kindOfSystemEvent == KindOfSystemEventOfInlineTrigger.Leave)
            //{
            //    Log($"targetSystemEventsTriggersList = {targetSystemEventsTriggersList.WriteListToString()}");
            //}
            //Log($"targetSystemEventsTriggersList = {targetSystemEventsTriggersList.WriteListToString()}");
#endif

            if (targetSystemEventsTriggersList.Any())
            {
                if(normalOrder)
                {
                    targetSystemEventsTriggersList.Reverse();
                }

                var processInitialInfoList = new List<ProcessInitialInfo>();

                foreach (var targetTrigger in targetSystemEventsTriggersList)
                {
                    var localCodeExecutionContext = new LocalCodeExecutionContext(_localCodeExecutionContext);

                    var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
                    localCodeExecutionContext.Storage = new LocalStorage(localStorageSettings);

                    localCodeExecutionContext.Holder = holder;

                    var processInitialInfo = new ProcessInitialInfo();
                    processInitialInfo.CompiledFunctionBody = targetTrigger.SetCompiledFunctionBody;
                    processInitialInfo.LocalContext = localCodeExecutionContext;
                    processInitialInfo.Metadata = targetTrigger;
                    processInitialInfo.Instance = this;
                    processInitialInfo.ExecutionCoordinator = executionCoordinator;

                    processInitialInfoList.Add(processInitialInfo);
                }

#if DEBUG
                //Log($"processInitialInfoList = {processInitialInfoList.WriteListToString()}");
#endif

                var taskValue = _context.CodeExecutor.ExecuteBatchAsync(processInitialInfoList);

#if DEBUG
                //Log($"taskValue = {taskValue}");
#endif
            }
        }

        protected virtual void RunMutuallyExclusiveStatesSets()
        {
        }

        protected virtual void RunExplicitStates()
        {
        }

        protected virtual void RunActivatorsOfStates()
        {
        }

        protected virtual void RunDeactivatorsOfStates()
        {
        }

        protected virtual void RunFinalizationTrigges()
        {
#if DEBUG
            //Log("Begin");
#endif

            var finalizationExecutionCoordinator = new ExecutionCoordinator(this);
            finalizationExecutionCoordinator.ExecutionStatus = ActionExecutionStatus.Executing;

            RunLifecycleTriggers(KindOfSystemEventOfInlineTrigger.Leave, finalizationExecutionCoordinator, false);

#if DEBUG
            //Log("End");
#endif
        }

        protected virtual void ExecutionCoordinator_OnFinished()
        {
#if DEBUG
            //Log("Begin");
            //Log($"this = {this}");
#endif

            if (_parentInstance != null)
            {
                _parentInstance.RemoveChildInstance(this);
                _parentInstance = null;
            }

            RunFinalizationTrigges();

            if(_childInstances.Any())
            {
#if DEBUG
                //Log($"_childInstances.Count = {_childInstances.Count}");
#endif

                foreach(var childInstance in _childInstances.ToList())
                {
#if DEBUG
                    //Log($"childInstance = {childInstance}");
#endif

                    childInstance.CancelExecution();
                }
            }

            Dispose();

#if DEBUG
            //Log("End");
#endif
        }

        /// <inheritdoc/>
        public virtual IExecutable GetExecutable(KindOfFunctionParameters kindOfParameters, IDictionary<StrongIdentifierValue, Value> namedParameters, IList<Value> positionedParameters)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual void SetPropertyValue(StrongIdentifierValue propertyName, Value value)
        {
#if DEBUG
            //DebugLogger.Instance.Info(this);
#endif

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual void SetVarValue(StrongIdentifierValue varName, Value value)
        {
#if DEBUG
            //DebugLogger.Instance.Info(this);
#endif

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Value GetPropertyValue(StrongIdentifierValue propertyName)
        {
#if DEBUG
            //DebugLogger.Instance.Info(this);
#endif

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Value GetVarValue(StrongIdentifierValue varName)
        {
#if DEBUG
            //DebugLogger.Instance.Info(this);
#endif

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            foreach (var triggerInstance in _logicConditionalTriggersList)
            {
                _globalTriggersStorage.Remove(triggerInstance);

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
    }
}
