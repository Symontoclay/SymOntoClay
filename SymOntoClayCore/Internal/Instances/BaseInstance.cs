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
        protected BaseInstance(CodeItem codeItem, IEngineContext context, IStorage parentStorage, IStorageFactory storageFactory, List<Var> varList)
            : base(context.Logger)
        {
#if DEBUG
            //Log($"parentStorage = {parentStorage}");
#endif

            _codeItem = codeItem;

            Name = codeItem.Name;
            _context = context;

            _globalTriggersStorage = context.Storage.GlobalStorage.TriggersStorage;

            _executionCoordinator = new ExecutionCoordinator(this);
            _executionCoordinator.OnFinished += ExecutionCoordinator_OnFinished;

            _localCodeExecutionContext = new LocalCodeExecutionContext();
            var localStorageSettings = RealStorageSettingsHelper.Create(context, parentStorage);
            _storage = storageFactory.CreateStorage(localStorageSettings);

            if(!varList.IsNullOrEmpty())
            {
                var varStorage = _storage.VarStorage;

                foreach(var varItem in varList)
                {
                    varStorage.Append(varItem);
                }
            }

            _localCodeExecutionContext.Storage = _storage;
            _localCodeExecutionContext.Holder = Name;

#if DEBUG
            //Log($"_localCodeExecutionContext = {_localCodeExecutionContext}");
#endif

            _triggersResolver = new TriggersResolver(context);
        }

        /// <inheritdoc/>
        public abstract KindOfInstance KindOfInstance { get; }

        protected readonly CodeItem _codeItem;

        /// <inheritdoc/>
        public StrongIdentifierValue Name { get; private set; }

        protected readonly IEngineContext _context;
        private readonly ITriggersStorage _globalTriggersStorage;
        protected readonly IStorage _storage;
        protected readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private readonly TriggersResolver _triggersResolver;
        private InstanceState _instanceState = InstanceState.Created;
        private List<LogicConditionalTriggerInstance> _logicConditionalTriggersList = new List<LogicConditionalTriggerInstance>();
        private List<AddingFactNonConditionalTriggerInstance> _addingFactNonConditionalTriggerInstancesList = new List<AddingFactNonConditionalTriggerInstance>();
        private List<AddingFactConditionalTriggerInstance> _addingFactConditionalTriggerInstancesList = new List<AddingFactConditionalTriggerInstance>();

        protected IExecutionCoordinator _executionCoordinator;

        private List<IInstance> _childInstances = new List<IInstance>();
        private IInstance _parentInstance;
        private readonly object _childInstancesLockObj = new object();

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
                    var targetTriggerInfo = targetTrigger.ResultItem;

#if DEBUG
                    //Log($"targetTrigger = {targetTrigger}");
                    //Log($"targetTriggerInfo = {targetTriggerInfo}");
#endif

                    if(targetTriggerInfo.SetCondition == null)
                    {
                        var triggerInstance = new AddingFactNonConditionalTriggerInstance(targetTriggerInfo, this, _context, _storage);
                        triggerInstance.Init();
                        _addingFactNonConditionalTriggerInstancesList.Add(triggerInstance);
                    }
                    else
                    {
                        var triggerInstance = new AddingFactConditionalTriggerInstance(targetTriggerInfo, this, _context, _storage);
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
                    //Log($"targetTrigger.ResultItem = {targetTrigger.ResultItem}");
#endif

                    var triggerInstance = new LogicConditionalTriggerInstance(targetTrigger.ResultItem, this, _context, _storage);
                    _logicConditionalTriggersList.Add(triggerInstance);

                    _globalTriggersStorage.Append(triggerInstance);

                    Task.Run(() => { triggerInstance.Init(); });                    
                }
            }

            _instanceState = InstanceState.Initialized;            
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
                    var localCodeExecutionContext = new LocalCodeExecutionContext();

                    var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
                    localCodeExecutionContext.Storage = new LocalStorage(localStorageSettings);

                    localCodeExecutionContext.Holder = holder;

                    var processInitialInfo = new ProcessInitialInfo();
                    processInitialInfo.CompiledFunctionBody = targetTrigger.ResultItem.SetCompiledFunctionBody;
                    processInitialInfo.LocalContext = localCodeExecutionContext;
                    processInitialInfo.Metadata = targetTrigger.ResultItem;
                    processInitialInfo.Instance = this;
                    processInitialInfo.ExecutionCoordinator = executionCoordinator;

                    processInitialInfoList.Add(processInitialInfo);
                }

#if DEBUG
                //Log($"processInitialInfoList = {processInitialInfoList.WriteListToString()}");
                //Log($"appInstanceExecutionCoordinator?.ExecutionStatus = {appInstanceExecutionCoordinator?.ExecutionStatus}");
                //Log($"stateExecutionCoordinator?.ExecutionStatus = {stateExecutionCoordinator?.ExecutionStatus}");
                //Log($"actionExecutionCoordinator?.ExecutionStatus = {actionExecutionCoordinator?.ExecutionStatus}");
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
