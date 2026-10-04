using SymOntoClay.ActiveObject.EventsInterfaces;
using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Common.Cancellation;
using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class ProcessInfoWaiter: IBaseFunctor, IOnCompletedActiveObjectHandler, IObjectWithPeriodicMethod
    {
        public static ProcessInfoWaiter Run(IMonitorLogger logger, IEngineContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor, string callMethodId, IProcessInfo waitingProcess, List<IExecutionCoordinator> executionCoordinators, ulong? cancelAfter, TimeoutCancellationMode timeoutCancellationMode, params IProcessInfo[] processes)
        {
            var waiter = new ProcessInfoWaiter(logger, true, context, threadPool, serializationAnchor, callMethodId, waitingProcess, executionCoordinators, cancelAfter, timeoutCancellationMode, processes);
            waiter.Run();
            return waiter;
        }

        public static ProcessInfoWaiter RunSync(IMonitorLogger logger, IEngineContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor, string callMethodId, IProcessInfo waitingProcess, List<IExecutionCoordinator> executionCoordinators, ulong? cancelAfter, TimeoutCancellationMode timeoutCancellationMode, params IProcessInfo[] processes)
        {
            var waiter = new ProcessInfoWaiter(logger, false, context, threadPool, serializationAnchor, callMethodId, waitingProcess, executionCoordinators, cancelAfter, timeoutCancellationMode, processes);
            waiter.RunSync();
            return waiter;
        }

        public ProcessInfoWaiter(IMonitorLogger logger, bool isAsync, IEngineContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor, string callMethodId, IProcessInfo waitingProcess, List<IExecutionCoordinator> executionCoordinators, ulong? cancelAfter, TimeoutCancellationMode timeoutCancellationMode, params IProcessInfo[] processes)
        {
            _logger = logger;
            _activeObjectContext = context.ActiveObjectContext;
            _dateTimeProvider = context.DateTimeProvider;
            _callMethodId = callMethodId;
            _waitingProcess = waitingProcess;
            _executionCoordinators = executionCoordinators;
            _cancelAfter = cancelAfter; 
            _timeoutCancellationMode = timeoutCancellationMode;
            _processes = processes;

            if (isAsync)
            {
                _activeObject = new AsyncActivePeriodicObject(context.ActiveObjectContext, threadPool, logger);
                _activeObject.AddOnCompletedHandler(this);
                _activeObject.ObjectWithPeriodicMethod = this;
            }
        }

        private IMonitorLogger _logger;
        private IActiveObjectContext _activeObjectContext;
        private IDateTimeProvider _dateTimeProvider;
        private IActivePeriodicObject _activeObject;
        private string _callMethodId;
        private IProcessInfo _waitingProcess;
        private List<IExecutionCoordinator> _executionCoordinators;
        private ulong? _cancelAfter;
        private TimeoutCancellationMode _timeoutCancellationMode;
        private IProcessInfo[] _processes;

        private float _initialTicks;

        /// <inheritdoc/>
        public void Run()
        {
            if (_processes.IsNullOrEmpty())
            {
                return;
            }

            Preparation();

            _activeObject.Start();
        }

        /// <inheritdoc/>
        public void RunSync()
        {
            if (_processes.IsNullOrEmpty())
            {
                return;
            }

            Preparation();

            while (true)
            {
                Iteration();
            }
        }

        private void Preparation()
        {
            _initialTicks = 0f;

            if (_cancelAfter.HasValue)
            {
                _initialTicks = _dateTimeProvider.CurrentTicks;
            }
        }

        private bool Iteration()
        {
            _logger.WaitProcessInfo("77298A46-278C-4DC9-B124-BB71D068EBB1", _waitingProcess?.Id, _waitingProcess?.ToLabel(_logger), _processes.Select(p => p.ToLabel(_logger)).ToList(), _callMethodId);

#if DEBUG
            //_logger.Info("F473B943-69C3-4B34-8D86-F7538F3A85B2", $"processes = {processes.Select(p => $"{p.Id}:{p.IsFinished(logger)};{p.ToHumanizedLabel()}").WritePODListToString()}");
#endif

            if (_processes.All(p => p.IsFinished(_logger)))
            {
                return false;
            }

            if (_executionCoordinators != null)
            {
                if (_executionCoordinators.Any(p => p.ExecutionStatus != ActionExecutionStatus.Executing))
                {
                    if (_executionCoordinators.Any(p => p.ExecutionStatus == ActionExecutionStatus.Canceled))
                    {
                        var cancelledExecutionCoordinatorsChangers = _executionCoordinators.Where(p => p.ExecutionStatus == ActionExecutionStatus.Canceled).Select(p => new Changer(KindOfChanger.ExecutionCoordinator, p.Id)).ToList();

                        foreach (var proc in _processes)
                        {
                            proc.Cancel(_logger, "15E111BA-A585-46AB-A73E-9A554CD128C4", ReasonOfChangeStatus.ByExecutionCoordinator, cancelledExecutionCoordinatorsChangers);
                        }
                    }
                    else
                    {
                        var executionCoordinatorsChangers = _executionCoordinators.Select(p => new Changer(KindOfChanger.ExecutionCoordinator, p.Id)).ToList();

                        foreach (var proc in _processes)
                        {
                            proc.WeakCancel(_logger, "589446A0-232E-4D86-A7F1-4E0A42BC13B8", ReasonOfChangeStatus.ByExecutionCoordinator, executionCoordinatorsChangers);
                        }
                    }

                    return false;
                }
            }

            if (_cancelAfter.HasValue)
            {
                var currentTick = _dateTimeProvider.CurrentTicks;

                var delta = currentTick - _initialTicks;

                if (delta >= _cancelAfter.Value)
                {
                    foreach (var proc in _processes)
                    {
                        switch (_timeoutCancellationMode)
                        {
                            case TimeoutCancellationMode.WeakCancel:
                                proc.WeakCancel(_logger, "6F69A1A8-5C55-4B33-B711-BDD4C4F39F83", ReasonOfChangeStatus.ByTimeout);
                                break;

                            case TimeoutCancellationMode.Cancel:
                                proc.Cancel(_logger, "208DD252-4216-40B9-BAAE-5049EBE3ED61", ReasonOfChangeStatus.ByTimeout);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(_timeoutCancellationMode), _timeoutCancellationMode, "157B1E7C-FC3A-425C-B1AF-33A1F7D00CAC");
                        }
                    }

                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        void IOnCompletedActiveObjectHandler.Invoke() 
        {
            _activeObject.RemoveOnCompletedHandler(this);
        }

        /// <inheritdoc/>
        bool IObjectWithPeriodicMethod.PeriodicHandler(ICancellationContext cancellationContext)
        {
            Thread.Sleep(100);

            return Iteration();
        }
    }
}
