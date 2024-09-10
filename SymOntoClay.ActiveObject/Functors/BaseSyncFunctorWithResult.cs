using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public partial class BaseSyncFunctorWithResult<TResult> : IBaseFunctor
    {
        public BaseSyncFunctorWithResult(IMonitorLogger logger, string functorId, Func<TResult> func, ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);
            _functorId = functorId;

            _func = func;
            _logger = logger;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private ISerializationAnchor _serializationAnchor;

        private Func<TResult> _func;

        private IMonitorLogger _logger;

        private bool _isFinished;

        private TResult _result;

        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            _result = _func();

            _isFinished = true;

            _serializationAnchor.RemoveFunctor(this);
        }

        public IMethodResponse<TResult> ToMethodResponse()
        {
            return new CompletedMethodResponse<TResult>(_result);
        }
    }

    public partial class BaseSyncFunctorWithResult<T, TResult> : IBaseFunctor
    {
        public BaseSyncFunctorWithResult(IMonitorLogger logger, string functorId, T arg, Func<T, TResult> func, ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);
            _functorId = functorId;

            _func = func;
            _arg = arg;
            _logger = logger;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private ISerializationAnchor _serializationAnchor;

        private Func<T, TResult> _func;
        private T _arg;

        private IMonitorLogger _logger;

        private bool _isFinished;

        private TResult _result;

        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            _result = _func(_arg);

            _isFinished = true;

            _serializationAnchor.RemoveFunctor(this);
        }

        public IMethodResponse<TResult> ToMethodResponse()
        {
            return new CompletedMethodResponse<TResult>(_result);
        }
    }

    public partial class BaseSyncFunctorWithResult<T1, T2, TResult> : IBaseFunctor
    {
        public BaseSyncFunctorWithResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, Func<T1, T2, TResult> func, ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);
            _functorId = functorId;

            _func = func;
            _arg1 = arg1;
            _arg2 = arg2;
            _logger = logger;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private ISerializationAnchor _serializationAnchor;

        private Func<T1, T2, TResult> _func;

        private T1 _arg1;
        private T2 _arg2;

        private IMonitorLogger _logger;

        private bool _isFinished;

        private TResult _result;

        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            _result = _func(_arg1, _arg2);

            _isFinished = true;

            _serializationAnchor.RemoveFunctor(this);
        }

        public IMethodResponse<TResult> ToMethodResponse()
        {
            return new CompletedMethodResponse<TResult>(_result);
        }
    }

    public partial class BaseSyncFunctorWithResult<T1, T2, T3, TResult> : IBaseFunctor
    {
        public BaseSyncFunctorWithResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, TResult> func, ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);
            _functorId = functorId;

            _func = func;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _logger = logger;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private ISerializationAnchor _serializationAnchor;

        private Func<T1, T2, T3, TResult> _func;

        private T1 _arg1;
        private T2 _arg2;
        private T3 _arg3;

        private IMonitorLogger _logger;

        private bool _isFinished;

        private TResult _result;

        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            _result = _func(_arg1, _arg2, _arg3);

            _isFinished = true;

            _serializationAnchor.RemoveFunctor(this);
        }

        public IMethodResponse<TResult> ToMethodResponse()
        {
            return new CompletedMethodResponse<TResult>(_result);
        }
    }
}
