/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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
            _codeItem = codeItem;

            Name = codeItem.Name;
            _context = context;

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

        protected readonly CodeItem _codeItem;

        /// <inheritdoc/>
        public StrongIdentifierValue Name { get; private set; }

        protected readonly IEngineContext _context;
        protected readonly IStorage _storage;
        protected readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private readonly TriggersResolver _triggersResolver;
        private InstanceState _instanceState = InstanceState.Created;
        private List<LogicConditionalTriggerInstance> _logicConditionalTriggersList = new List<LogicConditionalTriggerInstance>();

        protected IExecutionCoordinator _executionCoordinator;

        /// <inheritdoc/>
        public IExecutionCoordinator ExecutionCoordinator => _executionCoordinator;
        
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

                    var triggerInstanceInfo = new LogicConditionalTriggerInstance(targetTrigger.ResultItem, this, _context, _storage);
                    _logicConditionalTriggersList.Add(triggerInstanceInfo);

                    Task.Run(() => { triggerInstanceInfo.Init(); });                    
                }
            }

            _instanceState = InstanceState.Initialized;            
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
                    processInitialInfo.CompiledFunctionBody = targetTrigger.ResultItem.CompiledFunctionBody;
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
#endif

            RunFinalizationTrigges();

            Dispose();

#if DEBUG
            //Log("End");
#endif
        }

        protected void AppInstanceExecutionCoordinator_OnFinished()
        {
#if DEBUG
            //Log("Begin");
#endif

            RunFinalizationTrigges();

            Dispose();

#if DEBUG
            //Log("End");
#endif
        }

        protected virtual void StateExecutionCoordinator_OnFinished()
        {
#if DEBUG
            //Log("Begin");
#endif

            RunFinalizationTrigges();

            Dispose();

#if DEBUG
            //Log("End");
#endif
        }

        protected void ActionExecutionCoordinator_OnFinished()
        {
#if DEBUG
            //Log("Begin");
#endif

            RunFinalizationTrigges();

            Dispose();

#if DEBUG
            //Log("End");
#endif
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            foreach (var triggerInstanceInfo in _logicConditionalTriggersList)
            {
                triggerInstanceInfo.Dispose();
            }

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

            sb.PrintBriefObjProp(n, nameof(Name), Name);
            sb.AppendLine($"{spaces}{nameof(_instanceState)} = {_instanceState}");

            return sb.ToString();
        }
    }
}
