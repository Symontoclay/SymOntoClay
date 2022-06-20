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

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread
{
    public class InvocableInMainThread : IInvocableInMainThread
    {
        public InvocableInMainThread(Action function, IInvokerInMainThread invokingInMainThread)
        {
            _function = function;
            _invokingInMainThread = invokingInMainThread;
        }

        private Action _function;
        private IInvokerInMainThread _invokingInMainThread;
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
        public InvocableInMainThreadObj(Func<TResult> function, IInvokerInMainThread invokingInMainThread)
        {
            _function = function;
            _invokingInMainThread = invokingInMainThread;
        }

        private Func<TResult> _function;
        private IInvokerInMainThread _invokingInMainThread;
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
