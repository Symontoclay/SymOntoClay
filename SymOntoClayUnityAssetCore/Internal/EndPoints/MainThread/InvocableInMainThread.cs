using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread
{
    public class InvocableInMainThread : IInvocableInMainThread
    {
        public InvocableInMainThread(Action function, IInvokingInMainThread invokingInMainThread)
        {
            _function = function;
            _invokingInMainThread = invokingInMainThread;
        }

        private Action _function;
        private IInvokingInMainThread _invokingInMainThread;
        private bool _hasResult;
        private readonly object _lockObj = new object();

        public void Run()
        {
            _invokingInMainThread.SetInvocableObj(this);

            while (true)
            {
                lock (_lockObj)
                {
                    if (_hasResult)
                    {
                        break;
                    }
                }

                Thread.Sleep(10);
            }
        }

        /// <inheritdoc/>
        public void Invoke()
        {
            _function.Invoke();

            lock (_lockObj)
            {
                _hasResult = true;
            }
        }
    }

    public class InvocableInMainThreadObj<TResult> : IInvocableInMainThread
    {
        public InvocableInMainThreadObj(Func<TResult> function, IInvokingInMainThread invokingInMainThread)
        {
            _function = function;
            _invokingInMainThread = invokingInMainThread;
        }

        private Func<TResult> _function;
        private IInvokingInMainThread _invokingInMainThread;
        private bool _hasResult;
        private TResult _result;
        private readonly object _lockObj = new object();

        public TResult Run()
        {
            _invokingInMainThread.SetInvocableObj(this);

            while (true)
            {
                lock (_lockObj)
                {
                    if (_hasResult)
                    {
                        break;
                    }
                }

                Thread.Sleep(10);
            }

            return _result;
        }

        /// <inheritdoc/>
        public void Invoke()
        {
            _result = _function.Invoke();

            lock (_lockObj)
            {
                _hasResult = true;
            }
        }
    }
}
