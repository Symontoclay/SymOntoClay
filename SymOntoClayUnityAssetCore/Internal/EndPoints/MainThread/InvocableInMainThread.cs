/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
