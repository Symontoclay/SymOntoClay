using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public abstract partial class BaseSyncFunctorWithoutResult : IBaseFunctor
    {
        protected BaseSyncFunctorWithoutResult(IMonitorLogger logger, string functorId, Action action, ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);

            _functorId = functorId;

            _action = action;
            _logger = logger;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private ISerializationAnchor _serializationAnchor;

        private Action _action;

        private IMonitorLogger _logger;

        private bool _isFinished;

        public void Run()
        {
            if(_isFinished)
            {
                return;
            }

            _action();

            _isFinished = true;

            _serializationAnchor.RemoveFunctor(this);
        }

        public IMethodResponse ToMethodResponse()
        {
            return CompletedMethodResponse.Instance;
        }
    }

    public abstract partial class BaseSyncFunctorWithoutResult<T> : IBaseFunctor
    {
        protected BaseSyncFunctorWithoutResult(IMonitorLogger logger, string functorId, T arg, Action<T> action, ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);

            _functorId = functorId;

            _action = action;
            _arg = arg;
            _logger = logger;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private ISerializationAnchor _serializationAnchor;

        private Action<T> _action;

        private T _arg;

        private IMonitorLogger _logger;

        private bool _isFinished;

        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            _action(_arg);

            _isFinished = true;

            _serializationAnchor.RemoveFunctor(this);
        }

        public IMethodResponse ToMethodResponse()
        {
            return CompletedMethodResponse.Instance;
        }
    }

    public abstract partial class BaseSyncFunctorWithoutResult<T1, T2> : IBaseFunctor
    {
        protected BaseSyncFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, Action<T1, T2> action, ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);

            _functorId = functorId;

            _action = action;
            _arg1 = arg1;
            _arg2 = arg2;
            _logger = logger;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private ISerializationAnchor _serializationAnchor;

        private Action<T1, T2> _action;

        private T1 _arg1;
        private T2 _arg2;

        private IMonitorLogger _logger;

        private bool _isFinished;

        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            _action(_arg1, _arg2);

            _isFinished = true;

            _serializationAnchor.RemoveFunctor(this);
        }

        public IMethodResponse ToMethodResponse()
        {
            return CompletedMethodResponse.Instance;
        }
    }

    public abstract partial class BaseSyncFunctorWithoutResult<T1, T2, T3> : IBaseFunctor
    {
        protected BaseSyncFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, Action<T1, T2, T3> action, ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);

            _functorId = functorId;

            _action = action;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _logger = logger;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private ISerializationAnchor _serializationAnchor;

        private Action<T1, T2, T3> _action;

        private T1 _arg1;
        private T2 _arg2;
        private T3 _arg3;

        private IMonitorLogger _logger;

        private bool _isFinished;

        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            _action(_arg1, _arg2, _arg3);

            _isFinished = true;

            _serializationAnchor.RemoveFunctor(this);
        }

        public IMethodResponse ToMethodResponse()
        {
            return CompletedMethodResponse.Instance;
        }
    }
}
