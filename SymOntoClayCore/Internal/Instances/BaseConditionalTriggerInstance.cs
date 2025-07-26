using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.Instances
{
    public abstract class BaseConditionalTriggerInstance: BaseComponent,
        IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        protected BaseConditionalTriggerInstance(BaseInstance parent, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext)
            : base(context.Logger)
        {
            Id = NameHelper.GetNewEntityNameString();

            _executionCoordinator = parent.ExecutionCoordinator;
            _context = context;
            _globalLogicalStorage = context.Storage.GlobalStorage.LogicalStorage;
            _parent = parent;

            _dateTimeProvider = context.DateTimeProvider;

            _activeObjectContext = context.ActiveObjectContext;
            _threadPool = context.AsyncEventsThreadPool;
            _serializationAnchor = new SerializationAnchor();

            var localCodeExecutionContext = new LocalCodeExecutionContext(parentCodeExecutionContext);
            var localStorageSettings = RealStorageSettingsHelper.Create(context, parentStorage);
            _storage = new LocalStorage(localStorageSettings);
            localCodeExecutionContext.Storage = _storage;
            localCodeExecutionContext.Holder = parent.Name;
            localCodeExecutionContext.Instance = parent;

            _localCodeExecutionContext = localCodeExecutionContext;

            _triggerConditionNodeObserverContext = new TriggerConditionNodeObserverContext(context, _storage, parent.Name);

            _activeObject = new AsyncActivePeriodicObject(context.ActiveObjectContext, context.TriggersThreadPool, Logger);
            _activeObject.PeriodicMethod = Handler;
        }

        public string Id { get; }

        protected readonly TriggerConditionNodeObserverContext _triggerConditionNodeObserverContext;
        protected readonly IDateTimeProvider _dateTimeProvider;

        protected IActiveObjectContext _activeObjectContext;
        protected ICustomThreadPool _threadPool;
        protected SerializationAnchor _serializationAnchor;

        protected readonly IExecutionCoordinator _executionCoordinator;
        protected readonly IEngineContext _context;
        protected readonly ILogicalStorage _globalLogicalStorage;
        protected readonly BaseInstance _parent;

        protected readonly IStorage _storage;
        protected readonly ILocalCodeExecutionContext _localCodeExecutionContext;

        private IActivePeriodicObject _activeObject;

        protected readonly object _lockObj = new object();
        protected volatile bool _needRun;

        private int _runInterval = 100;

        /// <inheritdoc/>
        public bool IsOn => _triggerConditionNodeObserverContext.IsOn;

        public virtual void Init(IMonitorLogger logger)
        {
            _activeObject.Start();

            _needRun = true;
        }

        private bool Handler(CancellationToken cancellationToken)
        {
            try
            {
#if DEBUG
                //Info("0611DFCC-6C77-4E9D-A587-96BC0E9189D7", $"_needRun = {_needRun};{_trigger.ToHumanizedLabel()}");
#endif

                Thread.Sleep(_runInterval);

                lock (_lockObj)
                {
                    if (!_needRun)
                    {
                        return true;
                    }

                    _needRun = false;
                }

                DoSearch(Logger);

                return true;
            }
            catch (Exception e)
            {
                Error("BB283A28-43CB-4B3B-957A-054B699039EE", e);

                throw;
            }
        }

        protected abstract void DoSearch(IMonitorLogger logger);

        protected void SetInitialTime()
        {
            var currentTicks = _dateTimeProvider.CurrentTicks;

            if (_triggerConditionNodeObserverContext.IsOn)
            {
                if (!_triggerConditionNodeObserverContext.InitialResetTime.HasValue)
                {
                    _triggerConditionNodeObserverContext.InitialResetTime = currentTicks;
                }

                _triggerConditionNodeObserverContext.InitialSetTime = currentTicks;
            }
            else
            {
                if (_triggerConditionNodeObserverContext.InitialResetTime.HasValue)
                {
                    _triggerConditionNodeObserverContext.InitialResetTime = null;
                }

                if (!_triggerConditionNodeObserverContext.InitialSetTime.HasValue)
                {
                    _triggerConditionNodeObserverContext.InitialSetTime = currentTicks;
                }
            }
        }

        protected void SetIsOn(IMonitorLogger logger, string messagePointId, string doTriggerSearchId, bool value)
        {
            _triggerConditionNodeObserverContext.IsOn = value;

            if (value)
            {
                logger.SetConditionalTrigger(messagePointId, doTriggerSearchId);
            }
            else
            {
                logger.ResetConditionalTrigger(messagePointId, doTriggerSearchId);
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _activeObject.Dispose();

            _serializationAnchor.Dispose();

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
            sb.Append($"{spaces}{nameof(Id)} = {Id}");
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
            sb.Append($"{spaces}{nameof(Id)} = {Id}");
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
            sb.Append($"{spaces}{nameof(Id)} = {Id}");
            return sb.ToString();
        }
    }
}
