using NLog;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Monitor.Common;
using System;

namespace SymOntoClay.Core.Internal.Instances.InternalRunners
{
    public abstract class BaseOnceCodeRunner:
        IOnCompletedThreadExecutorHandler
    {
        private enum State
        {
            Created,
            CreatingExecutor,
            WaitingForFinishingExecutor,
            Finished
        }

        protected BaseOnceCodeRunner(IMonitorLogger logger)
        {
            _logger = logger;
        }

        private volatile State _state = State.Created;

        private readonly IMonitorLogger _logger;

        private IThreadExecutor _threadExecutor;

        public void Run(IMonitorLogger logger)
        {
#if DEBUG
            logger.Info("0A1D5B53-9E17-435A-B9D9-6DC1B92622A8", $"_state = {_state}");
#endif

            if(_state == State.Created)
            {
                _state = State.CreatingExecutor;
            }

#if DEBUG
            logger.Info("3CEA8D2E-F411-4FA7-8B85-6A0160E949C2", $"_state = {_state}");
#endif

            if (_state == State.CreatingExecutor)
            {
                _threadExecutor = CreateExecutor(logger);

                if(_threadExecutor == null)
                {
                    _state = State.Finished;
                    OnFinshed();
                }
                else
                {
                    _state = State.WaitingForFinishingExecutor;
                    _threadExecutor.AddOnCompletedHandler(this);
                }
            }

#if DEBUG
            logger.Info("E0E2254F-0AFA-4C70-8374-D0D51C18A82A", $"_state = {_state}");
#endif

            //throw new NotImplementedException("CC9F0E10-B02C-4B6D-BA0C-5957F95C3DA9");
        }

        protected abstract IThreadExecutor CreateExecutor(IMonitorLogger logger);
        protected abstract void OnFinshed();

        void IOnCompletedThreadExecutorHandler.Invoke()
        {
#if DEBUG
            _logger.Info("BB16FC05-49F3-43A0-802F-713CD1E670E3", "Run");
#endif

            _state = State.Finished;
            OnFinshed();
        }
    }
}
